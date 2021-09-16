using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace FG {
	public class PlayerStun : MonoBehaviour, IStunnable {
		[SerializeField]
		private GameObject stunPrefab;
		[SerializeField]
		private int _scoreOnStun = -1;
		[SerializeField]
		private UnityEvent playerStunned = new UnityEvent();

		private bool _isStunned;
		private bool _isInvulnerable;
		
		public bool IsStunned => _isStunned;
		public bool IsInvulnerable => _isInvulnerable;

		public void Stun(float stunTimeInSeconds, float invulnerableTimeInSeconds) {
			if (_isStunned || _isInvulnerable) {
				return;
			}
			
			StartCoroutine(StartStun(stunTimeInSeconds, invulnerableTimeInSeconds));
		}

		/// <summary>
		/// Starts the stun and invulnerable sequence
		/// </summary>
		/// <param name="stunTimeInSeconds">Number of seconds the player is stunned</param>
		/// <param name="invulnerableTimeInSeconds">Number of seconds the player is invulnerable</param>
		/// <returns></returns>
		private IEnumerator StartStun(float stunTimeInSeconds, float invulnerableTimeInSeconds) {
			ScoreManager.Instance.AddScore(_scoreOnStun);

			playerStunned.Invoke();
			stunPrefab.SetActive(true);

			// Start the stun
			_isStunned = true;
			yield return new WaitForSeconds(stunTimeInSeconds);
			_isStunned = false;
			
			stunPrefab.SetActive(false);

			// The stun is done, and now the player is invulnerable
			_isInvulnerable = true;
			yield return new WaitForSeconds(invulnerableTimeInSeconds);
			_isInvulnerable = false;
		}
	}
}
