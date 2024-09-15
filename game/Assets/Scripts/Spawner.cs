using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public int spawnCount = 1;
    public Vector3 randomRotationOnAxis = new(0, 1, 0);
    public float timeBetweenSpawns = 1f;
    public bool oneShot;
    public Vector3 spawnArea = new(1, 1, 1);
    public float spawnRadius = 10f;
    public bool placeOnGround = false;
    public Transform parentTransform;

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
        var spawnPos = PickARandomPosition();
        var rotation = Quaternion.Euler(randomRotationOnAxis * Random.Range(-180f, 180f));
        Instantiate(prefabToSpawn, spawnPos, rotation, parentTransform);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
