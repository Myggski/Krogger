using Core.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace FG {
    public sealed class MenuManager : UIManagerBase<MenuManager> {
        [Scene] 
        [SerializeField] 
        private string gameScene = string.Empty;
        [Scene] 
        [SerializeField] 
        private string scoreboardScene = string.Empty;
        
        private VisualElement overlay;
        private Label pausedText;
        private Button resumeButton;
        private Button playButton;
        private Button scoreboard;
        private Button quitButton;

        private bool IsStillInMenu => SceneManager.GetActiveScene().name == gameScene || SceneManager.GetActiveScene().name == scoreboardScene;
        private bool IsActive => instance.gameObject.activeSelf;

        /// <summary>
        /// This is called when the player presses ESC with the new Unity Input System
        /// </summary>
        /// <param name="value"></param>
        public void OnMenuToggle(InputAction.CallbackContext value) {
            if (IsStillInMenu || value.phase != InputActionPhase.Started) {
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
            if (gameScene != string.Empty) {
                SceneManager.LoadScene(gameScene);
                ToggleMenu();
            } else {
                Debug.LogWarning($"You need to select a scene to load when pressing the Play-button.");
            }
        }

        private void DisplayScoreboard() {
            if (scoreboardScene != string.Empty) {
                SceneManager.LoadScene(scoreboardScene);
                ToggleMenu();
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
            HideElement(overlay);
            HideElement(pausedText);
            HideElement(resumeButton);
            ShowElement(playButton);
            ShowElement(scoreboard);
        }

        /// <summary>
        /// Display elements for pause menu, and hide everything else
        /// </summary>
        private void DisplayPausedGameMenu() {
            ShowElement(overlay);
            ShowElement(pausedText);
            ShowElement(resumeButton);
            HideElement(playButton);
            HideElement(scoreboard);
        }

        /// <summary>
        /// Showing correct button depending on the scene
        /// Start = Show Play
        /// Other = Show Resume
        /// </summary>
        private void DisplayCorrectElements() {
            if (IsStillInMenu) {
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

            overlay = rootElement.Q<VisualElement>("overlay");
            pausedText = rootElement.Q<Label>("pausedText");
            resumeButton = rootElement.Q<Button>("resume");
            playButton = rootElement.Q<Button>("play");
            scoreboard = rootElement.Q<Button>("scoreboard");
            quitButton = rootElement.Q<Button>("quit");

            resumeButton.On<ClickEvent>(ev => ToggleMenu());
            playButton.On<ClickEvent>(ev => StartGame());
            scoreboard.On<ClickEvent>(ev => DisplayScoreboard());
            quitButton.On<ClickEvent>(ev => QuitApplication());

            DisplayCorrectElements();
        }

        /// <summary>
        /// Cleaning up the click events
        /// </summary>
        protected override void RemoveClickEvents() {
            if (!ReferenceEquals(resumeButton, null)) {
                resumeButton.UnregisterCallback<ClickEvent>(ev => ToggleMenu());
            }

            if (!ReferenceEquals(playButton, null)) {
                playButton.UnregisterCallback<ClickEvent>(ev => StartGame());
            }
            
            if (!ReferenceEquals(scoreboard, null)) {
                scoreboard.UnregisterCallback<ClickEvent>(ev => DisplayScoreboard());
            }

            if (!ReferenceEquals(quitButton, null)) {
                quitButton.UnregisterCallback<ClickEvent>(ev => QuitApplication());
            }
        }
    }
}