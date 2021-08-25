using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadTrack : MonoBehaviour
{
    [SerializeField] private SpawnPoint[] _spawnPoints;

    [SerializeField] private GameObject[] _carPool;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var spawnPoint in _spawnPoints)
        {
            StartCoroutine(SpawnCar(spawnPoint, spawnPoint.frequency));
        }
    }

    IEnumerator SpawnCar(SpawnPoint spawnPoint, float frequency)
    {
        Transform spawnTransform = spawnPoint.transform;
        while (spawnPoint.active)
        {
            yield return new WaitForSeconds(frequency);
            
            Instantiate(_carPool[Random.Range(0, _carPool.Length)], spawnTransform.position, spawnTransform.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
