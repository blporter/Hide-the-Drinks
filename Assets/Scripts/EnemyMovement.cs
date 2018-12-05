using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {
	[SerializeField] private Transform _target;

	private NavMeshAgent _enemyAgent;
	private bool _isChasing;
	private float _currentTime;
	private Vector3 _currentDestination;
	private readonly Vector3 _startingLocation = new Vector3(-1f, 0.4f, 4f);
	private const float PauseLength = 2f;

	// Use this for initialization
	void Start() {
		_enemyAgent = GetComponent<NavMeshAgent>();
		_enemyAgent.Warp(_startingLocation);
		Wander();
	}

	// Update is called once per frame
	void Update() {
		RaycastHit hit = new RaycastHit();
		Vector3 direction = _target.position - transform.position;
		float angle = Vector3.Angle(direction, transform.forward);

		if (Physics.Raycast(transform.position, direction, out hit)) {
			if (hit.transform == _target && angle < 60.0f) {
				_isChasing = true;
				Chase();
			}
			else {
				if (_isChasing) {
					_isChasing = false;
				}
			}
		}

		if (ReachedDestination(_currentDestination)) {
			if (_currentTime < 0.1f) {
				_currentTime = Time.time;
			}

			if (Time.time - _currentTime >= PauseLength) {
				_currentTime = 0f;
				Wander();
			}
		}
	}

	private void Chase() {
		_enemyAgent.speed = 2.3f;
		_currentDestination = _target.position;
		_enemyAgent.SetDestination(_currentDestination);
	}

	private void Wander() {
		_enemyAgent.speed = 0.7f;
		_currentDestination = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-4.4f, 4.4f));
		_enemyAgent.SetDestination(_currentDestination);
	}

	private void ReturnHome() {
		_enemyAgent.SetDestination(_startingLocation);
	}

	private bool ReachedDestination(Vector3 location) {
		return Vector3.Distance(transform.position, location) < 1f;
	}
}