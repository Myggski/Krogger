using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlasticPipe.Server;
using UnityEditor.PackageManager;
using UnityEngine.Networking;

namespace FG {
	public class HighscoreService {
		private string apiUrl;

		public HighscoreService(string apiUrl) {
			this.apiUrl = apiUrl;
		}

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


			throw new Exception("Something went wrong when trying to get high score.");
		}
	}
}
