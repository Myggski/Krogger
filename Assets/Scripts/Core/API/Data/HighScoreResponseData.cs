using Newtonsoft.Json;

namespace FG {
	/// <summary>
	/// Request information
	/// </summary>
	public class HighScoreResponseData<T> {
		[JsonProperty("statuscode")]
		public string StatusCode;
		
		[JsonProperty("status")]
		public int Status;
		
		[JsonProperty("message")]
		public string Message;
		
		[JsonProperty("data")]
		public T Data;
	}
}
