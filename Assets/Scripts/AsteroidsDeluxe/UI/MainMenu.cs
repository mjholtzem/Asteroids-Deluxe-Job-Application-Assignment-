using UnityEngine;
using TMPro;
using DG.Tweening;

namespace AsteroidsDeluxe
{
    public class MainMenu : UIPanel
    {
        [SerializeField] private TMP_Text _startGameLabel;
        [SerializeField] private CanvasGroup _backgroundFadeGroup;

        private void OnEnable()
        {
            Debug.Log("Fading from black - Main Menu");
            _backgroundFadeGroup.DOKill();
            _backgroundFadeGroup.alpha = 1;
            _backgroundFadeGroup.DOFade(0, 1);

            //Start Game Animation
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
            Debug.Log("Killing fade");
            _backgroundFadeGroup.DOKill();
            _backgroundFadeGroup.alpha = 0;

            _startGameLabel.DOKill();
            _startGameLabel.alpha = 0;
        }

        public override void Close()
        {
            gameObject.SetActive(false);
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
            Debug.Log("Fading to black - Main Menu");
            _backgroundFadeGroup.DOKill();
            _backgroundFadeGroup.DOFade(1, 1).OnComplete(() =>
            {
                GameManager.Instance.StartGame();
            });
        }
    }
}
