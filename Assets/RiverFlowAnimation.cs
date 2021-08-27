using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RiverFlowAnimation : MonoBehaviour
{
    [FormerlySerializedAs("scrollY")]
    [Header("Flow Speed")]
    [SerializeField] public float flowSpeed = 0.5f;

    private Material _material;
    
    void Start()
    {
        _material = GetComponent<Renderer>().material;
    }
    
    void Update()
    {
        float offsetY = Time.time * flowSpeed;

        _material.mainTextureOffset = new Vector2(0, offsetY);
    }
}
