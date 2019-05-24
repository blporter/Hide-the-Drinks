using System.Collections;
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
		private const float Deceleration = 3f;
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
				_playerSpeed += Acceleration * Time.deltaTime;

				if (_playerSpeed > MaxSpeed) {
					_playerSpeed = MaxSpeed;
				}

				Quaternion targetRotation = Quaternion.LookRotation(_movement, Vector3.up);
				Quaternion newRotation =
					Quaternion.Lerp(_playerRigidbody.rotation, targetRotation, turningSpeed * Time.deltaTime);

				_playerRigidbody.MovePosition(transform.position +
				                              Time.deltaTime * _playerSpeed * speedMultiplier * _movement);
				_playerRigidbody.MoveRotation(newRotation);
			}
			else {
				// This makes transitioning to a stop more smooth
				if (_playerSpeed > 0.1f) {
					_playerSpeed -= Deceleration * Time.deltaTime;
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

		#endregion

		#region MidAir and Falling

		private void HandleFalling() {
			bool isGrounded = IsGrounded();

			// No need to do ground distance raycasting if we're grounded.
			if (isGrounded) {
				_distanceToGround = 0;
				_playerAnimator.SetInteger(DistanceToGroundAnim, _distanceToGround);
				return;
			}

			int distanceToGround = GetDistanceToGround();

			_playerAnimator.SetInteger(DistanceToGroundAnim, distanceToGround);
		}

		private bool IsGrounded() {
			RaycastHit groundHitInfo;
			Vector3 position = transform.position;
			Vector3 direction = Vector3.down;
			float distance = 0.3f;

			Physics.SphereCast(position + Vector3.up * (0.2f + Physics.defaultContactOffset),
				0.2f - Physics.defaultContactOffset, direction, out groundHitInfo, distance,
				groundLayer, QueryTriggerInteraction.Ignore);

			return ReferenceEquals(groundHitInfo.collider, null);
		}

		private int GetDistanceToGround() {
			RaycastHit groundHitInfo;

			Vector3 position = transform.position;
			Vector3 direction = Vector3.down;
			float distance = 3.0f;

			Debug.DrawRay(position, direction, Color.green);

			Physics.Raycast(position + Vector3.up * (0.2f + Physics.defaultContactOffset),
				direction, out groundHitInfo, distance, groundLayer, QueryTriggerInteraction.Ignore);

			if (ReferenceEquals(groundHitInfo.collider, null)) {
				_playerAnimator.SetBool(MidAirAnim, true);
				_distanceToGround = 3;
			}
			else if (groundHitInfo.distance >= _jumpHeight + 0.2f) {
				_playerAnimator.SetBool(MidAirAnim, true);
				_distanceToGround = 2;
			}
			else if (groundHitInfo.distance < _jumpHeight / 2) {
				_distanceToGround = 1;
				_playerAnimator.SetBool(MidAirAnim, false);
				// End jumps early if we're close enough to the ground
				_playerAnimator.SetBool(JumpAnim, false);
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

/* TODO: adjustments to falling
 * 	- transition from landing to running/walking better
 * 	- end jump animation early if landed
 * 		(ex: jumping up onto a platform continues animation instead of landing on platform)
 * 	- handle slopes
 * TODO: adjustments to movement
 * 	- fix turning motions
 *  - handle delayed movement
 *  - handle sudden stops
 * 	- handle clipping through walls and such
 * 		(clip through the pub walls, clip up the props)
 * TODO: finish IK handling?
 * TODO: Idle rotation
 */