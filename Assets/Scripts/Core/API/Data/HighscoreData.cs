using Newtonsoft.Json;

namespace FG {
	public class HighscoreData {
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("score")]
		public int Score { get; set; }
	}
}
