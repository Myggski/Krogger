using UnityEngine;

namespace FG {
	public class DontDestroyOnLoad : MonoBehaviour {
		/// <summary>
		/// Makes sure that the gameObject lives even if the scene changes
		/// </summary>
		public void DontDestroy() {
			DontDestroyOnLoad(gameObject);
		}
	}
}
