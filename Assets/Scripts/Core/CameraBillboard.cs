using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class CameraBillboard : MonoBehaviour
{
    private Camera _cam;

    public bool enableBillboard = false; 
    
    
    // Start is called before the first frame update
    void Start()
    {
        _cam = Camera.main;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (enableBillboard)
        {
            transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward, _cam.transform.rotation * Vector3.up);
        }
    }
}
