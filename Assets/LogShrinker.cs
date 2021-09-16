using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogShrinker : MonoBehaviour
{
    private bool _shrink = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    private void OnCollisionStay(Collision other)
    {
        if (other.transform.parent.position == transform.parent.position)
        {
            transform.parent.position += new Vector3(0, 0, 3);
        }
        if (transform.parent.localScale.z != 0 )
        {
            print("IM SHRINKING");
            transform.parent.localScale += Vector3.back;
        }
    }

   
}
