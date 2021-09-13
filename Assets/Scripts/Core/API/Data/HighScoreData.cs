using System.Text.RegularExpressions;
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

		public bool IsValid => Name.Length > 0 && Name.Length <= 6 && Regex.Match(Name, "^\\w+$").Success;
	}
}
