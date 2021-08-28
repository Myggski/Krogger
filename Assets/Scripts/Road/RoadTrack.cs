using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadTrack : MonoBehaviour
{
    [SerializeField] private SpawnPoint[] _spawnPoints;

    [SerializeField] private GameObject[] _obstaclePool;
    private void Start()
    {
        foreach (var spawnPoint in _spawnPoints)
        {
            StartCoroutine(SpawnCar(spawnPoint, spawnPoint.frequency));
        }
    }

    private IEnumerator SpawnCar(SpawnPoint spawnPoint, float frequency)
    {
        Transform spawnTransform = spawnPoint.transform;
        while (spawnPoint.active)
        {
            yield return new WaitForSeconds(frequency);
            
            Instantiate(_obstaclePool[Random.Range(0, _obstaclePool.Length)], spawnTransform.position, spawnTransform.rotation);
        }
    }
}
