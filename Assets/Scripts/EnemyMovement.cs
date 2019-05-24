using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {
	[SerializeField] private Transform target;

	private NavMeshAgent _enemyAgent;
	private bool _isChasing;
	private bool _isSearching;
	private float _currentTime;
	private Vector3 _currentDestination;
	private Vector3 _currentRotation;

	public float pauseLength = 3f;
	public float chaseSpeed = 2.5f;
	public float wanderSpeed = 1f;

	private const float LineOfSight = 80.0f;
	private const float TurningSpeed = 10f;

	void Start() {
		_enemyAgent = GetComponent<NavMeshAgent>();
		_currentDestination = transform.position;
	}

	void Update() {
		RaycastHit hit;
		var t = transform;
		var up = t.up;

		Vector3 direction = target.position - t.position;
		float angle = Vector3.Angle(direction, t.forward);

		Debug.DrawRay(transform.position + up * 0.9f, direction - up * 0.75f, Color.green);

		if (Physics.Raycast(t.position + up * 0.9f, direction - up * 0.75f, out hit)) {
			if (hit.transform == target && angle < LineOfSight) {
				// found player, begin chasing
				_isChasing = true;
				_isSearching = false;
				Debug.DrawRay(transform.position + up * 0.9f, direction - up * 0.75f, Color.red);
				Chase();
			}
			else {
				// lost line of sight of player, begin searching
				if (_isChasing) {
					_isChasing = false;
					_isSearching = true;
					Debug.DrawRay(transform.position + up * 0.9f, direction - up * 0.75f,
						Color.yellow);
					Search();
				}
			}
		}

		// wandering, reached patrol point.
		if (ReachedDestination(_currentDestination, 0.1f) && !_isChasing) {
			if (_isSearching) {
				_isSearching = false;
				LookAround();
			}

			if (_currentTime < 0.1f) {
				_currentTime = Time.time;
			}

			// wander to new patrol point.
			if (Time.time - _currentTime >= pauseLength) {
				_currentTime = 0f;
				Wander();
			}
		}
	}

	#region Movement Patterns

	private void Chase() {
		_enemyAgent.speed = chaseSpeed;
		var position = target.position;
		// Follow player
		_currentDestination = new Vector3(position.x, 0f, position.z);
		_enemyAgent.SetDestination(_currentDestination);
	}

	private void Wander() {
		_enemyAgent.speed = wanderSpeed;
		// Pick a new patrol spot
		_currentDestination = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-4.4f, 4.4f));
		_enemyAgent.SetDestination(_currentDestination);
	}

	private void Search() {
		Vector3 targetDirection = target.position;

		// Head towards last sighting of player
		_currentDestination = new Vector3(targetDirection.x, 0f, targetDirection.z);
		_enemyAgent.SetDestination(_currentDestination);
	}

	private void LookAround() {
		Vector3 targetDirection = target.position;
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

		// Look at last sighting of player
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, TurningSpeed * Time.deltaTime);
	}

	private bool ReachedDestination(Vector3 location, float distance) {
		return Vector3.Distance(transform.position, location) < distance;
	}

	#endregion
}