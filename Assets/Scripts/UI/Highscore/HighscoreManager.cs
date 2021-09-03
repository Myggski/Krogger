using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;

namespace FG {
	public sealed class HighscoreManager : UIManager<HighscoreManager> {
		// API
		[SerializeField]
		private string apiUrl = string.Empty;
		private HighscoreService highscoreService;
		
		// UI
		private VisualElement contentWrapper;
		private VisualElement loadingWrapper;
		private VisualElement contentButtonsWrapper;
		private Button nameButton;
		private Button scoreButton;
		private UQueryBuilder<VisualElement> playerScores;
		
		// Data information
		private List<HighscoreData> highScoreList;
		private bool ascending;
		private HighscoreSortType previousSortType;
		
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

		private void LoadingDone() {
			HideElement(loadingWrapper);
			contentWrapper.style.opacity = 1;
		}

		/// <summary>
		/// Get highscore from API with the help of HighscoreService
		/// </summary>
		private async void RequestHighscore() {
			try {
				HighscoreResponseData responseData = await highscoreService.GetList();
				highScoreList = responseData.Data;

				LoadingDone();
				SortListBy(HighscoreSortType.Score);
			}
			catch (Exception ex) {
				Debug.LogWarning(ex);
			}
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
			}
			else {
				highScoreList = highScoreList
					.OrderByDescending(highscoreData =>
						OrderByQuery(highscoreData, propertyNameFromEnum))
					.ToList();
			}

			previousSortType = sortType;
			DisplayList();
		}

		/// <summary>
		/// Setup Lists and HighscoreService
		/// </summary>
		private void Setup() {
			highScoreList = new List<HighscoreData>();

			if (apiUrl != string.Empty) {
				highscoreService = new HighscoreService(apiUrl);	
			}
			else {
				Debug.LogWarning("You need to add apiUrl to make an API-request");
			}
		}

		/// <summary>
		/// Add click events on the buttons for sorting
		/// </summary>
		protected override void InitializeElements() {
			rootElement = document.rootVisualElement;
			contentWrapper = rootElement.Q<VisualElement>("content-wrapper");
			loadingWrapper = rootElement.Q<VisualElement>("loading-wrapper");

			// content-wrapper
			contentButtonsWrapper = contentWrapper.Q<VisualElement>("buttons");
			nameButton = contentButtonsWrapper.Q<Button>("name-button");
			scoreButton = contentButtonsWrapper.Q<Button>("score-button");
			playerScores = rootElement.Query<VisualElement>("player-score");
			
			nameButton.RegisterCallback<ClickEvent>(ev => SortListBy(HighscoreSortType.Name));
			scoreButton.RegisterCallback<ClickEvent>(ev => SortListBy(HighscoreSortType.Score));
		}

		/// <summary>
		/// Cleaning up the click events
		/// </summary>
		protected override void RemoveClickEvents() {
			nameButton.UnregisterCallback<ClickEvent>(ev => SortListBy(HighscoreSortType.Name));
			scoreButton.UnregisterCallback<ClickEvent>(ev => SortListBy(HighscoreSortType.Score));
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
