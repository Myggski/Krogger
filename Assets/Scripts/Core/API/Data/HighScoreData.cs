using Unity.Plastic.Newtonsoft.Json;

namespace FG {
	/// <summary>
	/// Score information
	/// </summary>
	public class HighScoreData {
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("score")]
		public int Score { get; set; }
	}
}
