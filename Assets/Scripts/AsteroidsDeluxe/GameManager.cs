using System;
using System.Collections;
using System.Threading.Tasks;
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
			Dispatch.Listen<ObjectDestroyedMessage>(OnObjectDestroyed);

			StartCoroutine(InitRoutine());
		}

		private IEnumerator InitRoutine()
        {
			yield return new WaitForEndOfFrame();
			StartMainMenu();
        }

		private void OnDestroy()
		{
			Dispatch.Unlisten<ObjectDestroyedMessage>(OnObjectDestroyed);
		}

		private void SetGameState(GameState targetState)
		{
			var prevState = _currentGameState;
			_currentGameState = targetState;
			Dispatch.Fire(new GameStateMessage { state = targetState, prevState = prevState });

		}

		private async void OnObjectDestroyed(ObjectDestroyedMessage message)
		{
			if(message.DestroyedType != ObjectType.PlayerShip) return;

			if(_livesManager.PlayerLives <= 0)
            {
				OnGameOver();
				return;
            }

			await RespawnPlayer();
		}

		private async Task RespawnPlayer()
		{
			await Task.Delay(TimeSpan.FromSeconds(_respawnDelay));
			if(_livesManager.PlayerLives <= 0) return;

			_player.transform.localPosition = Vector3.zero;
			_player.gameObject.SetActive(true);
			_player.Init();
		}

		private void StartMainMenu()
        {
			SetGameState(GameState.MainMenu);
			WaveManager.SpawnWave();
        }

		public void StartGame()
		{
			WaveManager.DeInit();
			SetGameState(GameState.Gameplay);

			_livesManager.Init();
			_ = RespawnPlayer();
			_waveManager.SpawnWave();
		}

		private void OnGameOver()
        {
			_currentGameState = GameState.Leaderboard;
			Dispatch.Fire(new GameStateMessage { state = GameState.Leaderboard });
        }
	}
}