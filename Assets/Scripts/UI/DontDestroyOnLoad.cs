using System;
using UnityEngine;

namespace FG {
	public class DontDestroyOnLoad : MonoBehaviour {
		/// <summary>
		/// Makes sure that the gameObject lives even if the scene changes
		/// </summary>
		private void DontDestroy() {
			DontDestroyOnLoad(gameObject);
		}

		private void Awake() {
			DontDestroy();
		}
	}
}
