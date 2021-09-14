using System.Text.RegularExpressions;

namespace FG {
    public class HighScoreValidator {
        public const int MAX_LENGTH_NAME = 12;
        public const string DEFAULT_NAME_REGEX = "^[A-Za-z0-9]{1,12}$";
        
        /// <summary>
        /// Validates only the length of the player name
        /// </summary>
        /// <param name="name">The user name of the player</param>
        /// <returns></returns>
        public static bool IsNameValid(string name) {
            return IsNameValid(name, string.Empty);
        }

        /// <summary>
        /// Validates the name of the player
        /// </summary>
        /// <param name="name">The user name of the player</param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static bool IsNameValid(string name, string regex) {
            bool nameLengthValidation = name.Length > 0 && name.Length <= MAX_LENGTH_NAME;

            if (ReferenceEquals(regex, string.Empty)) {
                return nameLengthValidation;
            }
            
            return nameLengthValidation && Regex.Match(name, regex).Success;
        }
    }
}