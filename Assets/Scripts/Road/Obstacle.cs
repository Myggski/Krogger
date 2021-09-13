using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5.0f;

    [SerializeField] 
    private GameObject explosion;

    private MeshRenderer _renderer;
    private Rigidbody _rb;
    
    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
        _rb.AddForce(transform.forward * _speed);
    }

    private void Update()
    {
        var xPos = transform.position.x;
        if (xPos >= 40 || xPos <= -40)
        {
            Destroy(gameObject);
        }
    }

    public void GoBoomNow()
    {
        StartCoroutine(GoBoom(0f));
    }
    
    private IEnumerator GoBoom(float inSeconds)
    {
        yield return new WaitForSeconds(inSeconds);
        HideSelfAndAllChildren();
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
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

    private IEnumerator DeleteMe(float inSeconds)
    {
        yield return new WaitForSeconds(inSeconds);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "car")
        {
            StartCoroutine(GoBoom(1));
        }
        // TODO: if player -> remove 1 score and disable input for X seconds
    }
}
