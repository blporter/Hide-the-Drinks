using UnityEngine;

namespace Player {
	public class PlayerHealth : MonoBehaviour {
		public bool alive;

		private void Start() {
			alive = true;
		}

		private void OnTriggerEnter(Collider other) {
			if (other.CompareTag("Enemy") && alive) {
				alive = false;
			}
		}
	}
}