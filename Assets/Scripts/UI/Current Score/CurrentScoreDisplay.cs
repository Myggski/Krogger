using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace FG.CurrentScore {
    [RequireComponent(typeof(UIDocument))]
    public class CurrentScoreDisplay : MonoBehaviour {
        [SerializeField]
        private IntVariable currentScore;

        [Header("Animation")]
        [SerializeField]
        [Tooltip("Number of seconds for the animation to run")]
        private float animationTime = 1f;
        [SerializeField]
        [Tooltip("Default number of pixels, because the UIToolkit returns null otherwise when you ask for style")]
        private float fontSize = 128;
        [SerializeField]
        [Tooltip("Min size in pixels for the animation")]
        private float fontMinSize = 80;
        [SerializeField]
        [Tooltip("Max size in pixels for the animation")]
        private float fontMaxSize = 300;

        private UIDocument _document;
        private Label _scoreText;
        private Coroutine _animationCoroutine;

        /// <summary>
        /// Updates the score-text in the UI and then animating it
        /// This method will be called by an UnityEvent.
        /// </summary>
        public void UpdateScoreText() {
            SetScore();

            if (!ReferenceEquals(_animationCoroutine, null)) {
                StopCoroutine(_animationCoroutine);
            }
            
            _animationCoroutine = StartCoroutine(AnimateTextSize());
        }

        /// <summary>
        /// Updates the score-text in the UI
        /// </summary>
        private void SetScore() {
            _scoreText.text = $"{currentScore.Value}";
        }

        /// <summary>
        /// Linear animation, making the text bigger, then to smaller, and then "bounce" back to regular size
        /// TODO: Make this less complex
        /// </summary>
        /// <returns></returns>
        private IEnumerator AnimateTextSize() {
            float elapsedTime = 0f;
            float oneFourth = animationTime / 4;
            
            _scoreText.style.fontSize = fontSize;

            while (elapsedTime < animationTime) {
                if (elapsedTime <= oneFourth) {
                    _scoreText.style.fontSize = Mathf.Lerp(fontSize, fontMaxSize, elapsedTime / oneFourth);
                } else if (elapsedTime > oneFourth && elapsedTime <= oneFourth * 2f) {
                    _scoreText.style.fontSize = Mathf.Lerp(fontMaxSize, fontSize, ((elapsedTime - oneFourth) / oneFourth));
                }
                else if(elapsedTime > oneFourth * 2f && elapsedTime < oneFourth * 3f) {
                    _scoreText.style.fontSize = Mathf.Lerp(fontSize, fontMinSize, (elapsedTime - (oneFourth * 2f)) / oneFourth);
                } else if (elapsedTime > oneFourth * 3f) {
                    _scoreText.style.fontSize = Mathf.Lerp(fontMinSize, fontSize, (elapsedTime - (oneFourth * 3f)) / oneFourth);
                }
                
                yield return new WaitForFixedUpdate();
                elapsedTime += Time.fixedDeltaTime;
            }
            
            _scoreText.style.fontSize = fontSize;
        }

        /// <summary>
        /// Get the UIComponent that contains the score text
        /// </summary>
        private void Setup() {
            _document = GetComponent<UIDocument>();
        }

        /// <summary>
        /// Get the text-label for score
        /// </summary>
        private void InitializeElement() {
            _scoreText = _document.rootVisualElement.Q<Label>("score-text");
            SetScore();
        }

        private void Awake() {
            Setup();
        }

        private void OnEnable() {
            InitializeElement();
        }
    }
}