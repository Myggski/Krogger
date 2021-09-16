using System.Collections;
using FG;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingObstacle : MonoBehaviour {
    [SerializeField] 
    private GameObject explosion;

    [SerializeField]
    private float stunTime = 0.5f;
    [SerializeField]
    private float invulnerableTime = 0.2f;

    private float _speed = 5.0f;
    private float _trackWidth = 60f;
    private bool _hasAlreadyGivenPoints = false;
    private MeshRenderer _renderer;
    private Rigidbody _rb;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        _rb.AddForce(transform.forward * _speed);
    }

    private void Update()
    {
        var xPos = transform.position.x;
        if (xPos >= (_trackWidth/2) + 10 ||
            xPos <= -(_trackWidth/2) - 10)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(float speed, float trackWidth) {
        _speed = speed;
        _trackWidth = trackWidth;
    }

    public void GoBoomNow()
    {
        HideSelfAndAllChildren();
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Explodes the car in inSeconds seconds
    /// Disables meshrenderers in all children and self
    /// and instantiates an explosion prefab
    /// </summary>
    /// <param name="inSeconds"></param>
    /// <returns></returns>
    private IEnumerator GoBoom(float inSeconds)
    {
        yield return new WaitForSeconds(inSeconds);
        GoBoomNow();
    }

    private void HideSelfAndAllChildren()
    {
        _renderer.enabled = false;
        var childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var childRender in childRenderers)
        {
            childRender.enabled = false;
        }
    }

    /// <summary>
    /// When something collides with the moving obstacle that is stunnable
    /// The obstacle should be pushed it opposite direction of the sunnable game object
    /// </summary>
    /// <param name="stunnableGameObject">For example player</param>
    private void PushMovingObstacleOnCollision(Collision stunnableGameObject) {
        Vector3 otherPosition = new Vector3(stunnableGameObject.transform.position.x, 0, stunnableGameObject.transform.position.z);
        Vector3 obstaclePosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 collisionDirection = (otherPosition - obstaclePosition).normalized;
        
        _rb.angularVelocity = Vector3.zero;
        _rb.AddForce(-collisionDirection * 1000);
    }

    private void OnCollisionEnter(Collision other)
    {
        IStunnable stunnableObject = other.gameObject.GetComponent<IStunnable>();
        
        if (!ReferenceEquals(stunnableObject, null)) {
            stunnableObject.Stun(stunTime, invulnerableTime);
            PushMovingObstacleOnCollision(other);
            StartCoroutine(GoBoom(0.6f));
            _hasAlreadyGivenPoints = true;
        }
        
        if (other.gameObject.CompareTag("car"))
        {
            if (!_hasAlreadyGivenPoints) {
                _hasAlreadyGivenPoints = true;
                ScoreManager.Instance.AddScore(1);
            }
            
            StartCoroutine(GoBoom(0.6f));
        }
    }
}
