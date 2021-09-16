using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FG {
    public class RoadTrack : MonoBehaviour {
        [SerializeField]
        private List<SpawnPoint> spawnPoints;
        [Range(0, 2f)]
        [SerializeField]
        private float spawnDeviation = 1f;
        [SerializeField]
        private GameObject[] obstaclePool;
        [SerializeField]
        private float minObstacleSpeed = 400f;
        [SerializeField]
        private float maxObstacleSpeed = 1200f;

        private float _randomizedObstacleSpeed;
        private readonly List<GameObject> _spawnedObstacles = new List<GameObject>();

        /// <summary>
        /// Randomize the obstacle speed
        /// </summary>
        private void Setup() {
            _randomizedObstacleSpeed = Random.Range(minObstacleSpeed, maxObstacleSpeed);
        }

        /// <summary>
        /// Shuffles the spawn points, and set the first spawn point(s) as active
        /// </summary>
        private void ShuffleAndActivateRandomSpawnPoints() {
            List<SpawnPointPositionType> spawnedPositionTypes = new List<SpawnPointPositionType>();

            // Shuffles the spawn point list
            spawnPoints = spawnPoints
                .OrderBy(point => Random.Range(1, 1000))
                .ToList();

            // Spawning the obstacles, and if it meets the requirements with the spawnPositionTypes, it becomes active
            for (int i = 0; i < spawnPoints.Count - 1; i++) {
                SpawnPoint spawnPoint = spawnPoints[i];
                spawnPoint.active = !spawnedPositionTypes.Any()
                                    || !spawnedPositionTypes.Contains(spawnPoint.spawnPointPositionType);


                spawnedPositionTypes.Add(spawnPoint.spawnPointPositionType);
                StartCoroutine(StartObstacleSpawner(spawnPoint, spawnPoint.frequency));
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
                    obst.GetComponent<MovingObstacle>().GoBoomNow();
                }
            }
        }
        
        /// <summary>
        /// Get number of seconds to wait until next obstacle will spawn
        /// </summary>
        /// <param name="frequency">Frequency of spawns in seconds</param> 
        /// <returns></returns>
        private float NumberOfGetSecondsToWait(float frequency) {
            float deviation = Random.Range(-spawnDeviation, spawnDeviation);
            float secondsToWait = frequency + deviation;
            
            if (secondsToWait < 0) {
                secondsToWait = 0;
            }

            return secondsToWait;
        }

        /// <summary>
        /// Spawns an obstacle and setup the speed of the obstacle
        /// </summary>
        /// <param name="spawnTransform"></param>
        private void SpawnObstacle(Transform spawnTransform) {
            GameObject movingObstacle = Instantiate(obstaclePool[Random.Range(0, obstaclePool.Length)],
                spawnTransform.position, spawnTransform.rotation);
            movingObstacle.GetComponent<MovingObstacle>()?
                .Initialize(_randomizedObstacleSpeed, transform.localScale.z);

            _spawnedObstacles.Add(movingObstacle);
        }

        /// <summary>
        /// Coroutine that continually spawns obstacles.
        /// Uses spawnDeviation to add variance to spawn frequency
        /// </summary>
        /// <param name="spawnPoint">The transform used to instantiate the spawned obstacle</param> 
        /// <param name="frequency">Frequency of spawns in seconds</param> 
        /// <returns></returns>
        private IEnumerator StartObstacleSpawner(SpawnPoint spawnPoint, float frequency) {
            Transform spawnTransform = spawnPoint.transform;

            // The reason we don't use Transform as a parameter is because of this bool:
            while (spawnPoint.active) {
                SpawnObstacle(spawnTransform);

                yield return new WaitForSeconds(NumberOfGetSecondsToWait(frequency));
            }
        }

        private void Awake() {
            Setup();
        }

        private void OnEnable() {
            ShuffleAndActivateRandomSpawnPoints();
        }
    }
}
