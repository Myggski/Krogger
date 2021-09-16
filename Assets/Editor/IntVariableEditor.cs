using UnityEditor;

namespace FG {
#if UNITY_EDITOR

    [CustomEditor(typeof(IntVariable))]
    public class CustomTypeEditor : Editor {
        /// <summary>
        /// Makes the IntVariable editable in inspector
        /// </summary>
        public override void OnInspectorGUI() {
            IntVariable customType = (IntVariable)target;

            int value = EditorGUILayout.IntField("Value", customType.Value);
            
            if (value != customType.Value) {
                customType.Value = value;
            }
        }
    }

#endif
}