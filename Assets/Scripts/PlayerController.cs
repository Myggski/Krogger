using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FG {
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerController : MonoBehaviour {
		[SerializeField]
		public float _jumpForce = 50.0f;
		[SerializeField]
		private float _movementSpeed = 5.0f;
		[SerializeField]
		private float _movementStepsInUnits = 1f;
		[SerializeField]
		private Animator _animator;

		[SerializeField] private FloatEvent _jumpEvent;

		private bool _jumping = false;
		private Vector3 _nextDirection;
		private float _rotationAngle = 0f;

		private List<string> _keysPressed;
		private List<Vector3> _queuedMovements;
		private Vector3 _currentPosition;
		private Rigidbody _body;
		
		private Vector3 NextDirection => _queuedMovements.Any() ? _queuedMovements[0] : Vector3.zero;

		/// <summary>
		/// Gets called with UnityEvents by Input System
		/// </summary>
		/// <param name="value"></param>
		public void OnMovement(InputAction.CallbackContext value) {
			if (value.phase == InputActionPhase.Started) {
				QueueNewDirection(value.control.displayName);
			}
		}

		private void QueueNewDirection(string keyPressed) {
			_nextDirection = InputHelper.GetMovingDirectionByKey(keyPressed);

			if (_queuedMovements.Any() && _queuedMovements.Count < 3) {
				_queuedMovements.Add(_queuedMovements[_queuedMovements.Count - 1] + _nextDirection * _movementStepsInUnits);
			}
			else {
				_queuedMovements.Add( new Vector3(_currentPosition.x, transform.position.y, _currentPosition.z) +
				                      (_nextDirection * _movementStepsInUnits));
				
				_rotationAngle = Quaternion.Angle(transform.rotation,
					Quaternion.LookRotation((NextDirection - transform.position).normalized));
			}
		}

		/// <summary>
		/// Resets jump, moving direction and such before moving again
		/// </summary>
		private void ResetMovementInfo() {
			_jumping = false;
			_currentPosition = transform.position;

			if (_queuedMovements.Count > 0) {
				_rotationAngle = Quaternion.Angle(transform.rotation,
					Quaternion.LookRotation((NextDirection - transform.position).normalized));
			}
		}
		
		/// <summary>
		/// Set jump animation and adds jump force
		/// </summary>
		private void TryToJump() {
			if (_jumping) {
				return;
			}

			_jumping = true;
			_jumpEvent.Invoke(_movementSpeed / _movementStepsInUnits);

			_body.AddForce(0, _jumpForce, 0, ForceMode.Force);
			_animator.SetTrigger("Jump");
		}

		/// <summary>
		/// Moves and rotates the player to new position
		/// </summary>
		/// <param name="nextPosition"></param>
		private void MoveAndRotate() { ;
			TryToJump();
			
			transform.position = Vector3.MoveTowards(transform.position, NextDirection, (_movementSpeed * _queuedMovements.Count) * Time.deltaTime);
			transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation((NextDirection - _currentPosition).normalized), Time.deltaTime * (_rotationAngle / (1 / _movementSpeed * _movementStepsInUnits)));

			if (transform.position == NextDirection) {
				_queuedMovements.RemoveAt(0);
				ResetMovementInfo();
			}
		}

		/// <summary>
		/// Sets rigidbody, current position and animationspeed
		/// </summary>
		private void Setup() {
			_body = GetComponent<Rigidbody>();
			_currentPosition = transform.position;
			_animator.speed = _movementSpeed / _movementStepsInUnits;
			_queuedMovements = new List<Vector3>();
			_keysPressed = new List<string>();
		}

		/// <summary>
		/// Check if there are movement, and move if it's true
		/// </summary>
		private void CheckForMovement() {
			if (_queuedMovements.Count > 0) {
				MoveAndRotate();
			}
			else {
				ResetMovementInfo();
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
