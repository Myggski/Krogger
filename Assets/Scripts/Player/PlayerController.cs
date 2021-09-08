using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FG {
	public class PlayerController : MonoBehaviour {
		[Header("Movement")]
		[SerializeField]
		private float movementSpeed = 5.0f;

		[SerializeField]
		private float movementStepsInUnits = 1f;

		[Header("Animation")]
		[SerializeField] 
		private string jumpAnimationTriggerName = "Jump";

		[SerializeField]
		private Animator animator;
		
		// Data information
		private float _rotationAngle;
		private List<Vector3> _queuedMovements;
		private Vector3 _currentPosition;
		private Transform _transform;

		// Helpers
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
				Vector2 inputDirection = value.ReadValue<Vector2>();
				QueueNewDirection(new Vector3(inputDirection.x, 0, inputDirection.y));
			}
		}

		/// <summary>
		/// Queues movement directions for movements smoothness
		/// </summary>
		/// <param name="nextDirection"></param>
		private void QueueNewDirection(Vector3 nextDirection) {
			if (HasQueuedMovements && HasSpotsLeftInMovementQueue) {
				_queuedMovements.Add(LastDirection + nextDirection * movementStepsInUnits);
			}
			else if (!HasQueuedMovements) {
				Vector3 currentPosition = new Vector3(_currentPosition.x, _transform.position.y, _currentPosition.z);
				_queuedMovements.Add(currentPosition + (nextDirection * movementStepsInUnits));

				SetupNewMovement();
			}
		}
		
		/// <summary>
		/// Sets the speed of the animation and recalculate the new angle to rotate
		/// </summary>
		private void SetupNewMovement() {
			_rotationAngle = Quaternion.Angle(_transform.rotation,
				Quaternion.LookRotation((NextDirection - _transform.position).normalized));

			if (animator) {
				animator.speed = (movementSpeed * _queuedMovements.Count) / movementStepsInUnits;
				animator.SetTrigger(jumpAnimationTriggerName);
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
			_currentPosition = _transform.position;

			if (HasQueuedMovements) {
				SetupNewMovement();
			}
		}

		/// <summary>
		/// Rotate Krister in sync with movement, the rotation should be done same time as movement
		/// </summary>
		private void MoveAndRotate() {
			_transform.position = Vector3.MoveTowards(_transform.position, NextDirection, (movementSpeed * _queuedMovements.Count) * Time.deltaTime);
			_transform.rotation = Quaternion.RotateTowards(_transform.rotation,Quaternion.LookRotation((NextDirection - _currentPosition).normalized), Time.deltaTime * (_rotationAngle / (1 / movementSpeed * movementStepsInUnits)));

			if (_transform.position == NextDirection) {
				ResetMovementInfo();
			}
		}

		/// <summary>
		/// Setup list and default value of _currentPosition
		/// </summary>
		private void Setup() {
			_queuedMovements = new List<Vector3>();
			_transform = transform;
			_currentPosition = _transform.position;
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
