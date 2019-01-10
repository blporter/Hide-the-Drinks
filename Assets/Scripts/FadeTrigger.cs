using UnityEngine;

public class FadeTrigger : MonoBehaviour {

	[SerializeField] private GameObject _object;

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			_object.SetActive(false);
		}
	}
	
	private void OnTriggerExit(Collider other) {
		_object.SetActive(true);
	}
}
