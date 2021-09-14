using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    [SerializeField] 
    private GameObject player;
    
    // Start is called before the first frame update
    private void Start()
    {
        if (!ReferenceEquals(player, null)) {
            var spawnTransform = transform;
            Instantiate(player, spawnTransform.position, spawnTransform.rotation);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
