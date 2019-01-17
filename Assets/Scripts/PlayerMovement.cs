using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
//	private float _distanceToGround;
	private bool _canDash = true;
	private float _playerSpeed;
	private Vector3 _movement;
	private Rigidbody _playerRigidbody;

	private Animator _playerAnimator;
//	private BoxCollider _collider;

	private const float DashTimer = 1f;
	private const float DashForce = 4f;
	private const float JumpForce = 4f;
	private const float Acceleration = 6f;
	private const float MaxSpeed = 2f;
	private const float TurningSpeed = 15f;

	// Use this for initialization
	void Start() {
		_playerRigidbody = GetComponent<Rigidbody>();
		_playerAnimator = GetComponent<Animator>();
//		_collider = GetComponent<BoxCollider>();
//		_distanceToGround = GetComponent<Collider>().bounds.extents.y;
	}

	// Update is called once per frame
	void Update() {
		_movement = Vector3.zero;
		_movement.x = Input.GetAxisRaw("Horizontal");
		_movement.z = Input.GetAxisRaw("Vertical");

		// Jump
		if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) {
			Jump();
		}

		// Dash
		if (Input.GetKeyDown(KeyCode.LeftShift) && IsGrounded()) {
			TryDash();
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

			_playerRigidbody.MovePosition(transform.position + _movement * Time.deltaTime * _playerSpeed);
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
		_playerRigidbody.AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
		_playerAnimator.stabilizeFeet = true;

	}

	private bool IsGrounded() {
		RaycastHit groundHitInfo = new RaycastHit();
//		return Physics.SphereCast(new Ray(_collider.bounds.center, Vector3.down), 0.2f, _collider.bounds.extents.y + 0.1f);
//		return !Physics.SphereCast(transform.position, _collider.size.x * _collider.size.z / 2, Vector3.down,
//			out groundHitInfo,
//			_collider.size.y);
//		return Physics.CheckBox(_groundCollider.bounds.center,
//			new Vector3(_groundCollider.bounds.center.x, _groundCollider.bounds.min.y - 0.1f,
//				_groundCollider.bounds.center.z));
//		return Physics.Raycast(transform.position, -Vector3.up, _distanceToGround + 0.1f);
		return true;
	}

	private void TryDash() {
		if (!_canDash) return;
		Dash();
		StartCoroutine(DashCooldown());
	}

	private void Dash() {
		_playerAnimator.SetBool("Dash", true);
		Vector3 dashMovement = new Vector3(_movement.x * DashForce, 0f, _movement.z * DashForce);
		_playerRigidbody.AddForce(dashMovement, ForceMode.VelocityChange);
	}

	private IEnumerator DashCooldown() {
		_canDash = false;
		yield return new WaitForSeconds(DashTimer);
		_canDash = true;
	}
}