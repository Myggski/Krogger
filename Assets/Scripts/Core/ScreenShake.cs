using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FG {
    public class ScreenShake : MonoBehaviour
    {
        [SerializeField]
        private float shakeDuration = 0f;
        [SerializeField]
        private float shakeAmount = 0.7f;
        [SerializeField]
        private float decreaseFactor = 1.0f;
        
        private Camera _mainCamera;
        private Vector3 _originalCameraPosition;
        private Coroutine _shakeCoroutine;
        private bool _shaking;

        /// <summary>
        /// Should be called with UnityEvent
        /// Starts the shake coroutine
        /// </summary>
        public void StartShake() {
            _originalCameraPosition = _mainCamera.transform.localPosition;

            if (!ReferenceEquals(_shakeCoroutine, null)) {
                StopCoroutine(_shakeCoroutine);    
            }
            
            _shakeCoroutine = StartCoroutine(LateShake());
        }

        /// <summary>
        /// Setup the _maincamera variable
        /// </summary>
        private void Setup() {
            _mainCamera = Camera.main;
        }

        /// <summary>
        /// Shakes the screen / change position of camera at the end of frame
        /// </summary>
        /// <returns></returns>
        private IEnumerator LateShake() {
            float duration = shakeDuration;
            _shaking = true;

            while (_shaking) {
                yield return new WaitForEndOfFrame();

                if (duration > 0) {
                    _mainCamera.transform.localPosition = _mainCamera.transform.localPosition + Random.insideUnitSphere * shakeAmount;
                    duration -= Time.deltaTime * decreaseFactor;
                } else {
                    _mainCamera.transform.localPosition = _originalCameraPosition;
                    _shaking = false;
                }
            }
        }

        private void Awake() {
            Setup();
        }
    }
}
