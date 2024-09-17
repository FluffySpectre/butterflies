using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public int spawnCount = 1;
    public Vector3 randomRotationOnAxis = new(0, 1, 0);
    public float timeBetweenSpawns = 1f;
    public bool oneShot;
    public float spawnRadius = 10f;
    public bool placeOnGround = false;
    public Transform parentTransform;
    public LayerMask blockLayer;

    private bool spawned = false;

    private void Update()
    {
        if (oneShot && !spawned)
        {
            spawned = true;
            for (int i = 0; i < spawnCount; i++)
            {
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        var spawnPos = Vector3.zero;
        var spawnPosFound = false;
        var numTries = 5;
        while (numTries-- > 0)
        {
            spawnPos = PickARandomPosition();
            if (CheckPosition(spawnPos))
            {
                spawnPosFound = true;
                break;
            }
        }

        if (spawnPosFound)
        {
            var rotation = Quaternion.Euler(randomRotationOnAxis * Random.Range(-180f, 180f));
            Instantiate(prefabToSpawn, spawnPos, rotation, parentTransform);
        }
    }

    private Vector3 PickARandomPosition()
    {
        var pos = Random.insideUnitSphere * spawnRadius + transform.position;

        if (pos.y < 0f)
        {
            pos = new(pos.x, transform.position.y + Mathf.Abs(pos.y), pos.z);
        }

        if (placeOnGround)
        {
            pos = new(pos.x, 0f, pos.z);
        }

        return pos;
    }

    private bool CheckPosition(Vector3 pos)
    {
        var checkRadius = 2f;
        var colliders = Physics.OverlapSphere(pos, checkRadius, blockLayer);
        if (colliders.Length > 0)
        {
            return false;
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
