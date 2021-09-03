using System.Collections.Generic;
using Newtonsoft.Json;

namespace FG {
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
