using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
    [SerializeField]
    private float followSpeed = 0f;

    private Transform _target;
    private Vector3 _targetOffset;

    // Start is called before the first frame update
    private void Awake() {
        StartCoroutine(Setup());
    }

    private IEnumerator Setup() {

        while (ReferenceEquals(GameObject.FindWithTag("Player"), null)) {
            yield return new WaitForEndOfFrame();
        }
        
        _target = GameObject.FindWithTag("Player").transform;
        _targetOffset = transform.position - _target.position;
    }

    // Update is called once per frame
    private void Update() {
        if (_target && _target.position.y >= 0) {
            transform.position = Vector3.Lerp(transform.position, _target.position + _targetOffset,
                followSpeed * Time.deltaTime);
        }
    }
}
