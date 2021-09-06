using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

namespace FG {
	/// <summary>
	/// Request information
	/// </summary>
	public class HighscoreResponseData {
		[JsonProperty("statuscode")]
		public string StatusCode;
		
		[JsonProperty("status")]
		public int Status;
		
		[JsonProperty("message")]
		public string Message;
		
		[JsonProperty("data")]
		public List<HighscoreData> Data;
	}
}
