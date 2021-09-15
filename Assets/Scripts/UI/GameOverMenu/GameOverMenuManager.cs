using System;
using System.Net;
using System.Text.RegularExpressions;
using Core.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace FG {
	public class GameOverMenuManager : UIManagerBase<GameOverMenuManager> {
		// Scenes
		[Scene]
		[SerializeField]
		private string gameScene = string.Empty;
		[Scene]
		[SerializeField]
		private string menuScene = string.Empty;
		
		// API
		[Header("API")]
		[SerializeField] 
		private string apiUrl = string.Empty;
		[SerializeField] 
		private string successMessage = "Score Saved!";
		[SerializeField] 
		private string loadingMessage = "Loading";
		[SerializeField] 
		private string internalServerErrorMessage = "Server is down :(";
		[SerializeField] 
		private IntVariable currentScore = default;
		
		// UI
		private Label _totalScoreText;
		private Label _messageText;
		private Button _submitButton;
		private Button _playAgainButton;
		private Button _backToTheMenuButton;
		private Button _quitButton;
		private TextField _nameInput;

		// Data information
		private HighScoreService<HighScoreData> _highScoreService;
		
		/// <summary>
		/// Loads scene with the SceneManager
		/// </summary>
		/// <param name="scene">What kind of scene to load</param>
		private void LoadScene(string scene) {
			SceneManager.LoadScene(scene);
		}

		/// <summary>
		/// Sets the success/error-message in the UI
		/// </summary>
		/// <param name="message">success/error-message</param>
		/// <param name="messageType">What time of message, if it's a success or a error message</param>
		private void SetMessage(string message) {
			_messageText.text = message.ToUpper();
		}

		/// <summary>
		/// Sets the score text in UI
		/// </summary>
		private void SetScoreText() {
			_totalScoreText.text = $"TOTAL SCORE: {currentScore.Value}";
		}

		/// <summary>
		/// Validate input with regex, and the length of the characters
		/// </summary>
		/// <param name="ev"></param>
		private void ValidateInput(InputEvent ev) {
			string inputValue = ev.newData;
			bool isNameValid = HighScoreValidator.IsNameValid(ev.newData, HighScoreValidator.DEFAULT_NAME_REGEX);

			if (!isNameValid) {
				if (inputValue.Length <= 0) {
					SetMessage("Name is required!");
				} else if (inputValue.Length > HighScoreValidator.MAX_LENGTH_NAME) {
					SetMessage($"Max length is {HighScoreValidator.MAX_LENGTH_NAME} characters!");
				} else if (!Regex.Match(inputValue, HighScoreValidator.DEFAULT_NAME_REGEX).Success) {
					SetMessage("Invalid input! (Only A-Z and 0-9)");
				}
			}
			else {
				SetMessage(string.Empty);
			}

			_submitButton.SetEnabled(isNameValid);
		}

		/// <summary>
		/// Submit high score to the API
		/// </summary>
		private async void SubmitHighScore() {
			string originalButtonText = _submitButton.text;
			MessageType messageType = MessageType.Error;

			// Setting the UI in "loading"-mode
			_submitButton.text = loadingMessage;
			_submitButton.SetEnabled(false);
			_nameInput.SetEnabled(false);

			try {
				HighScoreResponseData<HighScoreData> responseData =
					await _highScoreService.SaveScore(new HighScoreData(_nameInput.value, currentScore.Value));

				if (responseData.Status == (int)HttpStatusCode.Created) {
					messageType = MessageType.Success;
					SetMessage(successMessage);
				} else {
					SetMessage(responseData.Message);
				}
			} catch {
				SetMessage(internalServerErrorMessage);
			} finally {
				// Setting the UI in "done"-mode depending on the end result
				_submitButton.text = originalButtonText;
				_submitButton.SetEnabled(messageType == MessageType.Error);
				_nameInput.SetEnabled(messageType == MessageType.Error);
			}
		}

		/// <summary>
		/// Setup HighScoreService
		/// </summary>
		private void Setup() {
			if (apiUrl != string.Empty) {
				_highScoreService = new HighScoreService<HighScoreData>(apiUrl);	
			} else {
				Debug.LogWarning("You need to add apiUrl to make an API-request");
			}
		}

		/// <summary>
		/// Add click events on the buttons in the menu
		/// </summary>
		protected override void InitializeElements() {
			VisualElement root = Document.rootVisualElement.Q<VisualElement>("root");
			_totalScoreText = root.Q<Label>("game-over-sub-header");
			_messageText = root.Q<Label>("message-text");
			_submitButton = root.Q<Button>("submit-button");
			_playAgainButton = root.Q<Button>("play-again-button");
			_backToTheMenuButton = root.Q<Button>("back-to-menu-button");
			_quitButton = root.Q<Button>("quit-button");
			_nameInput = root.Q<TextField>("name-input");
			
			// Don't need to wait (await) for SubmitHighScore
			_submitButton.On<ClickEvent>(ev => SubmitHighScore());
			_playAgainButton.On<ClickEvent>(ev => LoadScene(gameScene));
			_backToTheMenuButton.On<ClickEvent>(ev => LoadScene(menuScene));
			_quitButton.On<ClickEvent>(ev => QuitApplication());
			_nameInput.RegisterCallback<InputEvent>(ValidateInput);
			
			_submitButton.SetEnabled(false);
		}

		/// <summary>
		/// Cleaning up the click events
		/// </summary>
		protected override void RemoveClickEvents() {
			_submitButton.UnregisterCallback<ClickEvent>(ev => SubmitHighScore());
			_playAgainButton.UnregisterCallback<ClickEvent>(ev => LoadScene(gameScene));
			_backToTheMenuButton.UnregisterCallback<ClickEvent>(ev => LoadScene(menuScene));
			_quitButton.UnregisterCallback<ClickEvent>(ev => QuitApplication());
			_nameInput.UnregisterCallback<InputEvent>(ValidateInput);
		}

		protected override void Awake() {
			base.Awake();

			Setup();
		}

		protected override void OnEnable() {
			base.OnEnable();
			
			SetScoreText();
		}
	}
}
