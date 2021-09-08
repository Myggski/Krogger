using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Core.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace FG {
	public sealed class HighscoreManager : UIManagerBase<HighscoreManager> {
		// Scene
		[Scene]
		[SerializeField]
		private string menuScene = string.Empty;

		[Header("API")]
		// API
		[SerializeField]
		private string apiUrl = string.Empty;
		[SerializeField] 
		private string loadingMessage = "LOADING HIGH SCORES...";
		[SerializeField] 
		private string internalServerErrorMEssage = "Server is down :(";
		
		// UI
		private VisualElement _scoresContent;
		private VisualElement _messageContent;
		private VisualElement _contentButtonsWrapper;
		private Button _nameButton;
		private Button _scoreButton;
		private Button _backButton;
		private Button _refreshButton;
		private Label _messageText;
		private UQueryBuilder<VisualElement> _playerScores;
		
		// Data information
		private bool _ascending;
		private HighScoreService _highScoreService;
		private HighScoreSortType _previousSortType;
		private List<HighScoreData> _highScoreList;
		
		
		// "Constant" top highscore
		private List<HighScoreData> TopHighScore => _highScoreList
			.OrderByDescending(highScore => highScore.Score)
			.ToList();

		/// <summary>
		/// Trying to find index of currentHighscoreData in top highscore list and return it as a fancy string
		/// </summary>
		/// <param name="currentHighScoreData">The currenet highscore data that's being rendered</param>
		/// <returns></returns>
		private string GetRankText(HighScoreData currentHighScoreData) {
			if (ReferenceEquals(currentHighScoreData, default(HighScoreData))) {
				return string.Empty;
			}

			int playerScoreIndex = TopHighScore
				.FindIndex(highScoreData => 
					ReferenceEquals(highScoreData, currentHighScoreData));

			return playerScoreIndex >= 0 
				? $"#{playerScoreIndex + 1}" 
				: string.Empty;
		}

		/// <summary>
		/// Disable refresh-button and change text to loading...
		/// </summary>
		private void DisplayLoading() {
			_refreshButton.SetEnabled(false);
			_messageText.text = loadingMessage;

			ShowElement(_messageContent);
			HideElement(_scoresContent, VisibilityStyleType.opacity);
		}

		/// <summary>
		/// Enables refresh-button and removes message-text
		/// </summary>
		private void HideLoading(string errorMessage) {
			_refreshButton.SetEnabled(true);

			if (errorMessage == string.Empty) {
				HideElement(_messageContent);
				ShowElement(_scoresContent, VisibilityStyleType.opacity);
			} else {
				_messageText.text = errorMessage;
			}
		}

		/// <summary>
		/// Get highscore from API with the help of HighscoreService
		/// </summary>
		private async void RequestHighScores() {
			string errorMessage = string.Empty;

			DisplayLoading();

			try {
				HighScoreResponseData responseData = await _highScoreService.GetList();

				if (responseData.Status == (int)HttpStatusCode.OK) {
					_highScoreList = responseData.Data;
				
					SortListBy(HighScoreSortType.Score);
					
				} else {
					errorMessage = responseData.Message;
				}
			}
			catch {
				errorMessage = internalServerErrorMEssage;
			}
			
			HideLoading(errorMessage);
		}

		/// <summary>
		/// Returns HighscoreData from highScoreList with index
		/// </summary>
		/// <param name="index">The index of the HighscoreData that we want from highScoreList</param>
		/// <returns></returns>
		private HighScoreData GetHighScoreDataFromIndex(int index) {
			if (index < _highScoreList.Count) {
				return _highScoreList[index];
			}

			return default;
		}

		/// <summary>
		/// Render list in UI
		/// </summary>
		private void DisplayList() {
			for (int i = 0; i < _playerScores.ToList().Count; i++) {
				HighScoreData highScoreData = GetHighScoreDataFromIndex(i);
				
				_playerScores.AtIndex(i).Q<Label>("player-rank-text").text = GetRankText(highScoreData);
				_playerScores.AtIndex(i).Q<Label>("player-name-text").text = highScoreData?.Name;
				_playerScores.AtIndex(i).Q<Label>("player-score-text").text = highScoreData?.Score.ToString();
			}
		}

		/// <summary>
		/// Sort list depending on sortType (HighscoreSortType) and sortOrder (ascending = true/false)
		/// </summary>
		/// <param name="sortType">What to sort, by name or by score</param>
		private void SortListBy(HighScoreSortType sortType) {
			string propertyNameFromEnum = Enum.GetName(typeof(HighScoreSortType), sortType);
			_ascending = !_ascending;
			
			if (sortType != _previousSortType) {
				_ascending = false;
			}

			// Local function to get right property to sort
			object OrderByQuery(HighScoreData data, string propertyName) {
				return data.GetType().GetProperty(propertyName)?.GetValue(data);
			}
			
			if (_ascending) {
				_highScoreList = _highScoreList
					.OrderBy(highscoreData =>
						OrderByQuery(highscoreData, propertyNameFromEnum))
					.ToList();
			} else {
				_highScoreList = _highScoreList
					.OrderByDescending(highscoreData =>
						OrderByQuery(highscoreData, propertyNameFromEnum))
					.ToList();
			}

			_previousSortType = sortType;
			DisplayList();
		}

		/// <summary>
		/// Loads menu scene
		/// </summary>
		private void GoBackScene() {
			if (menuScene != string.Empty) {
				UnloadSelfScene();
				LoadSceneAdditively(menuScene);
			} else {
				Debug.LogWarning($"You need to select a scene to load when pressing the \"Back to menu\"-button.");
			}
		}

		/// <summary>
		/// Setup Lists and HighscoreService
		/// </summary>
		private void Setup() {
			_highScoreList = new List<HighScoreData>();

			if (apiUrl != string.Empty) {
				_highScoreService = new HighScoreService(apiUrl);	
			} else {
				Debug.LogWarning("You need to add apiUrl to make an API-request");
			}
		}

		/// <summary>
		/// Add click events on the buttons for sorting
		/// </summary>
		protected override void InitializeElements() {
			_rootElement = _document.rootVisualElement;

			// content-wrapper
			_scoresContent = _rootElement.Q<VisualElement>("scores-content");
			_contentButtonsWrapper = _scoresContent.Q<VisualElement>("buttons");
			_nameButton = _contentButtonsWrapper.Q<Button>("name-button");
			_scoreButton = _contentButtonsWrapper.Q<Button>("score-button");
			_backButton = _rootElement.Q<Button>("back-button");
			_refreshButton = _rootElement.Q<Button>("refresh-button");
			_playerScores = _rootElement.Query<VisualElement>("player-score");
			_messageContent = _rootElement.Q<VisualElement>("message-content");
			_messageText = _messageContent.Q<Label>("message-text");

			_nameButton.On<ClickEvent>(ev => SortListBy(HighScoreSortType.Name));
			_scoreButton.On<ClickEvent>(ev => SortListBy(HighScoreSortType.Score));
			_backButton.On<ClickEvent>(ev => GoBackScene());
			_refreshButton.On<ClickEvent>(ev => RequestHighScores());
		}

		/// <summary>
		/// Cleaning up the click events
		/// </summary>
		protected override void RemoveClickEvents() {
			_nameButton.UnregisterCallback<ClickEvent>(ev => SortListBy(HighScoreSortType.Name));
			_scoreButton.UnregisterCallback<ClickEvent>(ev => SortListBy(HighScoreSortType.Score));
			_backButton.UnregisterCallback<ClickEvent>(ev => GoBackScene());
			_refreshButton.UnregisterCallback<ClickEvent>(ev => RequestHighScores());
		}

		protected override void Awake() {
			base.Awake();

			Setup();
		}

		protected override void OnEnable() {
			base.OnEnable();

			RequestHighScores();
		}
	}
}
