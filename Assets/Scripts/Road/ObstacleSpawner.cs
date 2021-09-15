using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Spawns random gameObjects at random x-position
/// </summary>
public class ObstacleSpawner : MonoBehaviour {
    [Header("Trees")]
    [SerializeField]
    private int minToSpawn = 3;
    [SerializeField]
    private int maxToSpawn = 10;
    [SerializeField]
    [Tooltip("It's how close a neighbour-tree can be placed, it should be the same as player movement")]
    private int gameObjectPlacedPerUnit = 3;
    [SerializeField]
    private List<GameObject> prefabsToSpawn = new List<GameObject>();

    // Data information
    private int _maxPositionZ;
    private Transform _parentTransform;
    private List<GameObject> _placedGameObject;

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
        int randomXPosition = Random.Range(-_maxPositionZ + gameObjectPlacedPerUnit, _maxPositionZ - gameObjectPlacedPerUnit + 1);
        bool isPositionInList = _placedGameObject
            .Exists(obstacle => obstacle.transform.position.z.Equals(randomXPosition));

        if (randomXPosition % gameObjectPlacedPerUnit == 0 && !isPositionInList) {
            return randomXPosition;
        }

        return GetRandomXPosition();
    }

    /// <summary>
    /// Get position to spawn randomly
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomSpawningPosition() {
        Vector3 parentPosition = _parentTransform.position;
        return new Vector3(GetRandomXPosition(), parentPosition.y, parentPosition.z);
    }

    /// <summary>
    /// Setup and initializing the spawning of game objects
    /// </summary>
    private void InitializeSpawning() {
        _parentTransform = transform;
        _maxPositionZ = Mathf.RoundToInt(_parentTransform.localScale.z / 2f);
        _placedGameObject = new List<GameObject>();

        SpawnObjects();
    }

    /// <summary>
    /// Spawns random numbers of game objects at random x-position
    /// </summary>
    private void SpawnObjects() {
        int numberOfObjectsToSpawn = Random.Range(minToSpawn, maxToSpawn + 1);
        
        for (int i = 0; i < numberOfObjectsToSpawn; i++) {
            Vector3 gameObjectPosition = GetRandomSpawningPosition();
            GameObject spawnedObject = Instantiate(GetRandomObject(), gameObjectPosition, _parentTransform.rotation);

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
