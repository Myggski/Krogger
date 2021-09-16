using Newtonsoft.Json;

namespace FG {
	/// <summary>
	/// Score information
	/// </summary>
	public class HighScoreData {
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("score")]
		public int Score { get; set; }

		public bool IsValid => HighScoreValidator.IsNameValid(Name, HighScoreValidator.DEFAULT_NAME_REGEX);

		/// <summary>
		/// Constructor that sets name and score directly
		/// </summary>
		/// <param name="name"></param>
		/// <param name="score"></param>
		public HighScoreData(string name, int score) {
			Name = name.ToUpper();
			Score = score;
		}
	}
}
