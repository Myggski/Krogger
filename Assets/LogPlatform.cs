using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LogPlatform : MonoBehaviour
{
    [Tooltip("The max z-scale a log can spawn with on initialization")]
    [Range(1, 6)]
    [SerializeField]
    private int maxSize;
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
        int randomLength = Random.Range(1, maxSize + 1);
        transform.localScale = new Vector3(1, 1, randomLength);
    }

    // Start the sinking timer 
    private void OnTriggerEnter(Collider other)
    {
        // This collision / trigger means player is on a log object
        if (!other.CompareTag("Player") || isStoodUpon) return;
        // Set isStoodUpon to true, which triggers the shaking
        isStoodUpon = true;
        StartSinkCountdown(stayAfloatDuration);
    }

    // Stop the sinking timer when player leaves the log,
    // and stop shaking the log
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
        Destroy(this, 3f);
    }

    private void Update()
    {
        if (sink)
        {
            transform.position += Vector3.down * SINK_SPEED * Time.deltaTime;
        } else if (isStoodUpon)
        {
            float shake =  Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            transform.position = new Vector3(transform.position.x, shake,
                transform.position.z);
        }
    }
}
