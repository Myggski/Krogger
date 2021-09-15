using System;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace FG {
	public class HighScoreService<T> {
		/// <summary>
		/// Url to the API server
		/// </summary>
		private readonly string _apiUrl;

		public HighScoreService(string apiUrl) {
			_apiUrl = apiUrl;
		}
		
		/// <summary>
		/// Async call to the API to get the list of high scores
		/// </summary>
		/// <returns>HighScoreResponseData</returns>
		public async Task<HighScoreResponseData<T>> GetList() {
			return await DoRequest(RequestType.GET);
		}

		/// <summary>
		/// Async call to the API to save high score
		/// </summary>
		/// <param name="highScoreData">High score that's being saved in the list</param>
		/// <returns>HighScoreResponseData</returns>
		public async Task<HighScoreResponseData<T>> SaveScore(HighScoreData highScoreData) {
			if (!highScoreData.IsValid) {
				throw new Exception($"The name need to contain letters and digits (between 1-{HighScoreValidator.MAX_LENGTH_NAME} characters).");
			}
			
			return await DoRequest(RequestType.POST, Encryption.Encrypt(JsonConvert.SerializeObject(highScoreData)));
		}
		
		/// <summary>
		/// Does a HTTP-web-request
		/// </summary>
		/// <param name="requestType">GET- / POST-request</param>
		/// <param name="serializedData">Only for POST-request (Saving Score)</param>
		/// <returns></returns>
		private async Task<HighScoreResponseData<T>> DoRequest(RequestType requestType, string serializedData = "") {
			UnityWebRequest request;

			if (requestType == RequestType.POST) {
				WWWForm form = new WWWForm();
				form.AddField("data", serializedData);

				request = UnityWebRequest.Post(_apiUrl, form);
			} else {
				request = UnityWebRequest.Get(_apiUrl);
			}

			UnityWebRequestAsyncOperation requestOperation = request.SendWebRequest();

			while (!requestOperation.isDone) {
				await Task.Yield();
			}
			
			if (request.result == UnityWebRequest.Result.Success) {
				string decryptedString = Encryption.Decrypt(request.downloadHandler.text);
				
				request.Dispose();
				return JsonConvert.DeserializeObject<HighScoreResponseData<T>>(decryptedString);
			}
			
			request.Dispose();
			throw new Exception("Something went wrong when trying to interact with the API.");
		}
	}
}
