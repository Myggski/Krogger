using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] 
    private GameObject player;

    [SerializeField] 
    private Transform prefabReference;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (!ReferenceEquals(player, null))
        {
            Instantiate(player, transform.position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
