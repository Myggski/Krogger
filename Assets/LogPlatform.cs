using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LogPlatform : MonoBehaviour
{
    [Tooltip("The minimum z-scale a log can spawn with on initialization")]
    [Range(0.2f, 1f)]
    [SerializeField]
    private float minimumSize;
    [SerializeField] 
    private float shakeSpeed;
    [SerializeField] 
    private float shakeAmount;

    private bool isStoodUpon = false;
    private bool sink = false;
    private float stayAfloatDuration = 5f;
    private float SINK_SPEED = 0.1f;


    // Start is called before the first frame update
    private void Start()
    {
        float randomLength = Random.Range(minimumSize, 1f);
        transform.localScale = new Vector3(1, 1, randomLength);
    }

    private void OnTriggerEnter(Collider other)
    {
        // This collision / trigger means player is on a log object
        if (other.CompareTag("Player"))
        {
            // Set isStoodUpon to true, which triggers the shaking
            isStoodUpon = true;
            StartSinkCountdown(stayAfloatDuration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isStoodUpon = false;
        StopAllCoroutines();
    }

    /// <summary>
    /// Starts a countdown, which upon reaching 0 sinks the log
    /// </summary>
    /// <param name="afloatDuration">How long before the log sinks</param>
    private void StartSinkCountdown(float afloatDuration)
    {
        StartCoroutine(SinkingTimer(afloatDuration));
    }

    private IEnumerator SinkingTimer(float inSeconds)
    {
        // First wait inSeconds
        yield return new WaitForSeconds(inSeconds);
        // Then set sink flag
        sink = true;
        Destroy(this, 5f);
    }

    private void Update()
    {
        if (isStoodUpon)
        {
            var shake =  Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            transform.position = new Vector3(transform.position.x, shake,
                transform.position.z);
        }

        if (sink)
        {
            transform.position += Vector3.down * SINK_SPEED;
        }
    }
}
