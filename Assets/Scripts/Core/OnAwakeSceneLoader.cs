using UnityEngine;
using UnityEngine.SceneManagement;

namespace FG {
    public class OnAwakeSceneLoader : MonoBehaviour {
        [Scene]
        [SerializeField]
        private string sceneToLoad = string.Empty;

        /// <summary>
        /// Loads the scene with the help of SceneManager
        /// </summary>
        private void LoadScene() {
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        }
    
        private void Awake() {
            LoadScene();
        }
    }
}
