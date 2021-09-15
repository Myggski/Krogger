using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingObstacle : MonoBehaviour {
    private float _speed = 5.0f;
    private float _trackWidth = 60f;

    [SerializeField] 
    private GameObject explosion;

    private MeshRenderer _renderer;
    private Rigidbody _rb;
    
    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        _rb.AddForce(transform.forward * _speed);
    }

    private void Update()
    {
        var xPos = transform.position.x;
        if (xPos >= (_trackWidth/2) + 10 ||
            xPos <= -(_trackWidth/2) - 10)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(float speed, float trackWidth) {
        _speed = speed;
        _trackWidth = trackWidth;
    }

    public void GoBoomNow()
    {
        HideSelfAndAllChildren();
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Explodes the car in inSeconds seconds
    /// Disables meshrenderers in all children and self
    /// and instantiates an explosion prefab
    /// </summary>
    /// <param name="inSeconds"></param>
    /// <returns></returns>
    private IEnumerator GoBoom(float inSeconds)
    {
        yield return new WaitForSeconds(inSeconds);
        GoBoomNow();
    }

    private void HideSelfAndAllChildren()
    {
        _renderer.enabled = false;
        var childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var childRender in childRenderers)
        {
            childRender.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // TODO: if player -> remove 1 score and disable input for X seconds
        // TODO: add scoring when car go boom
        if (other.gameObject.CompareTag("car"))
        {
            StartCoroutine(GoBoom(1));
        }
        
    }
}
