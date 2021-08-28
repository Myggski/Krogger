using System;
using System.Collections;
using System.Collections.Generic;
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
            StartCoroutine(SpawnCar(spawnPoint, spawnPoint.frequency));
        }
    }

    private IEnumerator SpawnCar(SpawnPoint spawnPoint, float frequency)
    {
        var deviation = Random.Range(-spawnDeviation, spawnDeviation);
        if (frequency + deviation < 0) deviation = 0;
        Transform spawnTransform = spawnPoint.transform;
        while (spawnPoint.active)
        {
            yield return new WaitForSeconds(frequency + deviation);
            
            Instantiate(_obstaclePool[Random.Range(0, _obstaclePool.Length)], spawnTransform.position, spawnTransform.rotation);
        }
    }
}
