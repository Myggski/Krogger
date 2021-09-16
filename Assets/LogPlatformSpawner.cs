using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class LogPlatformSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject logPrefab;

    [SerializeField]
    private GameObject test;
    
    [Tooltip("Max logs to be spawned")]
    [SerializeField] 
    private int maxLogs = 4;

    [SerializeField]
    private bool isDoubleLane = false;
    
    private Transform _trackTransform;
    private const float UNIT_SIZE_PER_LOG = 3;

    private LayerMask obstacleLayerMask = 1 << 6;

    private Collider[] placedLogs = new Collider[10];


    private Vector3 gizmo_pos;
    private Transform gizmo_transform;

    // Start is called before the first frame update
    void Start()
    {
        _trackTransform = transform;
        SpawnLogs();
    }

    private void SpawnLogs()
    {
        for (int i = 0; i < maxLogs; i++)
        {
            PlaceLog();
        }
    }

    private void PlaceLog()
    {
        var log = Instantiate(logPrefab, Vector3.one, transform.rotation);
        log.transform.position = GetLogPosition(log, 0); 

    }

    private Vector3 GetLogPosition(GameObject log, int count)
    {
        int maxPos = Mathf.RoundToInt((_trackTransform.localScale.z / 2f) - (log.transform.localScale.z * UNIT_SIZE_PER_LOG)/2);
        int randomXPos = Random.Range(-maxPos, maxPos);
        Vector3 trackPosition = _trackTransform.position;
        
        Vector3 positionCandidate = new Vector3(randomXPos, trackPosition.y, trackPosition.z);

        if (ValidLogSpawnPosition(log, positionCandidate))
        {
            return positionCandidate;
        }

        if (count >= 5)
        {
            if (log.transform.localScale.z < 1) return Vector3.zero;
            log.transform.localScale += Vector3.back;
            count = 0;
        }

        return GetLogPosition(log, count++);
    }

    private bool ValidLogSpawnPosition(GameObject log, Vector3 position)
    {
        Vector3 logTransform = new Vector3(log.transform.localScale.z * UNIT_SIZE_PER_LOG, log.transform.localScale.y,
            log.transform.localScale.x ); //* UNIT_SIZE_PER_LOG
        
        //Vector3 logColliderSize = log.GetComponentInChildren<BoxCollider>().size;

        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = new Vector3(log.transform.localScale.z * UNIT_SIZE_PER_LOG, log.transform.localScale.y,
            log.transform.localScale.x);
        

        Vector3 pos = position - new Vector3(0, 0.1f, 0);

        //var testboy = Instantiate(test, position, log.transform.rotation);
        //testboy.transform.localScale = log.transform.localScale;

        int hits = Physics.OverlapBoxNonAlloc(position, logTransform/2, placedLogs, Quaternion.identity,
            obstacleLayerMask);
        
        

        print("hits: " + hits);
        return hits == 0;
    }
}
