using UnityEngine;

public class CameraMovement : MonoBehaviour {
	[SerializeField] private Transform _player;
	[SerializeField] private Vector3 _offset;
	
	private const float CameraFollowSpeed = 3f;

	// Update is called once per frame
	void Update() {
		Vector3 newPosition = _player.position + _offset;

		transform.position = Vector3.Lerp(transform.position, newPosition, CameraFollowSpeed * Time.deltaTime);
	}
}