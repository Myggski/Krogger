using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Obstacle : MonoBehaviour
{
    [Header("Properties")] 
    [SerializeField]
    private float _speed = 5.0f;
    
    private Rigidbody _rb;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.AddForce(transform.forward * _speed);
    }
}
