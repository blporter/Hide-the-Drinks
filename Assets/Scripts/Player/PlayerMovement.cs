using System.Collections;
using UnityEngine;

namespace Player {
	public class PlayerMovement : MonoBehaviour {
		[SerializeField] private float _dodgeForce = 4.0f;
		[SerializeField] private float _jumpForce = 4.0f;
		[SerializeField] private float _speedMultiplier = 1.6f;
		[SerializeField] private float _turningSpeed = 5f;

		public LayerMask GroundLayer;

		private bool _canDodge = true;
		private float _playerSpeed;
		private float _distanceToGround;
		private float _jumpHeight;
		private Vector3 _movement;
		private Rigidbody _playerRigidbody;
		private Animator _playerAnimator;

		private const float DodgeTimer = 1f;
		private const float Acceleration = 6f;
		private const float MaxSpeed = 2f;

		void Start() {
			_playerRigidbody = GetComponent<Rigidbody>();
			_playerAnimator = GetComponent<Animator>();
			_jumpHeight = _jumpForce / 3.5f;
		}

		void Update() {
			_movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
//			_movement.x = Input.GetAxis("Horizontal");
//			_movement.z = Input.GetAxis("Vertical");

			_playerAnimator.SetFloat("Horizontal", _movement.x);

			HandleInput();
		}

		private void FixedUpdate() {
			HandleMovement();
			HandleFalling();
		}

		private void HandleMovement() {
			if (_movement != Vector3.zero) {
				_playerSpeed = _playerSpeed + Acceleration * Time.deltaTime;

				if (_playerSpeed > MaxSpeed) {
					_playerSpeed = MaxSpeed;
				}

				Quaternion targetRotation = Quaternion.LookRotation(_movement, Vector3.up);
				Quaternion newRotation =
					Quaternion.Lerp(_playerRigidbody.rotation, targetRotation, _turningSpeed * Time.deltaTime);

				_playerRigidbody.MovePosition(transform.position +
				                              _movement * Time.deltaTime * _playerSpeed * _speedMultiplier);
				_playerRigidbody.MoveRotation(newRotation);
				
				_playerAnimator.SetFloat("Speed", _playerSpeed);
			}
			else {
				_playerSpeed = 0f;
				_playerAnimator.SetFloat("Speed", 0f);
			}
		}

		#region Getters and Setters

		public void SetPlayerSpeed(float speed) {
			_playerSpeed = speed;
		}

		#endregion

		#region MidAir and Falling

		private void HandleFalling() {
			bool isGrounded = IsGrounded();

			// No need to do ground distance raycasting if we're grounded.
			if (isGrounded) {
				_distanceToGround = 0f;
				_playerAnimator.SetFloat("DistanceToGround", _distanceToGround);
				return;
			}

			float distanceToGround = GetDistanceToGround();

			_playerAnimator.SetFloat("DistanceToGround", distanceToGround);
		}

		private bool IsGrounded() {
			RaycastHit groundHitInfo;
			Vector3 position = transform.position;
			Vector3 direction = Vector3.down;
			float distance = 0.3f;

			Physics.SphereCast(position + Vector3.up * (0.2f + Physics.defaultContactOffset),
				0.2f - Physics.defaultContactOffset, direction, out groundHitInfo, distance,
				GroundLayer, QueryTriggerInteraction.Ignore);

			return groundHitInfo.collider != null;
		}

		private float GetDistanceToGround() {
			RaycastHit groundHitInfo;

			Vector3 position = transform.position;
			Vector3 direction = Vector3.down;
			float distance = 3.0f;

			Debug.DrawRay(position, direction, Color.green);

			Physics.Raycast(position + Vector3.up * (0.2f + Physics.defaultContactOffset),
				direction, out groundHitInfo, distance, GroundLayer, QueryTriggerInteraction.Ignore);

			if (groundHitInfo.collider == null) {
				_playerAnimator.SetBool("MidAir", true);
				_distanceToGround = 3.0f;
			}
			else if (groundHitInfo.distance >= _jumpHeight + 0.2f) {
				_playerAnimator.SetBool("MidAir", true);
				_distanceToGround = 2.0f;
			}
			else if (groundHitInfo.distance < _jumpHeight / 2) {
				_distanceToGround = 1.0f;
				_playerAnimator.SetBool("MidAir", false);
				// End jumps early if we're close enough to the ground
				_playerAnimator.SetBool("Jump", false);
			}

			return _distanceToGround;
		}

		#endregion

		#region Input Handling

		private void HandleInput() {
			// Jump
			if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) {
				Jump();
			}

			// Dodge
			if (Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded()) {
				TryDodge();
			}
		}

		private void Jump() {
			_playerAnimator.SetBool("Jump", true);
			_playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
		}

		private void TryDodge() {
			if (!_canDodge) return;
			if (!(_playerSpeed > 0f)) return;
			Dodge();
			StartCoroutine(DodgeCooldown());
		}

		private void Dodge() {
			_playerAnimator.SetBool("Dodge", true);
			Vector3 dodgeMovement = new Vector3(_movement.x * _dodgeForce, 0f, _movement.z * _dodgeForce);
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

/* TODO: adjustments to falling
 * 	- transition from landing to running/walking better
 * 	- end jump animation early if landed
 * 		(ex: jumping up onto a platform continues animation instead of landing on platform)
 * 	- handle slopes
 * TODO: adjustments to movement
 * 	- add turning motions instead of instant turning
 * 		(blend tree?)
 * 	- handle clipping through walls and such
 * 		(clip through the pub walls, clip up the props)
 * TODO: finish IK handling?
 * TODO: Idle rotation
 */