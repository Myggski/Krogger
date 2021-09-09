using Core.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace FG {
    public sealed class MenuManager : UIManagerBase<MenuManager> {
        // Scenes
        [Scene] 
        [SerializeField] 
        private string gameScene = string.Empty;

        [Scene] 
        [SerializeField] 
        private string scoreboardScene = string.Empty;

        // Event
        [SerializeField]
        private UnityEvent onGameStart = new UnityEvent();
        
        // UI
        private VisualElement _overlay;
        private Label _pausedText;
        private Button _resumeButton;
        private Button _playButton;
        private Button _scoreboard;
        private Button _quitButton;

        // Helpers
        private bool IsInStartScene => SceneManager.GetActiveScene().buildIndex == 0;
        private bool IsActive => !ReferenceEquals(_instance, null) && _instance.gameObject.activeSelf;

        /// <summary>
        /// This is called when the player presses ESC with the new Unity Input System
        /// </summary>
        /// <param name="value"></param>
        public void OnMenuToggle(InputAction.CallbackContext value) {
            if (IsInStartScene || value.phase != InputActionPhase.Started) {
                return;
            }

            ToggleMenu();
        }

        /// <summary>
        /// Displaying/hiding menu, when the menu shows, pause the game
        /// </summary>
        private void ToggleMenu() {
            _instance.gameObject.SetActive(!_instance.gameObject.activeSelf);
            Time.timeScale = IsActive ? 0 : 1;
        }

        /// <summary>
        /// When a player clicks on the "Play"-button, it changes scene
        /// </summary>
        private void StartGame() {
            if (gameScene != string.Empty) {
                onGameStart.Invoke();
                SceneManager.LoadScene(gameScene);
                ToggleMenu();
            } else {
                Debug.LogWarning($"You need to select a scene to load when pressing the Play-button.");
            }
        }

        private void DisplayScoreboard() {
            if (scoreboardScene != string.Empty) {
                UnloadSelfScene();
                LoadSceneAdditively(scoreboardScene);
            } else {
                Debug.LogWarning($"You need to select a scene to load when pressing the Scoreboard-button.");
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
            ShowElement(_scoreboard);
        }

        /// <summary>
        /// Display elements for pause menu, and hide everything else
        /// </summary>
        private void DisplayPausedGameMenu() {
            ShowElement(_overlay);
            ShowElement(_pausedText);
            ShowElement(_resumeButton);
            HideElement(_playButton);
            HideElement(_scoreboard);
        }

        /// <summary>
        /// Showing correct button depending on the scene
        /// Start = Show Play
        /// Other = Show Resume
        /// </summary>
        private void DisplayCorrectElements() {
            if (IsInStartScene) {
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
            _rootElement = _document.rootVisualElement;

            _overlay = _rootElement.Q<VisualElement>("overlay");
            _pausedText = _rootElement.Q<Label>("pausedText");
            _resumeButton = _rootElement.Q<Button>("resume");
            _playButton = _rootElement.Q<Button>("play");
            _scoreboard = _rootElement.Q<Button>("scoreboard");
            _quitButton = _rootElement.Q<Button>("quit");

            _resumeButton.On<ClickEvent>(ev => ToggleMenu());
            _playButton.On<ClickEvent>(ev => StartGame());
            _scoreboard.On<ClickEvent>(ev => DisplayScoreboard());
            _quitButton.On<ClickEvent>(ev => QuitApplication());
            
            DisplayCorrectElements();
        }

        /// <summary>
        /// Cleaning up the click events
        /// </summary>
        protected override void RemoveClickEvents() {
            _resumeButton.UnregisterCallback<ClickEvent>(ev => ToggleMenu());
            _playButton.UnregisterCallback<ClickEvent>(ev => StartGame());
            _scoreboard.UnregisterCallback<ClickEvent>(ev => DisplayScoreboard());
            _quitButton.UnregisterCallback<ClickEvent>(ev => QuitApplication());
        }
    }
}