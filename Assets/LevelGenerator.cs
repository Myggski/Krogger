using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] trackPieces;

    private Vector3 _spawnPosition = new Vector3(0, 0, 0);

    private GameObject _lastTrackPiece;
    private GameObject _currentTrackPiece;


    private void Start()
    {
        _lastTrackPiece = trackPieces[0];
        Instantiate(_lastTrackPiece, _spawnPosition, Quaternion.identity);

        for (int i = 0; i < 20; i++)
        {
            SpawnNextTrackPiece();
        }
 
        //StartCoroutine(SpawnTracks(2f));
    }

    private IEnumerator SpawnTracks(float frequency)
    {
        while (true)
        {
            yield return new WaitForSeconds(frequency);
            SpawnNextTrackPiece();
        }

    }

    private void SpawnNextTrackPiece()
    {
        int trackIndex = Random.Range(0, trackPieces.Length);
        _currentTrackPiece = trackPieces[trackIndex];
        _spawnPosition.x += (_lastTrackPiece.transform.localScale.x / 2 ) + (_currentTrackPiece.transform.localScale.x / 2);
        Instantiate(_currentTrackPiece, _spawnPosition, Quaternion.identity);

        _lastTrackPiece = _currentTrackPiece;
    }
}
