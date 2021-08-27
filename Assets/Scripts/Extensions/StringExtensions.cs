using System;
using UnityEngine;

namespace FG {
    public static class StringExtensions {
        /// <summary>
        /// Convert string to enum then compare the enum value and get direction depending on the value
        /// </summary>
        /// <param name="key">Can be W, A, S or D</param>
        /// <returns>Vector3 direction</returns>
        public static Vector3 GetMovingDirectionByKey(this string key) {
            Enum.TryParse(key, out KeyCode keyCode);

            if (keyCode == KeyCode.W) {
                return Vector3.forward;
            }

            if (keyCode == KeyCode.S) {
                return Vector3.back;
            }

            if (keyCode == KeyCode.A) {
                return Vector3.left;
            }

            if (keyCode == KeyCode.D) {
                return Vector3.right;
            }

            return Vector3.zero;
        }
    }
}