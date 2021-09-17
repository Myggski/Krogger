using UnityEngine;
using Random = UnityEngine.Random;

namespace FG {
    /// <summary>
    /// Spawns random gameObjects at random x-position
    /// </summary>
    public class ObstacleSpawner : SpawnerBase {
        /// <summary>
        /// Removes obstacles to make it more safe
        /// </summary>
        public void InitializeSafeMode() {
            int amountToRemove = Random.Range(minToSpawn, maxToSpawn);
            amountToRemove = Mathf.Clamp(amountToRemove, 0, _placedGameObject.Count);

            for (int i = 0; i < amountToRemove; i++) {
                Destroy(_placedGameObject[i]);
            }
        }
    }
}
