using System;
using UnityEngine;

namespace FG {
    public abstract class ManagerBase<T> : MonoBehaviour {
        // The instance
        protected static T Instance;

        /// <summary>
        /// Making sure that there's only one of this component
        /// </summary>
        private void InitializeManager() {
            if (!ReferenceEquals(Instance, null) && !ReferenceEquals(Instance, this)) {
                Destroy(gameObject);
            }
            else {
                Instance = (T)Convert.ChangeType(this, typeof(T));
            }
        }
        
        /// <summary>
        /// Cleanup the instance when destroyed
        /// </summary>
        private void RemoveInstance() {
            if (ReferenceEquals((T) Convert.ChangeType(this, typeof(T)), Instance)) {
                Instance = default;
            }
        }
        
        protected virtual void Awake() {
            InitializeManager();
        }
        
        private void OnDestroy() {
            RemoveInstance();
        }
    }
}