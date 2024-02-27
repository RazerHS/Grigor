using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Grigor.UI.Widgets
{
    public class ReceiveMessageWidget : UIWidget
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private float messageDuration;

        private Sequence sequence;

        protected override void OnShow()
        {
            Reset();
        }

        protected override void OnHide()
        {

        }

        public void DisplayMessage(string sender)
        {
            Reset();

            messageText.text = $"Message received from: {sender}! Check your data pod.";

            sequence?.Kill();

            sequence = DOTween.Sequence();

            sequence.Append(messageText.DOFade(1f, 0.5f));
            sequence.AppendInterval(messageDuration);
            sequence.Append(messageText.DOFade(0f, 0.5f));

            sequence.Play();
        }

        public void Reset()
        {
            messageText.DOFade(0f, 0f);
        }
    }
}
