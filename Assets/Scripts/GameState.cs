using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameState : MonoBehaviour {
	private bool _gameStarted;

	[SerializeField] private Text gameStateText;
	[SerializeField] private GameObject player;
	[SerializeField] private EnemyMovement enemyMovement;

	private const float RestartDelay = 3f;
	private float _restartTimer;
	private PlayerMovement _playerMovement;
	private PlayerHealth _playerHealth;

	private void Start() {
		player.SetActive(true);

		Cursor.visible = false;

		_playerMovement = player.GetComponent<PlayerMovement>();
		_playerHealth = player.GetComponent<PlayerHealth>();

		_playerMovement.enabled = false;
		enemyMovement.enabled = false;
	}

	private void Update() {
		if (_gameStarted == false && Input.GetKeyUp(KeyCode.Space)) {
			StartGame();
		}

		if (_playerHealth.alive == false) {
			EndGame();
			_restartTimer = _restartTimer + Time.deltaTime;

			if (_restartTimer >= RestartDelay) {
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		}
	}

	private void StartGame() {
		_gameStarted = true;

		gameStateText.color = Color.clear;

		_playerMovement.enabled = true;
		enemyMovement.enabled = true;
	}

	private void EndGame() {
		_gameStarted = false;

		gameStateText.color = Color.black;
		gameStateText.text = "Game Over";

		player.SetActive(false);
	}
}