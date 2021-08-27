using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FG {
	public class PlayerController : MonoBehaviour {
		[Header("Movement")]
		[SerializeField]
		private float _movementSpeed = 5.0f;
		[SerializeField]
		private float _movementStepsInUnits = 1f;

		[Header("Animation")]
		[SerializeField] 
		private string _jumpAnimationTriggerName = "Jump";
		[SerializeField]
		private Animator _animator;
		
		private float _rotationAngle;
		private List<Vector3> _queuedMovements;
		private Vector3 _currentPosition;

		private bool HasQueuedMovements => _queuedMovements.Any();
		private bool HasSpotsLeftInMovementQueue => _queuedMovements.Count < 2;
		private Vector3 NextDirection => HasQueuedMovements ? _queuedMovements[0] : Vector3.zero;
		private Vector3 LastDirection => HasQueuedMovements ? _queuedMovements[_queuedMovements.Count - 1] : Vector3.zero;

		/// <summary>
		/// Gets called with UnityEvents by Input System
		/// </summary>
		/// <param name="value">The input value, what button, what state on the button and so on</param>
		public void OnMovement(InputAction.CallbackContext value) {
			if (value.phase == InputActionPhase.Started) {
				QueueNewDirection(value.control.displayName);
			}
		}

		/// <summary>
		/// Queues movement directions for movements smoothness
		/// </summary>
		/// <param name="keyPressed"></param>
		private void QueueNewDirection(string keyPressed) {
			Vector3 nextDirection = keyPressed.GetMovingDirectionByKey();

			if (HasQueuedMovements && HasSpotsLeftInMovementQueue) {
				_queuedMovements.Add(LastDirection + nextDirection * _movementStepsInUnits);
			}
			else if (!HasQueuedMovements) {
				Vector3 currentPosition = new Vector3(_currentPosition.x, transform.position.y, _currentPosition.z);
				_queuedMovements.Add(currentPosition + (nextDirection * _movementStepsInUnits));

				SetupNewMovement();
			}
		}
		
		/// <summary>
		/// Sets the speed of the animation and recalculate the new angle to rotate
		/// </summary>
		private void SetupNewMovement() {
			_rotationAngle = Quaternion.Angle(transform.rotation,
				Quaternion.LookRotation((NextDirection - transform.position).normalized));

			if (_animator) {
				_animator.speed = (_movementSpeed * _queuedMovements.Count) / _movementStepsInUnits;
				_animator.SetTrigger(_jumpAnimationTriggerName);
			}
			else {
				Debug.LogWarning("There's no animation attached of Krister jumping.");
			}
		}

		/// <summary>
		/// Reset the current position, and recalculate the angle between the current position and the new position
		/// </summary>
		private void ResetMovementInfo() {
			_queuedMovements.RemoveAt(0);
			_currentPosition = transform.position;

			if (HasQueuedMovements) {
				SetupNewMovement();
			}
		}

		/// <summary>
		/// Rotate Krister in sync with movement, the rotation should be done same time as movement
		/// </summary>
		private void MoveAndRotate() {
			transform.position = Vector3.MoveTowards(transform.position, NextDirection, (_movementSpeed * _queuedMovements.Count) * Time.deltaTime);
			transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation((NextDirection - _currentPosition).normalized), Time.deltaTime * (_rotationAngle / (1 / _movementSpeed * _movementStepsInUnits)));

			if (transform.position == NextDirection) {
				ResetMovementInfo();
			}
		}

		/// <summary>
		/// Setup list and default value of _currentPosition
		/// </summary>
		private void Setup() {
			_queuedMovements = new List<Vector3>();
			_currentPosition = transform.position;
		}

		/// <summary>
		/// Move and rotate Krister if there's any queued movements
		/// </summary>
		private void CheckForMovement() {
			if (HasQueuedMovements) {
				MoveAndRotate();
			}
		}
		
		private void Awake() {
			Setup();
		}

		private void Update() {
			CheckForMovement();
		}
	}
}
