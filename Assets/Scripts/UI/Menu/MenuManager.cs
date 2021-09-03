using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace FG {
    public sealed class MenuManager : UIManager<MenuManager> {
        [FG.Scene] 
        [SerializeField] private string _sceneName = string.Empty;
        
        private VisualElement _overlay;
        private Label _pausedText;
        private Button _resumeButton;
        private Button _playButton;
        private Button _quitButton;

        private bool IsFirstScene => SceneManager.GetActiveScene().buildIndex == 0;
        private bool IsActive => instance.gameObject.activeSelf;

        /// <summary>
        /// This is called when the player presses ESC with the new Unity Input System
        /// </summary>
        /// <param name="value"></param>
        public void OnMenuToggle(InputAction.CallbackContext value) {
            if (IsFirstScene || value.phase != InputActionPhase.Started) {
                return;
            }

            ToggleMenu();
        }

        /// <summary>
        /// Displaying/hiding menu, when the menu shows, pause the game
        /// </summary>
        private void ToggleMenu() {
            // Remove focus to remove warning about focusable
            rootElement.focusController?.focusedElement?.Blur();

            instance.gameObject.SetActive(!IsActive);
            Time.timeScale = IsActive ? 0 : 1;
        }

        /// <summary>
        /// When a player clicks on the "Play"-button, it changes scene
        /// </summary>
        private void StartGame() {
            if (_sceneName != string.Empty) {
                SceneManager.LoadScene(_sceneName);
                ToggleMenu();
            }
            else {
                Debug.LogWarning($"You need to select a scene to load when pressing the Play-button.");
            }
        }

        /// <summary>
        /// When player clicks on the "Quit"-button, the application quits
        /// </summary>
        private void QuitApplication() {
            Application.Quit();
        }

        /// <summary>
        /// Display elements for start menu, and hide everything else
        /// </summary>
        private void DisplayStartSceneMenu() {
            HideElement(_overlay);
            HideElement(_pausedText);
            HideElement(_resumeButton);
            ShowElement(_playButton);
        }

        /// <summary>
        /// Display elements for pause menu, and hide everything else
        /// </summary>
        private void DisplayPausedGameMenu() {
            ShowElement(_overlay);
            ShowElement(_pausedText);
            ShowElement(_resumeButton);
            HideElement(_playButton);
        }

        /// <summary>
        /// Showing correct button depending on the scene
        /// Start = Show Play
        /// Other = Show Resume
        /// </summary>
        private void DisplayCorrectElements() {
            if (IsFirstScene) {
                DisplayStartSceneMenu();
            }
            else {
                DisplayPausedGameMenu();
            }
        }

        /// <summary>
        /// Add click events on the buttons in the menu
        /// </summary>
        protected override void InitializeElements() {
            rootElement = document.rootVisualElement;

            _overlay = rootElement.Q<VisualElement>("overlay");
            _pausedText = rootElement.Q<Label>("pausedText");
            _resumeButton = rootElement.Q<Button>("resume");
            _playButton = rootElement.Q<Button>("play");
            _quitButton = rootElement.Q<Button>("quit");

            _resumeButton.RegisterCallback<ClickEvent>(ev => ToggleMenu());
            _playButton.RegisterCallback<ClickEvent>(ev => StartGame());
            _quitButton.RegisterCallback<ClickEvent>(ev => QuitApplication());

            DisplayCorrectElements();
        }

        /// <summary>
        /// Cleaning up the click events
        /// </summary>
        protected override void RemoveClickEvents() {
            if (!ReferenceEquals(_resumeButton, null)) {
                _resumeButton.UnregisterCallback<ClickEvent>(ev => ToggleMenu());
            }

            if (!ReferenceEquals(_playButton, null)) {
                _playButton.UnregisterCallback<ClickEvent>(ev => StartGame());
            }

            if (!ReferenceEquals(_quitButton, null)) {
                _quitButton.UnregisterCallback<ClickEvent>(ev => QuitApplication());
            }
        }
    }
}