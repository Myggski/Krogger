using UnityEngine;

namespace FG {
    public class InfiniteRotate : MonoBehaviour {
        [SerializeField]
        private float rotationSpeed = 200f;

        /// <summary>
        /// Rotating a gameobject forward locally
        /// </summary>
        private void Rotate() {
            transform.Rotate(transform.forward, rotationSpeed * Time.deltaTime, Space.World);
        }
    
        private void LateUpdate() {
            Rotate();
        }
    }
}