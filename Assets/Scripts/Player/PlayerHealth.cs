using UnityEngine;

namespace Player {
	public class PlayerHealth : MonoBehaviour {
		public bool Alive;

		void Start() {
			Alive = true;
		}

		private void OnTriggerEnter(Collider other) {
			if (other.CompareTag("Enemy") && Alive) {
				Alive = false;
			}
		}
	}
}