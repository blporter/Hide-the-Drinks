﻿using System.Collections;
using UnityEngine;

namespace Player {
	public class PlayerMovement : MonoBehaviour {
		[SerializeField] private float dodgeForce = 5.0f;
		[SerializeField] private float jumpForce = 4.0f;
		[SerializeField] private float speedMultiplier = 1.6f;
		[SerializeField] private float turningSpeed = 7f;

		public LayerMask groundLayer;

		private bool _canDodge = true;
		private float _playerSpeed;
		private float _jumpHeight;
		private int _distanceToGround;
		private Vector3 _movement;
		private Vector3 _lastMovement;
		private Rigidbody _playerRigidbody;
		private Animator _playerAnimator;

		private static readonly int JumpAnim = Animator.StringToHash("Jump");
		private static readonly int HorizAnim = Animator.StringToHash("Horizontal");
		private static readonly int SpeedAnim = Animator.StringToHash("Speed");
		private static readonly int DistanceToGroundAnim = Animator.StringToHash("DistanceToGround");
		private static readonly int MidAirAnim = Animator.StringToHash("MidAir");
		private static readonly int DodgeAnim = Animator.StringToHash("Dodge");

		private const float DodgeTimer = 1f;
		private const float Acceleration = 6f;
		private const float Deceleration = 4f;
		private const float MaxSpeed = 2f;

		void Start() {
			_playerRigidbody = GetComponent<Rigidbody>();
			_playerAnimator = GetComponent<Animator>();
			_jumpHeight = jumpForce / 3.5f;
		}

		void Update() {
			_movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

			_playerAnimator.SetFloat(HorizAnim, _movement.x);

			HandleInput();
		}

		private void FixedUpdate() {
			HandleMovement();
			HandleFalling();
		}

		private void HandleMovement() {
			if (_movement != Vector3.zero) {
				_lastMovement = _movement;
				_playerSpeed += Acceleration * Time.deltaTime;

				if (_playerSpeed > MaxSpeed) {
					_playerSpeed = MaxSpeed;
				}

				SetMovement(_movement, true);
			}
			else {
				// This makes transitioning to a stop more smooth
				if (_playerSpeed > 0.1f) {
					_playerSpeed -= Deceleration * Time.deltaTime;

					SetMovement(_lastMovement, false);
				}
				else {
					_playerSpeed = 0f;
				}
			}

			_playerAnimator.SetFloat(SpeedAnim, _playerSpeed);
		}

		#region Getters and Setters

		public void SetPlayerSpeed(float speed) {
			_playerSpeed = speed;
		}

		public float GetPlayerSpeed() {
			return _playerSpeed;
		}

		private void SetMovement(Vector3 movement, bool accelerate) {
			Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
			Quaternion newRotation =
				Quaternion.Lerp(_playerRigidbody.rotation, targetRotation, turningSpeed * Time.deltaTime);

			if (accelerate) {
				_playerRigidbody.MovePosition(transform.position +
				                              Time.deltaTime * _playerSpeed * speedMultiplier * movement);
			}
			else {
				_playerRigidbody.MovePosition(transform.position +
				                              Time.deltaTime * _playerSpeed * speedMultiplier * movement / 3);
			}

			_playerRigidbody.MoveRotation(newRotation);
		}

		#endregion

		#region MidAir and Falling

		private void HandleFalling() {
			GetDistanceToGround();
			bool isGrounded = IsGrounded();

			if (isGrounded) {
				_distanceToGround = 0;
			}

			_playerAnimator.SetInteger(DistanceToGroundAnim, _distanceToGround);
		}

		private bool IsGrounded() {
			RaycastHit groundHitInfo;
			Vector3 position = transform.position;
			Vector3 direction = Vector3.down;
			float distance = 0.3f;

			Physics.SphereCast(position + Vector3.up * (0.2f + Physics.defaultContactOffset),
				0.2f - Physics.defaultContactOffset, direction, out groundHitInfo, distance,
				groundLayer, QueryTriggerInteraction.Ignore);

			return !ReferenceEquals(groundHitInfo.collider, null) && _distanceToGround < 2;
		}

		private void GetDistanceToGround() {
			RaycastHit groundHitInfo;

			Vector3 position = transform.position;
			Vector3 direction = Vector3.down;
			float distance = 3.0f;

			Debug.DrawRay(position, direction, Color.green);
			Physics.Raycast(position + Vector3.up * (0.2f + Physics.defaultContactOffset),
				direction, out groundHitInfo, distance, groundLayer, QueryTriggerInteraction.Ignore);

			// High Fall
			if (ReferenceEquals(groundHitInfo.collider, null)) {
				_playerAnimator.SetBool(MidAirAnim, true);
				_distanceToGround = 3;
			}
			// Mid-range Fall
			else if (groundHitInfo.distance >= _jumpHeight + 0.2f) {
				_playerAnimator.SetBool(MidAirAnim, true);
				_distanceToGround = 2;
			}
			// Landing
			else if (groundHitInfo.distance < _jumpHeight / 2) {
				_distanceToGround = 1;
				_playerAnimator.SetBool(MidAirAnim, false);
			}
		}

		#endregion

		#region Input Handling

		private void HandleInput() {
			// Jump
			if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !_playerAnimator.GetBool(DodgeAnim)) {
				Jump();
			}

			// Dodge
			if (Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded()) {
				TryDodge();
			}
		}

		private void Jump() {
			_playerAnimator.SetBool(JumpAnim, true);
			_playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
		}

		private void TryDodge() {
			if (!_canDodge) return;
			if (!(_playerSpeed > 0f)) return;
			Dodge();
			StartCoroutine(DodgeCooldown());
		}

		private void Dodge() {
			_playerAnimator.SetBool(DodgeAnim, true);
			Vector3 dodgeMovement = new Vector3(_movement.x * dodgeForce, 0f, _movement.z * dodgeForce);
			_playerRigidbody.AddForce(dodgeMovement, ForceMode.VelocityChange);
		}

		private IEnumerator DodgeCooldown() {
			_canDodge = false;
			yield return new WaitForSeconds(DodgeTimer);
			_canDodge = true;
		}

		#endregion
	}
}

/* TODO: adjustments to movement
 * 	- fix turning motions
 *  - remove input while falling and add momentum from jump?
 *  - add more of a delay to movement on landing from a high fall?
 * 	- handle clipping through walls and such
 * 		(clip through the pub walls, clip up the props)
 * TODO: finish IK handling
 */