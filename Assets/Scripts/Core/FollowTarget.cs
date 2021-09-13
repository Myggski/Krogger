using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    private Transform target;

    [SerializeField] 
    private float followSpeed = 2f;

    private Vector3 targetOffset;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(LateStart(1f));
        Invoke(nameof(LateStart), 1f);
    }

    void LateStart()
    {
        target = GameObject.FindWithTag("Player").transform;
        targetOffset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + targetOffset,
                followSpeed * Time.deltaTime);
        }}
}
