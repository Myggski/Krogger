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

    private void Update()
    {
        var zPos = transform.position.z;
        if (zPos >= 40 || zPos <= -40)
        {
            Destroy(gameObject);
        }
    }
}
