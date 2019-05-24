using UnityEngine;

public class CameraMovement : MonoBehaviour {
	[SerializeField] private Transform player;
	[SerializeField] private Vector3 offset;
	
	private const float CameraFollowSpeed = 3f;

	// Update is called once per frame
	void Update() {
		Vector3 newPosition = player.position + offset;

		transform.position = Vector3.Lerp(transform.position, newPosition, CameraFollowSpeed * Time.deltaTime);
	}
}