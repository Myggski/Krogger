using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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

		private Coroutine _speedUpCoroutine;

		private float _horizontalMovement = 0f;
		private float _verticalMovement = 0f;
		private bool _jumping = false;
		private bool _isKeyPressedDown = false;
		private bool _isCurrentlyRunning = false;
		private bool _inputStarted = false;
		private Vector3 _nextDirection;
		private float _rotationAngle = 0f;

		private List<Vector3> _queuedMovements;
		private Vector3 _currentPosition;
		private Rigidbody _body;

		private bool IsMovingHorizontal => !_horizontalMovement.Equals(0f);
		private bool IsMovingVertical => !_verticalMovement.Equals(0f);
		private bool IsMoving => IsMovingHorizontal || IsMovingVertical;
		private bool HasNoKeysPressedDown => !Keyboard.current.anyKey.isPressed;
		private Vector3 NextDirection => _queuedMovements.Any() ? _queuedMovements[0] : Vector3.zero;

		/// <summary>
		/// Gets called with UnityEvents by Input System
		/// </summary>
		/// <param name="value"></param>
		public void OnMovement(InputAction.CallbackContext value) {
			Vector2 inputMovement = value.ReadValue<Vector2>();

			_horizontalMovement = inputMovement.x;
			_verticalMovement = inputMovement.y;

			if (value.started) {
				QueueNewDirection();
			}
		}

		private void QueueNewDirection() {
			_nextDirection = Vector3.zero;

			if (IsMovingHorizontal) {
				_nextDirection.x = _horizontalMovement;
			} else if (IsMovingVertical) {
				_nextDirection.z = _verticalMovement;
			}

			if (_queuedMovements.Any() && _queuedMovements.Count < 3) {
				_queuedMovements.Add(_queuedMovements[_queuedMovements.Count - 1] + (_nextDirection * _movementStepsInUnits));
			}
			else {
				_queuedMovements.Add(new Vector3(_currentPosition.x, transform.position.y, _currentPosition.z) +
				                     (_nextDirection * _movementStepsInUnits));
			}
			
			_rotationAngle = IsMoving ? Quaternion.Angle(transform.rotation, Quaternion.LookRotation((NextDirection - transform.position).normalized)) : 0;
		}

		/// <summary>
		/// Resets jump, moving direction and such before moving again
		/// </summary>
		private void ResetMovementInfo() {
			_jumping = false;
			_currentPosition = transform.position;
		}

		/// <summary>
		/// Sets the moving direction and rotation angle
		/// </summary>
		private void SetupMovementInfo() {
			ResetMovementInfo();

			if (IsMoving) {
				_isKeyPressedDown = true;
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
			_body.AddForce(0, this._jumpForce, 0, ForceMode.Force);
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
				SetupMovementInfo();
			}
		}

		/// <summary>
		/// Sets rigidbody, current position and animationspeed
		/// </summary>
		private void Setup() {
			_body = GetComponent<Rigidbody>();
			_currentPosition = this.transform.position;
			_animator.speed = _movementSpeed / _movementStepsInUnits;
			_queuedMovements = new List<Vector3>();
		}

		/// <summary>
		/// Check if there are movement, and move if it's true
		/// </summary>
		private void CheckForMovement() {
			if (!_isKeyPressedDown) {
				_isCurrentlyRunning = false;
				SetupMovementInfo();
			} else if (_queuedMovements.Count > 0) {
				MoveAndRotate();
				_isCurrentlyRunning = true;
			} else if (HasNoKeysPressedDown) {
				_isKeyPressedDown = false;
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
