using AsteroidsDeluxe;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _livesLabel;
    [SerializeField] private TMP_Text _pointsLabel;
	[SerializeField] private TMP_Text _waveCountLabel;

	private void Start()
	{
		_pointsLabel.text = "0";
		UpdateWaveText(1);

		Dispatch.Listen<PointsAwardedMessage>(OnPointsAwarded);
		Dispatch.Listen<WaveStartedMessage>(OnWaveStarted);
	}

	private void OnDestroy()
	{
		Dispatch.Unlisten<PointsAwardedMessage>(OnPointsAwarded);
		Dispatch.Unlisten<WaveStartedMessage>(OnWaveStarted);
	}

	private void OnWaveStarted(WaveStartedMessage message)
	{
		UpdateWaveText(message.waveCount);
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

	private void UpdateWaveText(int waveCount)
	{
		_waveCountLabel.text = $"Wave - {waveCount}";
	}
}
