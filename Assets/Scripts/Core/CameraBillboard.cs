using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class CameraBillboard : MonoBehaviour {
    private Camera _cam;
    public bool enableBillboard = false;

    // Start is called before the first frame update
    private void Start()
    {
        GrabMainCamera();
    }

    /// <summary>
    /// Assigns the main camera to the _cam field
    /// </summary>
    private void GrabMainCamera()
    {
        _cam = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        if (enableBillboard)
        {
            AlignToCameraView();
        }
    }

    private void AlignToCameraView()
    {
        transform.LookAt(transform.position + _cam.transform.rotation * Vector3.forward, _cam.transform.rotation * Vector3.up);
    }
}
