using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Core.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace FG {
	public sealed class HighscoreManager : UIManagerBase<HighscoreManager> {
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
		private VisualElement scoresContent;
		private VisualElement messageContent;
		private VisualElement contentButtonsWrapper;
		private Button nameButton;
		private Button scoreButton;
		private Button backButton;
		private Button refreshButton;
		private Label messageText;
		private UQueryBuilder<VisualElement> playerScores;
		
		// Data information
		private bool ascending;
		private HighscoreService highscoreService;
		private HighscoreSortType previousSortType;
		private List<HighscoreData> highScoreList;
		
		
		// "Constant" top highscore
		private List<HighscoreData> TopHighscore => highScoreList
			.OrderByDescending(highscore => highscore.Score)
			.ToList();

		/// <summary>
		/// Trying to find index of currentHighscoreData in top highscore list and return it as a fancy string
		/// </summary>
		/// <param name="currentHighscoreData">The currenet highscore data that's being rendered</param>
		/// <returns></returns>
		private string GetRankText(HighscoreData currentHighscoreData) {
			if (ReferenceEquals(currentHighscoreData, default(HighscoreData))) {
				return string.Empty;
			}

			int playerScoreIndex = TopHighscore
				.FindIndex(highscoreData => 
					ReferenceEquals(highscoreData, currentHighscoreData));

			return playerScoreIndex >= 0 
				? $"#{playerScoreIndex + 1}" 
				: string.Empty;
		}

		/// <summary>
		/// Disable refresh-button and change text to loading...
		/// </summary>
		private void DisplayLoading() {
			refreshButton.SetEnabled(false);
			messageText.text = loadingMessage;

			ShowElement(messageContent);
			HideElement(scoresContent, VisibilityStyleType.opacity);
		}

		/// <summary>
		/// Enables refresh-button and removes message-text
		/// </summary>
		private void HideLoading(string errorMessage) {
			refreshButton.SetEnabled(true);

			if (errorMessage == string.Empty) {
				HideElement(messageContent);
				ShowElement(scoresContent, VisibilityStyleType.opacity);
			} else {
				messageText.text = errorMessage;
			}
		}

		/// <summary>
		/// Get highscore from API with the help of HighscoreService
		/// </summary>
		private async void RequestHighscore() {
			string errorMessage = string.Empty;

			DisplayLoading();

			try {
				HighscoreResponseData responseData = await highscoreService.GetList();

				if (responseData.Status == (int)HttpStatusCode.OK) {
					highScoreList = responseData.Data;
				
					SortListBy(HighscoreSortType.Score);
					
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
		private HighscoreData GetHighScoreDataFromIndex(int index) {
			if (index < highScoreList.Count) {
				return highScoreList[index];
			}

			return default;
		}

		/// <summary>
		/// Render list in UI
		/// </summary>
		private void DisplayList() {
			for (int i = 0; i < playerScores.ToList().Count; i++) {
				HighscoreData highscoreData = GetHighScoreDataFromIndex(i);
				
				playerScores.AtIndex(i).Q<Label>("player-rank-text").text = GetRankText(highscoreData);
				playerScores.AtIndex(i).Q<Label>("player-name-text").text = highscoreData?.Name;
				playerScores.AtIndex(i).Q<Label>("player-score-text").text = highscoreData?.Score.ToString();
			}
		}

		/// <summary>
		/// Sort list depending on sortType (HighscoreSortType) and sortOrder (ascending = true/false)
		/// </summary>
		/// <param name="sortType">What to sort, by name or by score</param>
		private void SortListBy(HighscoreSortType sortType) {
			string propertyNameFromEnum = Enum.GetName(typeof(HighscoreSortType), sortType);
			ascending = !ascending;
			
			if (sortType != previousSortType) {
				ascending = false;
			}

			// Local function to get right property to sort
			object OrderByQuery(HighscoreData data, string propertyName) {
				return data.GetType().GetProperty(propertyName)?.GetValue(data);
			}
			
			if (ascending) {
				highScoreList = highScoreList
					.OrderBy(highscoreData =>
						OrderByQuery(highscoreData, propertyNameFromEnum))
					.ToList();
			} else {
				highScoreList = highScoreList
					.OrderByDescending(highscoreData =>
						OrderByQuery(highscoreData, propertyNameFromEnum))
					.ToList();
			}

			previousSortType = sortType;
			DisplayList();
		}

		/// <summary>
		/// Loads menu scene
		/// </summary>
		private void GoBackScene() {
			if (menuScene != string.Empty) {
				SceneManager.LoadScene(menuScene);
			} else {
				Debug.LogWarning($"You need to select a scene to load when pressing the \"Back to menu\"-button.");
			}
		}

		/// <summary>
		/// Setup Lists and HighscoreService
		/// </summary>
		private void Setup() {
			highScoreList = new List<HighscoreData>();

			if (apiUrl != string.Empty) {
				highscoreService = new HighscoreService(apiUrl);	
			} else {
				Debug.LogWarning("You need to add apiUrl to make an API-request");
			}
		}

		/// <summary>
		/// Add click events on the buttons for sorting
		/// </summary>
		protected override void InitializeElements() {
			rootElement = document.rootVisualElement;

			// content-wrapper
			scoresContent = rootElement.Q<VisualElement>("scores-content");
			contentButtonsWrapper = scoresContent.Q<VisualElement>("buttons");
			nameButton = contentButtonsWrapper.Q<Button>("name-button");
			scoreButton = contentButtonsWrapper.Q<Button>("score-button");
			backButton = rootElement.Q<Button>("back-button");
			refreshButton = rootElement.Q<Button>("refresh-button");
			playerScores = rootElement.Query<VisualElement>("player-score");
			messageContent = rootElement.Q<VisualElement>("message-content");
			messageText = messageContent.Q<Label>("message-text");

			nameButton.On<ClickEvent>(ev => SortListBy(HighscoreSortType.Name));
			scoreButton.On<ClickEvent>(ev => SortListBy(HighscoreSortType.Score));
			backButton.On<ClickEvent>(ev => GoBackScene());
			refreshButton.On<ClickEvent>(ev => RequestHighscore());
		}

		/// <summary>
		/// Cleaning up the click events
		/// </summary>
		protected override void RemoveClickEvents() {
			nameButton.UnregisterCallback<ClickEvent>(ev => SortListBy(HighscoreSortType.Name));
			scoreButton.UnregisterCallback<ClickEvent>(ev => SortListBy(HighscoreSortType.Score));
			backButton.UnregisterCallback<ClickEvent>(ev => GoBackScene());
			refreshButton.UnregisterCallback<ClickEvent>(ev => RequestHighscore());
		}

		protected override void Awake() {
			base.Awake();

			Setup();
		}

		protected override void OnEnable() {
			base.OnEnable();

			RequestHighscore();
		}
	}
}
