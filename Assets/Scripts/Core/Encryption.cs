using System;
using System.Security.Cryptography;
using System.Text;

namespace FG {
	public class Encryption {
		private const string SUPER_SECRET_KEY = "KR15T3RL0V3P3P51";
			
		/// <summary>
		/// Encrypts Serialized data or a regular string
		/// </summary>
		/// <param name="source">data to encrypt</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static string Encrypt(string source) {
			TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();

			byte[] byteBuff;

			try {
				desCryptoProvider.Key = Encoding.UTF8.GetBytes(SUPER_SECRET_KEY);
				desCryptoProvider.IV = Convert.FromBase64String("QUJDREVGR0g=");
				byteBuff = Encoding.UTF8.GetBytes(source);

				string iv = Convert.ToBase64String(desCryptoProvider.IV);
				Console.WriteLine("iv: {0}", iv);

				string encoded =
					Convert.ToBase64String(desCryptoProvider.CreateEncryptor()
						.TransformFinalBlock(byteBuff, 0, byteBuff.Length));

				return encoded;
			}
			catch (Exception except) {
				throw new Exception($"{except}\n\n{except.StackTrace}");
			}
		}

		/// <summary>
		/// Decrypts stringified-JSON or a regular string
		/// </summary>
		/// <param name="encodedText">The stringified-JSON or a regular string</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static string Decrypt(string encodedText) {
			TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();

			byte[] byteBuff;

			try {
				desCryptoProvider.Key = Encoding.UTF8.GetBytes(SUPER_SECRET_KEY);
				desCryptoProvider.IV = Convert.FromBase64String("QUJDREVGR0g=");
				byteBuff = Convert.FromBase64String(encodedText);

				string plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor()
					.TransformFinalBlock(byteBuff, 0, byteBuff.Length));
				return plaintext;
			}
			catch (Exception except) {
				throw new Exception($"{except}\n\n{except.StackTrace}");
			}
		}
	}
}
