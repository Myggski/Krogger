using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
    [SerializeField]
    private float followSpeed = 2f;

    private Transform _target;
    private Vector3 _targetOffset;

    // Start is called before the first frame update
    private void Start() {
        Invoke(nameof(LateStart), 1f);
    }

    private void LateStart() {
        _target = GameObject.FindWithTag("Player").transform;
        _targetOffset = transform.position - _target.position;
    }

    // Update is called once per frame
    private void Update() {
        if (_target) {
            transform.position = Vector3.Lerp(transform.position, _target.position + _targetOffset,
                followSpeed * Time.deltaTime);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -6f, 6f), transform.position.y,
                transform.position.z);
        }
    }
}
