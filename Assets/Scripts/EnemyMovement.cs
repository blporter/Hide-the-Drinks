using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {
	[SerializeField] private Transform _target;

	private NavMeshAgent _enemyAgent;
	private float _distanceToChase;
	private readonly Vector3 _startingLocation = new Vector3(-1f, 0.4f, 4f);

	// Use this for initialization
	void Start() {
		_enemyAgent = GetComponent<NavMeshAgent>();
		_enemyAgent.Warp(_startingLocation);
	}

	// Update is called once per frame
	void Update() {
		RaycastHit hit = new RaycastHit();
		Vector3 direction = _target.position - transform.position;
		float angle = Vector3.Angle(direction, transform.forward);

		if (Physics.Raycast(transform.position, direction, out hit)) {
			if (hit.transform == _target && angle < 60.0f) {
				_enemyAgent.SetDestination(_target.position);
			}
			else {
				_enemyAgent.SetDestination(_startingLocation);
			}
		}
	}
}