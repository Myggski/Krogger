using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using FG;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace FG {
    public class MenuManager : MonoBehaviour {
        [Scene]
        [SerializeField] 
        private string _sceneName = string.Empty;

        private static MenuManager _instance;
        
        private VisualElement _rootElement;
        private VisualElement _overlay;
        private Label _pausedText;
        private Button _resumeButton;
        private Button _playButton;
        private Button _quitButton;

        private bool IsFirstScene => SceneManager.GetActiveScene().buildIndex == 0;
        private bool IsActive => _instance.gameObject.activeSelf;

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
        /// Making sure that there's only one of this component
        /// </summary>
        private void InitializeManager() {
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
            }
            else {
                _instance = this;
            }
        }

        /// <summary>
        /// Displaying/hiding menu, when the menu shows, pause the game
        /// </summary>
        private void ToggleMenu() {
            // Remove focus to remove warning about focusable
            _rootElement.focusController?.focusedElement?.Blur();

            _instance.gameObject.SetActive(!IsActive);
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
        /// Hides button from the menu by adding a class to the button
        /// </summary>
        /// <param name="element">It can be either Play- or Resume-button</param>
        private void HideElement(VisualElement element) {
            element.AddToClassList("hidden");
        }
        
        /// <summary>
        /// Show button from the menu by removing a class from the button
        /// </summary>
        /// <param name="element">It can be either Play- or Resume-button</param>
        private void ShowElement(VisualElement element) {
            element.RemoveFromClassList("hidden");
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
            HideElement( _playButton);
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
        private void InitializeElements() {
            _rootElement = GetComponent<UIDocument>().rootVisualElement;

            _overlay = _rootElement.Q<VisualElement>("overlay");
            _pausedText = _rootElement.Q<Label>("pausedText");
            _resumeButton = _rootElement.Q<Button>("resume");
            _playButton = _rootElement.Q<Button>("play");
            _quitButton = _rootElement.Q<Button>("quit");

            _resumeButton.RegisterCallback<ClickEvent>(ev => ToggleMenu());
            _playButton.RegisterCallback<ClickEvent>(ev => StartGame());
            _quitButton.RegisterCallback<ClickEvent>(ev => QuitApplication());
            
            DisplayCorrectElements();
        }

        /// <summary>
        /// Cleaning up the click events
        /// </summary>
        private void RemoveClickEvents() {
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

        private void Awake() {
            InitializeManager();
        }

        private void OnEnable() {
            InitializeElements();
        }

        private void OnDisable() {
            RemoveClickEvents();
        }
    }
}