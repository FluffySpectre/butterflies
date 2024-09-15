using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public int spawnCount = 1;
    public Vector3 randomRotationOnAxis = new(0, 1, 0);
    public float timeBetweenSpawns = 1f;
    public bool oneShot;
    public Vector3 spawnArea = new(1, 1, 1);
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
        return new Vector3(Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f), Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f), Random.Range(-spawnArea.z / 2f, spawnArea.z / 2f)) + transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }
}
