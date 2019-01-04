using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameState : MonoBehaviour {
	private bool _gameStarted;

	[SerializeField] private Text _gameStateText;
	[SerializeField] private GameObject _player;
	[SerializeField] private EnemyMovement _enemyMovement;

	private const float RestartDelay = 3f;
	private float _restartTimer;
	private PlayerMovement _playerMovement;
	private PlayerHealth _playerHealth;

	// Use this for initialization
	private void Start() {
		_player.SetActive(true);

		Cursor.visible = false;

		_playerMovement = _player.GetComponent<PlayerMovement>();
		_playerHealth = _player.GetComponent<PlayerHealth>();

		_playerMovement.enabled = false;
		_enemyMovement.enabled = false;
	}

	// Update is called once per frame
	private void Update() {
		if (_gameStarted == false && Input.GetKeyUp(KeyCode.Space)) {
			StartGame();
		}

		if (_playerHealth.Alive == false) {
			EndGame();
			_restartTimer = _restartTimer + Time.deltaTime;

			if (_restartTimer >= RestartDelay) {
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
		}
	}

	private void StartGame() {
		_gameStarted = true;

		_gameStateText.color = Color.clear;

		_playerMovement.enabled = true;
		_enemyMovement.enabled = true;
		_enemyMovement.Wander();
	}

	private void EndGame() {
		_gameStarted = false;

		_gameStateText.color = Color.black;
		_gameStateText.text = "Game Over";

		_player.SetActive(false);
	}
}