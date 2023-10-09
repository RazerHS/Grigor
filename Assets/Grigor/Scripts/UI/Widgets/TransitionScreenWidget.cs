using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Grigor.UI.Widgets
{
    public class TransitionScreenWidget : UIWidget
    {
        [SerializeField] private Image transitionImage;
        [SerializeField] private float transitionDuration;

        private Sequence sequence;

        protected override void OnShow()
        {
            sequence?.Kill();

            sequence = DOTween.Sequence();

            sequence.Append(transitionImage.DOFade(1f, transitionDuration).SetEase(Ease.InCubic));
            sequence.Append(transitionImage.DOFade(0f, transitionDuration).SetEase(Ease.InCubic));

            sequence.OnComplete(Hide);

            sequence.Play();
        }

        protected override void OnHide()
        {

        }
    }
}
