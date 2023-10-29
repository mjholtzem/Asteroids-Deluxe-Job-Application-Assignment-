using UnityEngine;
using TMPro;
using DG.Tweening;

namespace AsteroidsDeluxe
{
    public class MainMenu : UIPanel
    {
        [SerializeField] private TMP_Text _startGameLabel;

        private void OnEnable()
        {
            _startGameLabel.alpha = 0;

            var sequence = DOTween.Sequence();
            sequence.Append(_startGameLabel.DOFade(1, 1));
            sequence.AppendInterval(1);
            sequence.Append(_startGameLabel.DOFade(0, 1));
            sequence.SetLoops(-1, LoopType.Restart);
            sequence.SetId(_startGameLabel);
        }

        private void OnDisable()
        {
            _startGameLabel.DOKill();
            _startGameLabel.alpha = 0;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            GameManager.Instance.StartGame();
        }
    }
}
