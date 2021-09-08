using System;
using System.Collections;
using System.Collections.Generic;
using FG;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] 
    private GameObject[] trackPieces;

    
    public WeightedTrackPiece[] weightedTracks;

    [SerializeField]
    private int maxTrackPieces = 20;

    private Queue<GameObject> _trackQueue = new Queue<GameObject>();

    private Vector3 _spawnPosition = new Vector3(0, 0, 0);

    private GameObject _lastTrackPiece;
    private GameObject _currentTrackPiece;
    
    private void Awake()
    {
        SetupSafeStart();
        
        // Initiate with a chunk of the level already done
        for (int i = 0; i < 20; i++)
        {
            SpawnNextTrackPiece();
        }
 
        StartCoroutine(SpawnTracks(2f));
    }

    // Initializes _LastTrackPiece and sets up a safe first row
    // _lastTrackPiece is needed for the method SpawnNextTrackPiece()
    private void SetupSafeStart()
    {
        _lastTrackPiece = trackPieces[0];
        _currentTrackPiece = Instantiate(_lastTrackPiece, _spawnPosition, Quaternion.identity);
        _trackQueue.Enqueue(_currentTrackPiece);
        // Spawn two additional rows of safe grass
        SpawnNextTrackPiece(0);
        SpawnNextTrackPiece(0);
    }

    // Remove this? Used to infinitely spawn tracks every <frequency> seconds
    // Could be useful later. 
    private IEnumerator SpawnTracks(float frequency)
    {
        while (true)
        {
            yield return new WaitForSeconds(frequency);
            SpawnNextTrackPiece();
        }

    }
    
    /// <summary>
    /// Spawns the next track piece, and updates the position for the next piece.
    /// </summary>
    public void SpawnNextTrackPiece()
    {
        int trackIndex = Random.Range(0, trackPieces.Length);
        _currentTrackPiece = trackPieces[trackIndex];
        _spawnPosition.x += (_lastTrackPiece.transform.localScale.x / 2 ) + (_currentTrackPiece.transform.localScale.x / 2);
        
        _trackQueue.Enqueue(Instantiate(_currentTrackPiece, _spawnPosition, Quaternion.identity));
        
        if (_trackQueue.Count > maxTrackPieces)
        {
            Destroy(_trackQueue.Dequeue());
        }

        _lastTrackPiece = _currentTrackPiece;
    }

    public void SpawnNextTrackPiece(int trackIndex)
    {
        _currentTrackPiece = trackPieces[trackIndex];
        _spawnPosition.x += (_lastTrackPiece.transform.localScale.x / 2 ) + (_currentTrackPiece.transform.localScale.x / 2);
        
        _trackQueue.Enqueue(Instantiate(_currentTrackPiece, _spawnPosition, Quaternion.identity));
        
        if (_trackQueue.Count > maxTrackPieces)
        {
            Destroy(_trackQueue.Dequeue());
        }

        _lastTrackPiece = _currentTrackPiece;
    }
}
