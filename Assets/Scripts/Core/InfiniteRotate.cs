using UnityEngine;
using UnityEngine.Assertions.Must;

namespace FG {
    public enum RotationType {
        forward = 0,
        up = 1,
    }

    public class InfiniteRotate : MonoBehaviour {
        [SerializeField]
        private float rotationSpeed = 200f;
        [SerializeField]
        private RotationType rotationAxis;

        /// <summary>
        /// Rotating a gameobject forward locally
        /// </summary>
        private void Rotate() {
            transform.Rotate(GetRotationAxis(), rotationSpeed * Time.deltaTime, Space.World);
        }

        /// <summary>
        /// Get the rotation axis, how the gameObject should rotate
        /// </summary>
        /// <returns></returns>
        private Vector3 GetRotationAxis() {
            Vector3 rotation = transform.forward;

            switch (rotationAxis) {
                case RotationType.up:
                    rotation = transform.up;
                    break;
                case RotationType.forward:
                    rotation = transform.forward;
                    break;
            }

            return rotation;
        }
    
        private void LateUpdate() {
            Rotate();
        }
    }
}