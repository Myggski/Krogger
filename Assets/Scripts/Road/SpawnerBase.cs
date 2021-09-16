using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace FG {
    public abstract class SpawnerBase : MonoBehaviour {
        [Header("Spawnable")]
        [SerializeField]
        protected int minToSpawn = 3;
        [SerializeField]
        protected int maxToSpawn = 10;
        [Range(0, 1)]
        [SerializeField]
        [Tooltip("Percentage chance for the item to spawn")]
        private float chanceOfSpawning = 1f;
        [SerializeField]
        [Tooltip("It's how close a neighbour-tree can be placed, it should be the same as player movement")]
        private int gameObjectPlacedPerUnit = 3;
        [SerializeField]
        private List<GameObject> prefabsToSpawn = new List<GameObject>();
        
        // Data information
        private Transform _parentTransform;
        protected int _maxPositionZ;
        protected List<GameObject> _placedGameObject = new List<GameObject>();
        
        /// <summary>
        /// Returns random GameObject from list
        /// </summary>
        /// <returns></returns>
        private GameObject GetRandomObject() {
            return prefabsToSpawn[Random.Range(0, prefabsToSpawn.Count)];
        }

        /// <summary>
        /// Gets random-position that is divided by 3, and is not already in list
        /// </summary>
        /// <returns></returns>
        private int GetRandomXPosition() {
            // Can spawn in x-position from -30 to 30, and removes gameObjectPlacedPerUnit on both sides for some padding
            // Else half the trees at the edges spawns outside of the map  
            int randomXPosition = Random.Range(-_maxPositionZ + gameObjectPlacedPerUnit,
                _maxPositionZ - gameObjectPlacedPerUnit + 1);
            bool isPositionInList = _placedGameObject
                .Exists(obstacle => obstacle.transform.position.z.Equals(randomXPosition));

            if (randomXPosition % gameObjectPlacedPerUnit == 0 && !isPositionInList) {
                return randomXPosition;
            }

            return GetRandomXPosition();
        }

        /// <summary>
        /// Get position to spawn randomly
        /// Need to remove or add diff on tracks that are wider than 3 units (2 lanes tracks) or else
        /// game objects will spawn between lanes
        /// </summary>
        /// <param name="objectToSpawnPosition">The position of the object that's about to spawn</param>
        /// <returns></returns>
        private Vector3 GetRandomSpawningPosition(Vector3 objectToSpawnPosition) {
            float parentZPosition = _parentTransform.position.z;
            float amountOfDiff = parentZPosition % 3;
            float diffToRemoveOrAdd = Random.Range(0, 1) > 0.5f 
                ? -amountOfDiff 
                : amountOfDiff;
            
            return new Vector3(GetRandomXPosition(), objectToSpawnPosition.y,  parentZPosition + diffToRemoveOrAdd);
        }
        
        /// <summary>
        /// Setup and initializing the spawning of game objects
        /// </summary>
        private void InitializeSpawning() {
            float rollChanceOfSpawn = Random.Range(0f, 1f);

            // Checks if it should spawn or not
            if (!(rollChanceOfSpawn <= chanceOfSpawning)) {
                return;
            }

            _parentTransform = transform;
            _maxPositionZ = Mathf.RoundToInt(_parentTransform.localScale.z / 2f);

            SpawnObjects();
        }

        /// <summary>
        /// Spawns random numbers of game objects at random x-position
        /// </summary>
        private void SpawnObjects() {
            int numberOfObjectsToSpawn = Random.Range(minToSpawn, maxToSpawn + 1);

                for (int i = 0; i < numberOfObjectsToSpawn; i++) {
                    GameObject prefabToSpawn = GetRandomObject();
                    Vector3 gameObjectPosition = GetRandomSpawningPosition(prefabToSpawn.transform.position);
                    GameObject spawnedObject = 
                        Instantiate(prefabToSpawn, gameObjectPosition, prefabToSpawn.transform.rotation);

                    _placedGameObject.Add(spawnedObject);
                }
        }
        
        /// <summary>
        /// Removes all the obstacles that has been spawned
        /// </summary>
        private void TrackCleanup() {
            if (_placedGameObject.Any()) {
                _placedGameObject.ForEach(Destroy);
            }
        }

        private void Awake() {
            InitializeSpawning();
        }

        private void OnDestroy() {
            TrackCleanup();
        }
    }
}