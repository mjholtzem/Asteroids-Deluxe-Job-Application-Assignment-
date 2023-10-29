using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace AsteroidsDeluxe
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        public virtual void Open()
        {
            gameObject.SetActive(true);
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(1, .5f);
        }

        public virtual void Close()
        {
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(0, .5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }

        private void Reset()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
    }
}
