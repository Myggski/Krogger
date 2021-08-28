using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] trackPieces;

    private Vector3 spawnPosition = new Vector3(0, 0, 0);

    private GameObject lastTrackPiece;


    void Start()
    {
        lastTrackPiece = trackPieces[0];
        Instantiate(lastTrackPiece, spawnPosition, Quaternion.identity);
        spawnPosition.x += lastTrackPiece.transform.localScale.x / 2;

        StartCoroutine(SpawnTracks(2f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnTracks(float frequency)
    {
        while (true)
        {
            yield return new WaitForSeconds(frequency);
            SpawnNextTrackPiece();
        }

    }

    void SpawnNextTrackPiece()
    {
        print("spawn pos: " + spawnPosition);
        print("localScale.x: " + lastTrackPiece.transform.localScale.x);
        int trackIndex = Random.Range(0, trackPieces.Length);
        lastTrackPiece = trackPieces[trackIndex];
        Instantiate(lastTrackPiece, spawnPosition, Quaternion.identity);
        spawnPosition.x += lastTrackPiece.transform.localScale.x / 2;
        
    }
}
