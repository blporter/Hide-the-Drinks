﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {
	[SerializeField] private Transform _target;

	private NavMeshAgent _enemyAgent;
	private bool _isChasing;
	private bool _isSearching;
	private float _currentTime;
	private Vector3 _currentDestination;
	private Vector3 _currentRotation;

	private const float LineOfSight = 80.0f;
	private const float PauseLength = 3f;
	private const float ChaseSpeed = 2.5f;
	private const float WanderSpeed = 1f;
	private const float TurningSpeed = 10f;

	// Use this for initialization
	void Start() {
		_enemyAgent = GetComponent<NavMeshAgent>();
	}

	// Update is called once per frame
	void Update() {
		RaycastHit hit = new RaycastHit();
		Vector3 direction = _target.position - transform.position;
		float angle = Vector3.Angle(direction, transform.forward);

		if (Physics.Raycast(transform.position, direction, out hit)) {
			if (hit.transform == _target && angle < LineOfSight) {
				// found player, begin chasing
				_isChasing = true;
				_isSearching = false;
				Chase();
			}
			else {
				// lost line of sight of player, begin searching
				if (_isChasing) {
					_isChasing = false;
					_isSearching = true;
					LookAround();
				}
			}
		}

		// wandering, reached patrol point.
		if (ReachedDestination(_currentDestination, 1f) && !_isSearching) {
			if (_currentTime < 0.1f) {
				_currentTime = Time.time;
			}

			// wander to new patrol point.
			if (Time.time - _currentTime >= PauseLength) {
				_currentTime = 0f;
				Wander();
			}
		}
	}

	private void Chase() {
		_enemyAgent.speed = ChaseSpeed;
		_currentDestination = new Vector3(_target.position.x, 0f, _target.position.z);
		_enemyAgent.SetDestination(_currentDestination);
	}

	public void Wander() {
		_enemyAgent.speed = WanderSpeed;
		_currentDestination = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-4.4f, 4.4f));
		_enemyAgent.SetDestination(_currentDestination);
	}

	private void LookAround() {
		Vector3 targetDirection = _target.position;
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
		// Look at last sighting of player
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, TurningSpeed * Time.deltaTime);
		
		// Head towards last sighting of player
		_currentDestination = new Vector3(targetDirection.x, 0f, targetDirection.z);
		_enemyAgent.SetDestination(_currentDestination);
		_isSearching = false;
	}

	private bool ReachedDestination(Vector3 location, float distance) {
		return Vector3.Distance(transform.position, location) < distance;
	}
}