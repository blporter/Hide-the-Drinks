using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MalbersAnimations;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	[SerializeField] private float _dodgeForce = 5.0f;
	[SerializeField] private float _jumpForce = 5.0f;
	[SerializeField] private float _speedMultiplier = 1.8f;

	public LayerMask GroundLayer;

	private bool _canDodge = true;
	private float _playerSpeed;
	private Vector3 _movement;
	private Rigidbody _playerRigidbody;
	private Animator _playerAnimator;

	private const float DodgeTimer = 1f;
	private const float Acceleration = 6f;
	private const float MaxSpeed = 2f;
	private const float TurningSpeed = 15f;

	void Start() {
		_playerRigidbody = GetComponent<Rigidbody>();
		_playerAnimator = GetComponent<Animator>();
	}

	void Update() {
		_movement = Vector3.zero;
		_movement.x = Input.GetAxisRaw("Horizontal");
		_movement.z = Input.GetAxisRaw("Vertical");

		// Jump
		if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) {
			Jump();
		}

		// Dodge
		if (Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded()) {
			TryDodge();
		}
	}

	private void FixedUpdate() {
		if (_movement != Vector3.zero) {
			_playerSpeed = _playerSpeed + Acceleration * Time.deltaTime;
			if (_playerSpeed > MaxSpeed) {
				_playerSpeed = MaxSpeed;
			}

			Quaternion targetRotation = Quaternion.LookRotation(_movement, Vector3.up);
			Quaternion newRotation =
				Quaternion.Lerp(_playerRigidbody.rotation, targetRotation, TurningSpeed * Time.deltaTime);

			_playerRigidbody.MovePosition(transform.position +
			                              _movement * Time.deltaTime * _playerSpeed * _speedMultiplier);
			_playerRigidbody.MoveRotation(newRotation);
			_playerAnimator.SetFloat("Speed", _playerSpeed);
		}
		else {
			_playerSpeed = 0f;
			_playerAnimator.SetFloat("Speed", 0f);
		}

//		Debug.Log(IsGrounded());

		// TODO: fix falling detection.
		// Check if falling
//		if (!IsGrounded() && !_playerAnimator.GetBool("Jump") && !_playerAnimator.GetBool("Dodge")) {
//			_playerAnimator.SetBool("MidAir", true);
//		}
//		else {
//			_playerAnimator.SetBool("Midair", false);
//		}
	}

	private void Jump() {
		_playerAnimator.SetBool("Jump", true);
		_playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
	}

	private bool IsGrounded() {
		RaycastHit groundHitInfo;

		Vector3 position = transform.position;
		Vector3 direction = Vector3.down;
		float distance = 0.3f;

		Debug.DrawRay(position, direction, Color.green);

		Physics.SphereCast(position + Vector3.up * (0.2f + Physics.defaultContactOffset),
			0.2f - Physics.defaultContactOffset, direction, out groundHitInfo, distance,
			GroundLayer, QueryTriggerInteraction.Ignore);

		return groundHitInfo.collider != null;
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
}