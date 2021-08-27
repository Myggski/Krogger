using UnityEngine;

namespace FG {
    public class InputHelper {
        public static Vector3 GetMovingDirectionByKey(string key) {
            if (key == KeyCode.W.ToString()) {
                return Vector3.forward;
            }

            if (key == KeyCode.S.ToString()) {
                return Vector3.back;
            }

            if (key == KeyCode.A.ToString()) {
                return Vector3.left;
            }

            if (key == KeyCode.D.ToString()) {
                return Vector3.right;
            }

            return Vector3.zero;
        }
    }
}