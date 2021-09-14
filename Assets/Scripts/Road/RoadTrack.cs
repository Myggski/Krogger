using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadTrack : MonoBehaviour {
    [SerializeField]
    private SpawnPoint[] spawnPoints;

    [SerializeField]
    private GameObject[] obstaclePool;

    private readonly List<GameObject> _spawnedObstacles = new List<GameObject>();

    [Range(0, 1.5f)]
    public float spawnDeviation = 1f;

    private void Start() {
        foreach (var spawnPoint in spawnPoints) {
            StartCoroutine(SpawnObstacle(spawnPoint, spawnPoint.frequency));
        }
    }

    /// <summary>
    /// Stops spawning of cars by killing all coroutines
    /// </summary>
    public void DisableSpawnPoints() {
        StopAllCoroutines();
    }

    /// <summary>
    /// Explodes all cars this track has spawned
    /// </summary>
    public void BoomAllCars() {
        foreach (var obst in _spawnedObstacles) {
            // TODO: find a better way maybe? not that performance is an issue...
            if (obst != null) {
                obst.GetComponent<Obstacle>().GoBoomNow();
            }
        }
    }

    /// <summary>
    /// Coroutine that continually spawns obstacles.
    /// Uses spawnDeviation to add variance to spawn frequency
    /// </summary>
    /// <param name="spawnPoint">The transform used to instantiate the spawned obstacle</param> 
    /// <param name="frequency">Frequency of spawns in seconds</param> 
    /// <returns></returns>
    private IEnumerator SpawnObstacle(SpawnPoint spawnPoint, float frequency) {
        var deviation = Random.Range(-spawnDeviation, spawnDeviation);
        if (frequency + deviation < 0) deviation = 0;

        Transform spawnTransform = spawnPoint.transform;

        // The reason we don't use Transform as a parameter is because of this bool:
        while (spawnPoint.active) {
            yield return new WaitForSeconds(frequency + deviation);

            _spawnedObstacles.Add(Instantiate(obstaclePool[Random.Range(0, obstaclePool.Length)],
                spawnTransform.position, spawnTransform.rotation));
        }
    }
}
