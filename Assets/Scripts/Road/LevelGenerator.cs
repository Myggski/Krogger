using System;
using System.Collections;
using System.Collections.Generic;
using FG;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    private WeightedTrackPiece[] weightedTracks;

    [SerializeField]
    private int maxTrackPieces = 20;

    [SerializeField] 
    private int startingAmountTracks;

    [SerializeField] 
    private bool continuousSpawn = true;

    private Queue<GameObject> _trackQueue = new Queue<GameObject>();

    private Vector3 _spawnPosition = new Vector3(0, 0, 0);

    private int _sumWeights;

    private GameObject _lastTrackPiece;
    private GameObject _currentTrackPiece;
    
    private void Awake()
    {
        SetupSafeStart();
        
        print("forward: " + transform.forward);
        
        foreach (var track in weightedTracks)
        {
            _sumWeights += track.Weight;
        }

        // Initiate with a chunk of the level already done
        for (int i = 0; i < startingAmountTracks; i++)
        {
            WeightedSpawnNextTrackPiece();
        }
        
        if (continuousSpawn)
        {
            StartCoroutine(SpawnTracks(2f));
        }
    }

    // Initializes _LastTrackPiece and sets up a safe first row
    // _lastTrackPiece is needed for the method SpawnNextTrackPiece()
    private void SetupSafeStart()
    {
        _lastTrackPiece = weightedTracks[0].TrackPrefab;
        _currentTrackPiece = Instantiate(_lastTrackPiece, _spawnPosition, transform.rotation * Quaternion.Euler(0, 90, 0));
        _trackQueue.Enqueue(_currentTrackPiece);
        // Spawn one additional rows of safe grass
        SpawnNextTrackPiece(0);
    }

    // Remove this? Used to infinitely spawn tracks every <frequency> seconds
    // Could be useful later. 
    private IEnumerator SpawnTracks(float frequency)
    {
        while (continuousSpawn)
        {
            yield return new WaitForSeconds(frequency);
            WeightedSpawnNextTrackPiece();
        }
    }
    
    /// <summary>
    /// Spawns the next track piece, and updates the position for the next piece.
    /// </summary>
    public void SpawnNextTrackPiece()
    {
        int trackIndex = Random.Range(0, weightedTracks.Length);
        _currentTrackPiece = weightedTracks[trackIndex].TrackPrefab;
        _spawnPosition.x += (_lastTrackPiece.transform.localScale.x / 2 ) + (_currentTrackPiece.transform.localScale.x / 2);
        
        _trackQueue.Enqueue(Instantiate(_currentTrackPiece, _spawnPosition, transform.rotation));
        
        if (_trackQueue.Count > maxTrackPieces)
        {
            Destroy(_trackQueue.Dequeue());
        }

        _lastTrackPiece = _currentTrackPiece;
    }

    public void WeightedSpawnNextTrackPiece()
    {
        // Make sure the array isn't empty
        if (weightedTracks.Length == 0) throw new Exception("WeightedTrackPiece array is empty! Go scream at the devs");

        int roll = Random.Range(0, _sumWeights);
        int currentIndex = 0;

        foreach (var trackPiece in weightedTracks)
        {
            currentIndex += trackPiece.Weight;
            if (roll < currentIndex)
            {
                SpawnNextTrackPiece(trackPiece.TrackPrefab);
                return;
            }
        }

        throw new Exception("No track could be picked. Go scream at the devs!");

    }

    private void SpawnNextTrackPiece(GameObject trackPiecePrefab)
    {
        _currentTrackPiece = trackPiecePrefab;
        //_spawnPosition.x += (_lastTrackPiece.transform.localScale.x / 2 ) + (_currentTrackPiece.transform.localScale.x / 2);
        _spawnPosition += transform.forward * ((_lastTrackPiece.transform.localScale.x / 2 ) + (_currentTrackPiece.transform.localScale.x / 2));

        _trackQueue.Enqueue(Instantiate(_currentTrackPiece, _spawnPosition, transform.rotation * Quaternion.Euler(0, 90, 0)));
        
        if (_trackQueue.Count > maxTrackPieces)
        {
            Destroy(_trackQueue.Dequeue());
        }

        _lastTrackPiece = trackPiecePrefab;
    }

    public void SpawnNextTrackPiece(int trackIndex)
    {
        _currentTrackPiece = weightedTracks[trackIndex].TrackPrefab;
        //_spawnPosition.x += (_lastTrackPiece.transform.localScale.x / 2 ) + (_currentTrackPiece.transform.localScale.x / 2);
        _spawnPosition += transform.forward * ((_lastTrackPiece.transform.localScale.x / 2 ) + (_currentTrackPiece.transform.localScale.x / 2));

        
        _trackQueue.Enqueue(Instantiate(_currentTrackPiece, _spawnPosition, transform.rotation * Quaternion.Euler(0, 90, 0)));
        
        if (_trackQueue.Count > maxTrackPieces)
        {
            Destroy(_trackQueue.Dequeue());
        }

        _lastTrackPiece = _currentTrackPiece;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 20;
        Gizmos.DrawRay(transform.position, direction);
        
        
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(transform.position, new Vector3(60, 1, 3));
    }
}
