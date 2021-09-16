using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogShuffler : MonoBehaviour
{
    public bool shuffle;

    private Collider[] results = new Collider[20];

    private LayerMask obstLayer = (1 << 6);
    public int DIR = 1;
    private const int LOG_UNIT_SIZE = 3;
    private const int TRACK_SIZE = 30;

    // Update is called once per frame
    void Update()
    {
         if (shuffle)
         {
             transform.position += transform.forward * LOG_UNIT_SIZE * DIR;
             shuffle = false;
         }
         
         CheckOverlap();

         if (transform.position.x > TRACK_SIZE - LOG_UNIT_SIZE / 2)
         {
             DIR = -1;
             shuffle = true;
         } else if (transform.position.x < -(TRACK_SIZE - LOG_UNIT_SIZE / 2))
         {
             DIR = 1;
             shuffle = true;
         }
    }

    private void CheckOverlap()
    {
        results = new Collider[results.Length];
        int hits = Physics.OverlapSphereNonAlloc(transform.position, 1f, results, obstLayer);

        
        for (int i = 0; i < hits; i++)
        {
            if (i > 1)
            {
                if (results[i].transform.root.position.x >= (TRACK_SIZE - LOG_UNIT_SIZE / 2) 
                    || results[i].transform.root.position.x <= -(TRACK_SIZE + LOG_UNIT_SIZE / 2)) DIR *= -1;
                results[i].transform.root.position += transform.forward * LOG_UNIT_SIZE * DIR;
            }
        }

        
    }
}
