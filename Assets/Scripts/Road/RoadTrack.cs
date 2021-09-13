using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadTrack : MonoBehaviour
{
    [SerializeField] private SpawnPoint[] _spawnPoints;

    [SerializeField] private GameObject[] _obstaclePool;

    [Range(0, 1.5f)]
    public float spawnDeviation = 1f;
    
    private void Start()
    {
        foreach (var spawnPoint in _spawnPoints)
        {
            StartCoroutine(SpawnObstacle(spawnPoint, spawnPoint.frequency));
        }
    }

    public void DisableSpawnPoints()
    {
        foreach (var spawnPoint in _spawnPoints)
        {
            spawnPoint.active = false;
        }
    }

    /// <summary>
    /// Coroutine that continually spawns obstacles.
    /// Uses spawnDeviation to add variance to spawn frequency
    /// </summary>
    /// <param name="spawnPoint">The transform used to instantiate the spawned obstacle</param> 
    /// <param name="frequency">Frequency of spawns in seconds</param> 
    /// <returns></returns>
    private IEnumerator SpawnObstacle(SpawnPoint spawnPoint, float frequency)
    {
        var deviation = Random.Range(-spawnDeviation, spawnDeviation);
        if (frequency + deviation < 0) deviation = 0;
        
        Transform spawnTransform = spawnPoint.transform;
        
        // The reason we don't use Transform as a parameter is because of this bool:
        while (spawnPoint.active)
        {
            yield return new WaitForSeconds(frequency + deviation);
            
            Instantiate(_obstaclePool[Random.Range(0, _obstaclePool.Length)], spawnTransform.position, spawnTransform.rotation);
        }
    }
}
