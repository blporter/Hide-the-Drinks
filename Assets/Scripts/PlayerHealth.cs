using UnityEngine;

public class PlayerHealth : MonoBehaviour {
	public bool Alive;

	// Use this for initialization
	void Start() {
		Alive = true;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Enemy") && Alive) {
			Alive = false;
		}
	}
}