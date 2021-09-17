using FG;
using UnityEngine;

public class PowerUp : MonoBehaviour {
    [SerializeField]
    private int scoreToAdd = 5;
    [SerializeField]
    private float movementSpeed = 20f;
    [SerializeField]
    private float duration = 5f;

    /// <summary>
    /// If power up collides with player, it triggers an event and adds score
    /// </summary>
    /// <param name="other">A game object, could be a player</param>
    private void CheckPlayerCollision(Collider other) {
        PowerUpCollector collector = other.GetComponent<PowerUpCollector>();
        if (!ReferenceEquals(collector, null)) {
            ScoreManager.Instance.AddScore(scoreToAdd);
            collector.CollectPowerUp(movementSpeed, duration);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        CheckPlayerCollision(other);
    }
}
