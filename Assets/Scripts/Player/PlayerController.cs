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

		[SerializeField]
		[Tooltip("The layer where the obstacles are, for optimization.")]
		private LayerMask obstacleLayerMask = 1 << 6;
		
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
		/// Checks if the next position doesn't contains any obstacles
		/// </summary>
		/// <param name="nextPosition">The next position that the player wants to move to</param>
		/// <returns></returns>
		private bool IsNextPositionValid(Vector3 nextPosition) {
			Collider[] obstacle = new Collider[1];
			Physics.OverlapSphereNonAlloc(nextPosition, movementStepsInUnits / 4, obstacle, obstacleLayerMask);
			return ReferenceEquals(obstacle[0], null);
		}

		/// <summary>
		/// Queues movement directions for movements smoothness
		/// </summary>
		/// <param name="nextDirection"></param>
		private void QueueNewDirection(Vector3 nextDirection) {
			Vector3 newPosition = Vector3.zero;
			
			if (HasQueuedMovements && HasSpotsLeftInMovementQueue) {
				newPosition = LastDirection + nextDirection * movementStepsInUnits;
			} else if (!HasQueuedMovements) {
				Vector3 currentPosition = new Vector3(_currentPosition.x, _transform.position.y, _currentPosition.z);
				newPosition = currentPosition + (nextDirection * movementStepsInUnits);
			}

			// If it's a tree ahead, don't queue the movement
			if (IsNextPositionValid(newPosition)) {
				_queuedMovements.Add(newPosition);	

				// If the first direction is the same as the newly added, call SetupNewMovement
				if (NextDirection.Equals(newPosition)) {
					SetupNewMovement();
				}				
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
