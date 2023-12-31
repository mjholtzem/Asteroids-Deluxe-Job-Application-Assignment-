using System.Collections;
using UnityEngine;

namespace AsteroidsDeluxe
{
    public class GameManager : Singleton<GameManager>
    {
		public enum GameState
        {
			None = -1,
			MainMenu = 0,
			Gameplay = 5,
			Leaderboard = 10
        }

		[Header("Config")]
		[Min(.25f)]
		[SerializeField] private float _respawnDelay = 2;
		[SerializeField] private float _gameOverDelay = 2;

		[Header("Game References")]
		[SerializeField] private Player _player;
		public Player Player => _player;

		[Header("Managers")]
		[SerializeField] private ScreenWrapManager _screenWrapManager;
		[SerializeField] private WaveManager _waveManager;
		[SerializeField] private WaveManager _mainMenuWaveManager;
		[SerializeField] private LivesManager _livesManager;

		public ScreenWrapManager ScreenWrapManager => _screenWrapManager;
		public WaveManager WaveManager => _currentGameState == GameState.MainMenu ? _mainMenuWaveManager : _waveManager;

		private GameState _currentGameState = GameState.None;
		public GameState CurrentGameState => _currentGameState;

		private void Start()
		{
			Dispatch.Listen<LivesChangedMessage>(OnLivesChanged);

			StartCoroutine(InitRoutine());
		}

		private IEnumerator InitRoutine()
        {
			yield return new WaitForEndOfFrame();
			StartMainMenu();
        }

		private void OnDestroy()
		{
			Dispatch.Unlisten<LivesChangedMessage>(OnLivesChanged);
		}

		private void SetGameState(GameState targetState)
		{
			var prevState = _currentGameState;
			_currentGameState = targetState;
			Dispatch.Fire(new GameStateMessage { state = targetState, prevState = prevState });

		}

		private void OnLivesChanged(LivesChangedMessage message)
		{
			if(_currentGameState != GameState.Gameplay) return;
			if(message.deltaLives > 0) return;

			if(message.currentLives <= 0)
            {
				StartCoroutine(OnGameOver());
				return;
            }

			StartCoroutine(RespawnPlayer());
		}

		private IEnumerator RespawnPlayer()
		{
			yield return new WaitForSeconds(_respawnDelay);
			if(_livesManager.PlayerLives <= 0) yield break;

			_player.transform.localPosition = Vector3.zero;
			_player.gameObject.SetActive(true);
			_player.Init();
		}

		private void StartMainMenu()
        {
			SetGameState(GameState.MainMenu);
			StartCoroutine(WaveManager.SpawnWave());
        }

		public void StartGame()
		{
			WaveManager.DeInit();
			SetGameState(GameState.Gameplay);

			_livesManager.Init();
			StartCoroutine(RespawnPlayer());
			StartCoroutine(_waveManager.SpawnWave());
		}

		private IEnumerator OnGameOver()
        {
			yield return new WaitForSeconds(_gameOverDelay);

			WaveManager.DeInit();
			StartMainMenu();
        }
	}
}