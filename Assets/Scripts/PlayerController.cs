using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using UnityEditor.U2D;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace FG {
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerController : MonoBehaviour {
		private float _horizontalMovement = 0f;
		private float _verticalMovement = 0f;
		private bool _jumping = false;
		private bool _hasMoved = false;

		private Vector3 _nextDir;
		private Rigidbody _body;

		public float JumpForce = 50.0f;
		public float Speed = 5.0f;
		public float SpeedRotation = 1000.0f;
		public Vector3 CurrentPosition;
		public Animator Animator;

		private bool IsMovingHorizontal => !_horizontalMovement.Equals(0f);
		private bool IsMovingVertical => !_verticalMovement.Equals(0f);
		private bool IsMoving => IsMovingHorizontal || IsMovingVertical;
		private bool HasNoKeysPressedDown => !Input.anyKey;

		private void ResetMovementInfo() {
			_jumping = false;
			_nextDir = Vector3.zero;
			CurrentPosition = transform.position;
		}

		private void SetMovement() {
			_horizontalMovement = Input.GetAxisRaw("Horizontal");
			_verticalMovement = Input.GetAxisRaw("Vertical");
			
			if (IsMovingHorizontal) {
				_nextDir.x = _horizontalMovement;
			} else if (IsMovingVertical) {
				_nextDir.z = _verticalMovement;
			}

			if (IsMoving) {
				_hasMoved = true;
			}
		}
		
		private void TryToJump() {
			if (this._jumping) {
				return;
			}

			_jumping = true;
			_body.AddForce(0, this.JumpForce, 0, ForceMode.Force);
			Animator.SetTrigger("Jump");
		}
		
		private void Awake() {
			_body = GetComponent<Rigidbody>();
			CurrentPosition = this.transform.position;
		}

		private void Update() {
			Vector3 nextPosition = new Vector3(this.CurrentPosition.x, this.transform.position.y, this.CurrentPosition.z) +this
				._nextDir;

			if (transform.position != nextPosition) {
				_hasMoved = true;
				TryToJump();
				
				transform.position = Vector3.MoveTowards(transform.position, nextPosition, Speed * Time.deltaTime);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_nextDir), SpeedRotation * Time.deltaTime);
			}
			else if (!_hasMoved) {
				ResetMovementInfo();
				SetMovement();
			} else if (HasNoKeysPressedDown) {
				_hasMoved = false;
			}
		}
	}
}
