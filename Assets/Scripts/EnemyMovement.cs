using System;
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
		float distanceToTarget = CalculateDistance(_target);

		if (distanceToTarget < 4f) {
			_enemyAgent.SetDestination(_target.position);
		}
		else {
			_enemyAgent.SetDestination(_startingLocation);
		}
	}

	private float CalculateDistance(Transform target) {
		Vector3 currentPosition = transform.position;
		Vector3 targetPosition = target.position;

		float x = Math.Abs(currentPosition.x - targetPosition.x);
		float z = Math.Abs(currentPosition.z - targetPosition.z);

		float distance = Mathf.Sqrt((float) Math.Pow(x, 2) + (float) Math.Pow(z, 2));

		return distance;
	}
}