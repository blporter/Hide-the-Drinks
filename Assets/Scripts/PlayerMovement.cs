using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float _moveHorizontal;
    private float _moveVertical;
    private Vector3 _movement;
    private const float MovementSpeed = 3f;
    private const float TurningSpeed = 20f;
    private Rigidbody _playerRigidbody;
    private Boolean _isJumping = false;

    // Use this for initialization
    void Start()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _moveHorizontal = Input.GetAxisRaw("Horizontal");
        _moveVertical = Input.GetAxisRaw("Vertical");

        _movement = new Vector3(_moveHorizontal, 0.0f, _moveVertical);

        if (Input.GetKeyDown(KeyCode.Space) && !_isJumping)
        {
            _playerRigidbody.AddForce(0f, 200f, 0f);
            _isJumping = true;
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _playerRigidbody.AddForce(_movement.x * 200f, 0f, _movement.z * 200f);
        }
    }

    private void FixedUpdate()
    {
        if (_movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_movement, Vector3.up);
            Quaternion newRotation =
                Quaternion.Lerp(_playerRigidbody.rotation, targetRotation, TurningSpeed * Time.deltaTime);

            _playerRigidbody.MoveRotation(newRotation);
            _playerRigidbody.MovePosition(transform.position + transform.forward * Time.deltaTime * MovementSpeed);
        }

        if (_isJumping)
        {
            GameObject ground = GameObject.FindGameObjectWithTag("PlayerCar").gameObject;
            ground.
        }
    }
}