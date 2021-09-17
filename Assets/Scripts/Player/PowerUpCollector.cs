using System;
using UnityEngine;

namespace FG {
    [RequireComponent(typeof(PlayerController), typeof(PlayerStun))]
    public class PowerUpCollector : MonoBehaviour {
        private PlayerController _playerController;
        private PlayerStun _playerStun;

        /// <summary>
        /// Collecting powerup and set the needed values
        /// Is being called whenever PowerUpCollector collides with a PowerUp
        /// </summary>
        /// <param name="movementSpeed"></param>
        /// <param name="duration"></param>
        public void CollectPowerUp(float movementSpeed, float duration) {
            _playerController.SpeedUp(movementSpeed, duration);
            _playerStun.Deactivate(duration);
        }

        /// <summary>
        /// Setup the components
        /// </summary>
        private void Setup() {
            _playerController = GetComponent<PlayerController>();
            _playerStun = GetComponent<PlayerStun>();
        }

        private void Awake() {
            Setup();
        }
    }
}