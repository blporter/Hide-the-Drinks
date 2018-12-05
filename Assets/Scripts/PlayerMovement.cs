using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	private float _moveHorizontal;
	private float _moveVertical;
	private float _distanceToGround;
	private Vector3 _movement;
	private bool _canDash = true;
	private const float DashTimer = 1f;
	private const float MovementSpeed = 3f;
	private const float TurningSpeed = 20f;
	private Rigidbody _playerRigidbody;

	// Use this for initialization
	void Start() {
		_playerRigidbody = GetComponent<Rigidbody>();
		_distanceToGround = GetComponent<Collider>().bounds.extents.y;
	}

	// Update is called once per frame
	void Update() {
		_moveHorizontal = Input.GetAxisRaw("Horizontal");
		_moveVertical = Input.GetAxisRaw("Vertical");

		_movement = new Vector3(_moveHorizontal, 0.0f, _moveVertical);

		// Jump
		if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) {
			Jump();
		}

		// Dash
		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			TryDash();
		}
	}

	private void FixedUpdate() {
		if (_movement != Vector3.zero) {
			Quaternion targetRotation = Quaternion.LookRotation(_movement, Vector3.up);
			Quaternion newRotation =
				Quaternion.Lerp(_playerRigidbody.rotation, targetRotation, TurningSpeed * Time.deltaTime);

			_playerRigidbody.MoveRotation(newRotation);
			_playerRigidbody.MovePosition(transform.position + transform.forward * Time.deltaTime * MovementSpeed);
		}
	}

	private void Jump() {
		_playerRigidbody.AddForce(0f, 250f, 0f);
	}

	private bool IsGrounded() {
		return Physics.Raycast(transform.position, -Vector3.up, _distanceToGround + 0.1f);
	}

	private void TryDash() {
		if (!_canDash) return;
		Dash();
		StartCoroutine(DashCooldown());
	}

	private void Dash() {
		_playerRigidbody.AddForce(_movement.x * 200f, 0f, _movement.z * 200f);
	}

	private IEnumerator DashCooldown() {
		_canDash = false;
		yield return new WaitForSeconds(DashTimer);
		_canDash = true;
	}
}