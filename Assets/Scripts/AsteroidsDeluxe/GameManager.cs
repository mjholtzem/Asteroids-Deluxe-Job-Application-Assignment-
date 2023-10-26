using System;
using System.Threading.Tasks;
using UnityEngine;

namespace AsteroidsDeluxe
{
    public class GameManager : Singleton<GameManager>
    {
		[Header("Config")]
		[Min(.25f)]
		[SerializeField] private float _respawnDelay = 2;

		[Header("References")]
		[SerializeField] private Player _player;
		public Player Player => _player;
		[SerializeField] private ScreenWrapManager _screenWrapManager;
		[SerializeField] private WaveManager _waveManager;

		public ScreenWrapManager ScreenWrapManager => _screenWrapManager;
		public WaveManager WaveManager => _waveManager;

		private void Start()
		{
			Dispatch.Listen<ObjectDestroyedMessage>(OnObjectDestroyed);
			BeginGame();
		}

		private void OnDestroy()
		{
			Dispatch.Unlisten<ObjectDestroyedMessage>(OnObjectDestroyed);
		}

		private async void OnObjectDestroyed(ObjectDestroyedMessage message)
		{
			if(message.DestroyedType != ObjectType.PlayerShip) return;

			await RespawnPlayer();
		}

		private async Task RespawnPlayer()
		{
			await Task.Delay(TimeSpan.FromSeconds(_respawnDelay));
			_player.transform.localPosition = Vector3.zero;
			_player.gameObject.SetActive(true);
			_player.Init();
		}

		private async void BeginGame()
		{
			//just a little delay to make sure everything has started up. Perhaps in the future this would be triggered by some MainMenu UI interaction or something
			await Task.Delay(TimeSpan.FromSeconds(.25f));
			_waveManager.SpawnWave();
		}
	}
}