using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FG;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : ManagerBase<LevelGenerator> {
    [Header("Track settings")]
    [SerializeField]
    private WeightedTrackPiece[] weightedTracks;
    [SerializeField]
    private GameObject powerUpPrefab;
    [SerializeField]
    private int maxTrackPieces = 23;
    [SerializeField] 
    private int startingAmountTracks = 20;
    [SerializeField] 
    private int numberOfSafeTracks = 10;
    [SerializeField]
    [Tooltip("How close a gameObject (player) has to be to the last piece")]
    private int minSpawnDistance = 30;
    [Space(16)]
    [SerializeField]
    [Tooltip("How much time it takes for a new track to spawn")]
    private float timeBetweenTrackSpawns = 3f;
    [SerializeField]
    [Tooltip("How much time to remove whenever the track speeds up")]
    private float timeBetweenTrackSpeedIncrement = 0.075f;
    [SerializeField]
    [Tooltip("Minimal time that the spawning time can get to")]
    private float minTrackSpeedIncrement = 0.4f;
    [Space(16)]
    [SerializeField]
    private float shakeSpeed = 1.0f;
    [SerializeField]
    private float shakeAmount  = 1.0f;

    private int _sumWeights;
    private Vector3 _spawnPosition = new Vector3(0, 0, 0);
    private GameObject _lastTrackPiece;
    private GameObject _currentTrackPiece;
    private GameObject _shakeTrackPiece;
    private GameObject _sinkingTrackPiece;
    private Coroutine _spawnTrackCoroutine;
    
    private readonly LinkedList<GameObject> _trackQueue = new LinkedList<GameObject>();
    private const float SINK_SPEED = 0.1f;

    private void InitializeLevel() {
        SetupSafeStart();
        
        // Setting up the weights of the tracks 
        foreach (WeightedTrackPiece track in weightedTracks) {
            _sumWeights += track.Weight;
        }

        // Initiate with a chunk of the level already done
        for (int i = 0; i < startingAmountTracks; i++) {
            WeightedSpawnNextTrackPiece();
        }

        StartSpawningTracks();
    }

    /// <summary>
    /// Trying to spawn a track depending on the distance between gameObjectPosition and the last track position
    /// </summary>
    /// <param name="gameObjectPosition">Some sort of position of a gameObject, example the player</param>
    public void TrySpawnTrack(Vector3 gameObjectPosition) {
        if (!_trackQueue.Any()) {
            Debug.LogWarning("No track could be found.");
            return;
        } 

        // We only care about the distance between the z-positions
        Vector3 checkPosition = new Vector3(0, 0, gameObjectPosition.z);
        Vector3 trackPosition = new Vector3(0, 0, _trackQueue.Last().transform.position.z);

        if (Vector3.Distance(checkPosition, trackPosition) < minSpawnDistance) {
            StartSpawningTracks();
        }
    }

    /// <summary>
    /// Speeds up the time between track spawns
    /// </summary>
    public void SpeedUpTrackSpawn() {
        timeBetweenTrackSpawns -= timeBetweenTrackSpeedIncrement;
        timeBetweenTrackSpawns = Mathf.Clamp(timeBetweenTrackSpawns, minTrackSpeedIncrement, timeBetweenTrackSpawns);
    }

    /// <summary>
    /// Starts coroutine that spawns tracks after a certain amount of time
    /// </summary>
    private void StartSpawningTracks() {
        if (!ReferenceEquals(_spawnTrackCoroutine, null)) {
            StopCoroutine(_spawnTrackCoroutine);
        }

        _spawnTrackCoroutine = StartCoroutine(SpawnTracks());
    }

    // Initializes _lastTrackPiece and sets up a safe first row
    // _lastTrackPiece is needed for the method SpawnNextTrackPiece()
    private void SetupSafeStart() {
        SetupSpawningTrack();
        SetupSafeTracks();
    }

    /// <summary>
    /// Makes the first track safe by removing all the obstacles
    /// </summary>
    private void SetupSpawningTrack() {
        _lastTrackPiece = weightedTracks[0].TrackPrefab;
        _currentTrackPiece = Instantiate(_lastTrackPiece, _spawnPosition, transform.rotation * Quaternion.Euler(0, 90, 0));
        _trackQueue.AddLast(_currentTrackPiece);
        _shakeTrackPiece = _trackQueue.First();
        
        // Removes component that adds obstacles on the safe spawn point
        ObstacleSpawner obstacleSpawner = _shakeTrackPiece.GetComponent<ObstacleSpawner>();
        
        if (!ReferenceEquals(obstacleSpawner, null)) {
            Destroy(obstacleSpawner);    
        }
    }

    /// <summary>
    /// Setting up a few tracks after the first track that is safer than the upcoming tracks
    /// </summary>
    private void SetupSafeTracks() {
        for (int i = 0; i < numberOfSafeTracks; i++) {
            GameObject trackPiece = SpawnNextTrackPiece(0);
            ObstacleSpawner obstacleSpawner = trackPiece.GetComponent<ObstacleSpawner>();
            
            if (!ReferenceEquals(obstacleSpawner, null)) {
                obstacleSpawner.InitializeSafeMode();    
            }
        }
    }

    // Remove this? Used to infinitely spawn tracks every <frequency> seconds
    // Could be useful later. 
    private IEnumerator SpawnTracks() {
        while (true) {
            WeightedSpawnNextTrackPiece();
            yield return new WaitForSeconds(timeBetweenTrackSpawns);
        }
    }

    private void WeightedSpawnNextTrackPiece()
    {
        // Gives warning if array is empty
        if (weightedTracks.Length == 0) {
            Debug.Log("WeightedTrackPiece array is empty! Go scream at the devs");
        }

        int roll = Random.Range(0, _sumWeights);
        int currentIndex = 0;

        foreach (var trackPiece in weightedTracks) {
            currentIndex += trackPiece.Weight;

            if (roll < currentIndex) {
                SpawnNextTrackPiece(trackPiece.TrackPrefab);
                return;
            }
        }
    }

    /// <summary>
    /// Spawns a trackpiece using the supplied prefab
    /// </summary>
    /// <param name="trackPiecePrefab">Roadtrack prefab to be spawned</param>
    private GameObject SpawnNextTrackPiece(GameObject trackPiecePrefab)
    {
        _currentTrackPiece = trackPiecePrefab;
        return SpawnCurrentPiece();
    }

    /// <summary>
    /// Spawns a trackpiece using the supplied track index
    /// </summary>
    /// <param name="trackIndex"></param>
    private GameObject SpawnNextTrackPiece(int trackIndex)
    {
        _currentTrackPiece = weightedTracks[trackIndex].TrackPrefab;
        return SpawnCurrentPiece();
    }

    private GameObject SpawnCurrentPiece()
    {
        _spawnPosition += transform.forward *
                          ((_lastTrackPiece.transform.localScale.x / 2) + (_currentTrackPiece.transform.localScale.x / 2));

        _trackQueue.AddLast(
            Instantiate(_currentTrackPiece, _spawnPosition, transform.rotation * Quaternion.Euler(0, 90, 0)));

        if (_trackQueue.Count > maxTrackPieces) {
            var poppedPiece = _trackQueue.First();
            _trackQueue.RemoveFirst();
            _sinkingTrackPiece = poppedPiece;
            DisableTrackPieceSpawn(poppedPiece);
            // Sinking piece animation, gets destroyed after 2 seconds
            StartCoroutine(DelayedDestroyTrackPiece(poppedPiece, 2f));
            _shakeTrackPiece = _trackQueue.First();
        }

        _lastTrackPiece = _currentTrackPiece;
        return _trackQueue.Last();
    }

    private IEnumerator DelayedDestroyTrackPiece(GameObject trackPiece, float inSeconds)
    {
        yield return new WaitForSeconds(inSeconds);
        
        Destroy(trackPiece);
        // Set _sinkingTrackPiece to null to avoid accessing it later on in update
        _sinkingTrackPiece = null;
    }

    /// <summary>
    /// Disables trackpiece and booms all cars spawned by it
    /// </summary>
    /// <param name="trackPiece"></param>
    private void DisableTrackPieceSpawn(GameObject trackPiece)
    {
        var roadTrackComponent = trackPiece.GetComponents<RoadTrack>();

        // check if it's a roadtrack with the component that spawns cars and not
        // just a grass track (which doesnt have the roadtrack component)
        if (roadTrackComponent.Length != 0) {
            var road = roadTrackComponent[0];
            road.DisableSpawnPoints();
            road.BoomAllCars();
        }
    }

    /// <summary>
    /// Adds shake animation on the first piece on the track
    /// </summary>
    private void ShakeAnimation() {
        if (!ReferenceEquals(_shakeTrackPiece, null)) {
            Vector3 shakeTrackPosition = _shakeTrackPiece.transform.position;
            float shake =  Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;

            _shakeTrackPiece.transform.position = 
                new Vector3(shakeTrackPosition.x, shake, shakeTrackPosition.z);
        }

        if (!ReferenceEquals(_sinkingTrackPiece, null)) {
            _sinkingTrackPiece.transform.position += Vector3.down * SINK_SPEED;
        }
    }

    private void Update() {
        ShakeAnimation();
    }
    
    protected override void Awake() {
        base.Awake();

        InitializeLevel();
    }
    
    // Gizmos for helping with how this object is facing
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 20;
        Gizmos.DrawRay(transform.position, direction);
        
        // TODO: fix magic numbers
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(transform.position, new Vector3(60, 1, 3));
    }
}
