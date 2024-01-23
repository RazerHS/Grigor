using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Grigor.UI.Widgets
{
    public class TransitionWidget : UIWidget
    {
        [SerializeField] private Image transitionImage;
        [SerializeField] private float transitionDuration;

        private Sequence sequence;

        public float TransitionDuration => transitionDuration;

        public event Action TransitionEndedEvent;

        private void Awake()
        {
            transitionImage.DOFade(0f, 0f);
        }

        protected override void OnShow()
        {
            transitionImage.DOFade(0f, 0f);

            sequence?.Kill();

            sequence = DOTween.Sequence();

            sequence.Append(transitionImage.DOFade(1f, transitionDuration).SetEase(Ease.OutQuad).OnComplete(() => { TransitionEndedEvent?.Invoke(); }));
            sequence.Append(transitionImage.DOFade(0f, transitionDuration).SetEase(Ease.OutQuad));

            sequence.OnComplete(Hide);

            sequence.Play();
        }

        protected override void OnHide()
        {

        }
    }
}
