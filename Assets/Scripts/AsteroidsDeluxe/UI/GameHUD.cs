using AsteroidsDeluxe;
using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;

public class GameHUD : UIPanel
{
    [SerializeField] private TMP_Text _livesLabel;
    [SerializeField] private TMP_Text _pointsLabel;
	[SerializeField] private TMP_Text _waveCountLabel;
	[SerializeField] private TMP_Text _waveIntroLabel;
	[SerializeField] private Image _shieldProgressImage;
	[SerializeField] private CanvasGroup _backgroundFadeGroup;

	private void OnEnable()
	{
		_pointsLabel.text = "0";

		Dispatch.Listen<PointsAwardedMessage>(OnPointsAwarded);
		Dispatch.Listen<WaveStartedMessage>(OnWaveStarted);
		Dispatch.Listen<LivesChangedMessage>(OnLivesChanged);
		Dispatch.Listen<ShieldUpdateMessage>(OnShieldUpdated);
	}

	private void OnDisable()
	{
		Dispatch.Unlisten<PointsAwardedMessage>(OnPointsAwarded);
		Dispatch.Unlisten<WaveStartedMessage>(OnWaveStarted);
		Dispatch.Unlisten<LivesChangedMessage>(OnLivesChanged);
		Dispatch.Unlisten<ShieldUpdateMessage>(OnShieldUpdated);
	}

	private void OnWaveStarted(WaveStartedMessage message)
	{
		UpdateWaveText(message.waveCount);

		DisplayMessage($"Wave {message.waveCount}");
	}

	private void OnPointsAwarded(PointsAwardedMessage message)
	{
		DOTween.Complete("PointsTween");
		var points = message.totalPoints - message.pointsAwarded;

		var sequence = DOTween.Sequence();
		sequence.SetId("PointsTween");
		sequence.Append(DOTween.To(() => points, p => points = p, message.totalPoints, .25f).SetEase(Ease.InQuad).OnUpdate(() =>
		{
			_pointsLabel.text = points.ToString("N0");
		}));
		sequence.Join(_pointsLabel.transform.DOPunchScale(Vector3.one * .25f, .4f, 0, 0));
	}

	private async void OnLivesChanged(LivesChangedMessage message)
    {
		if(GameManager.Instance.CurrentGameState != GameManager.GameState.Gameplay) return;
		
		UpdateLivesText(message.currentLives);
		if(message.currentLives <= 0)
		{
			DisplayMessage("Game Over");

			await Task.Delay(TimeSpan.FromSeconds(1.5f));

			Debug.Log("fading to black - HUD");
			_backgroundFadeGroup.gameObject.SetActive(true);
			_backgroundFadeGroup.alpha = 0;
			_backgroundFadeGroup.DOFade(1, 1);
		}
    }

	private void OnShieldUpdated(ShieldUpdateMessage message)
    {
		_shieldProgressImage.fillAmount = message.remainingShield;
    }

	private void UpdateWaveText(int waveCount)
	{
		_waveCountLabel.text = $"Wave {waveCount}";
	}

	private void DisplayMessage(string messageText)
    {
		_waveIntroLabel.DOKill();
		_waveIntroLabel.text = messageText;
		_waveIntroLabel.color = new Color(1, 1, 1, 0);
		_waveIntroLabel.gameObject.SetActive(true);

		var sequence = DOTween.Sequence();
		sequence.SetId(_waveIntroLabel);
		sequence.Append(_waveIntroLabel.DOFade(1, 1.5f).SetEase(Ease.InOutQuad));
		sequence.AppendInterval(1);
		sequence.Append(_waveIntroLabel.DOFade(0, .25f).SetEase(Ease.InOutQuad));
		sequence.OnComplete(() => _waveIntroLabel.gameObject.SetActive(false));
	}

	private void UpdateLivesText(int lives)
    {
		_livesLabel.text = $"<space=1.7em>x {lives}";
    }
}
