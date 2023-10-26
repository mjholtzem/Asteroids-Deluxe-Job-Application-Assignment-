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
		[SerializeField] private ScreenWrapManager _screenWrapManager;
		[SerializeField] private WaveManager _waveManager;

		public ScreenWrapManager ScreenWrapManager => _screenWrapManager;
		public WaveManager WaveManager => _waveManager;

		private void Start()
		{
			Dispatch.Listen<PlayerDestroyedMessage>(OnPlayerDestroyed);
		}

		private void OnDestroy()
		{
			Dispatch.Unlisten<PlayerDestroyedMessage>(OnPlayerDestroyed);
		}

		private async void OnPlayerDestroyed(PlayerDestroyedMessage message)
		{
			await RespawnPlayer();
		}

		private async Task RespawnPlayer()
		{
			await Task.Delay(TimeSpan.FromSeconds(_respawnDelay));
			_player.transform.localPosition = Vector3.zero;
			_player.gameObject.SetActive(true);
			_player.Init(true);
		}
	}
}