using System;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine.Networking;

namespace FG {
	public class HighscoreService {
		/// <summary>
		/// Url to the API server
		/// </summary>
		private string apiUrl;

		public HighscoreService(string apiUrl) {
			this.apiUrl = apiUrl;
		}
		
		/// <summary>
		/// Async call to the API to get the list of high scores
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public async Task<HighscoreResponseData> GetList() {
			UnityWebRequest request = UnityWebRequest.Get(apiUrl);
			request.SetRequestHeader("Content-Type", "application/json");
			UnityWebRequestAsyncOperation requestOperation = request.SendWebRequest();

			while (!requestOperation.isDone) {
				await Task.Yield();
			}

			if (request.result == UnityWebRequest.Result.Success) {
				HighscoreResponseData responseDataData =
						JsonConvert.DeserializeObject<HighscoreResponseData>(request.downloadHandler.text);

					return responseDataData;
			}


			// Don't wont to spoil the details of the error to the user
			throw new Exception("Something went wrong when trying to get high score.");
		}
	}
}
