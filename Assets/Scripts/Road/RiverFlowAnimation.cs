using UnityEngine;

public class RiverFlowAnimation : MonoBehaviour
{
    [Header("Flow Speed")]
    public float flowSpeed = 0.5f;

    private Material _material;
    
    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
    }
    
    private void Update()
    {
        float offsetY = Time.time * flowSpeed;

        _material.mainTextureOffset = new Vector2(0, offsetY);
    }
}
