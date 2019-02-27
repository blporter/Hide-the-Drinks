using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MalbersAnimations;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	[SerializeField] private float _dodgeForce = 5.0f;
	[SerializeField] private float _jumpForce = 5.0f;
	[SerializeField] private float _speedMultiplier = 1.8f;


	private bool _canDodge = true;
	private float _playerSpeed;
	private Vector3 _movement;
	private Rigidbody _playerRigidbody;
	private Animator _playerAnimator;

	private const float DodgeTimer = 1f;
	private const float Acceleration = 6f;
	private const float MaxSpeed = 2f;
	private const float TurningSpeed = 15f;

	// Use this for initialization
	void Start() {
		_playerRigidbody = GetComponent<Rigidbody>();
		_playerAnimator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update() {
		_movement = Vector3.zero;
		_movement.x = Input.GetAxisRaw("Horizontal");
		_movement.z = Input.GetAxisRaw("Vertical");

		Debug.DrawLine(transform.position,
			new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z), Color.cyan, 1.0f);

		// Jump
		if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) {
			Jump();
		}

		// Dash
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
	}

	private void Jump() {
		_playerAnimator.SetBool("Jump", true);
		_playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
		_playerAnimator.stabilizeFeet = true;
	}

	private bool IsGrounded() {
		RaycastHit groundHitInfo;
//		return Physics.SphereCast(new Ray(_collider.bounds.center, Vector3.down), 0.2f, _collider.bounds.extents.y + 0.1f);
//		return !Physics.SphereCast(transform.position, _collider.size.x * _collider.size.z / 2, Vector3.down,
//			out groundHitInfo,
//			_collider.size.y);

		if (!Physics.Raycast(
			new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z),
			new Vector3(transform.position.x, transform.position.y - 1.0f, transform.position.z),
			out groundHitInfo,
			1.0f)) return false;
		
		var debugString = "Distance: " + groundHitInfo.distance +
		                  "\nCollider: " + groundHitInfo.collider +
		                  "\nTransform: " + groundHitInfo.transform;
		Debug.Log(debugString);

		if (!groundHitInfo.collider.CompareTag("Player")) {
			return groundHitInfo.distance < 1.0f;
		}

		return false;
	}

	private void TryDodge() {
		if (!_canDodge) return;
		if (!(_playerSpeed > 0f)) return;
		Dodge();
		StartCoroutine(DashCooldown());
	}

	private void Dodge() {
		_playerAnimator.SetBool("Dash", true);
		Vector3 dashMovement = new Vector3(_movement.x * _dodgeForce, 0f, _movement.z * _dodgeForce);
		_playerRigidbody.AddForce(dashMovement, ForceMode.VelocityChange);
	}

	private IEnumerator DashCooldown() {
		_canDodge = false;
		yield return new WaitForSeconds(DodgeTimer);
		_canDodge = true;
	}
}