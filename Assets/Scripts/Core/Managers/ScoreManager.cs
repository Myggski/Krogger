using UnityEngine;
using UnityEngine.Events;

namespace FG {
	public class ScoreManager : ManagerBase<ScoreManager> {
		/// <summary>
		/// The current highscore
		/// </summary>
		[SerializeField]
		private IntVariable currentScore;

		/// <summary>
		/// Triggers when score is added
		/// </summary>
		[SerializeField]
		private UnityEvent playerScored = new UnityEvent();

		/// <summary>
		/// It adds score if the value is bigger than zero.
		/// This method will be called by an IntEvent. 
		/// </summary>
		/// <param name="scoreToAdd"></param>
		public void AddScore(int scoreToAdd) {
			currentScore.Value += scoreToAdd;
			playerScored.Invoke();
		}

		/// <summary>
		/// It resets the score.
		/// This method will be called by an IntEvent.
		/// </summary>
		public void ResetScore() {
			currentScore.Value = 0;
		}

		protected override void Awake() {
			base.Awake();

			ResetScore();
		}
	}
}
