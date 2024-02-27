using System;
using Grigor.Gameplay.Messages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Data
{
    public class MessageUIDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button selectMessageButton;

        private Message message;

        public event Action<Message> OnSelectedMessage;

        public void Initialize(Message message)
        {
            this.message = message;

            titleText.text = message.Title;

            selectMessageButton.onClick.AddListener(OnSelectedMessageButton);
        }

        ~MessageUIDisplay()
        {
            selectMessageButton.onClick.RemoveListener(OnSelectedMessageButton);
        }

        private void OnSelectedMessageButton()
        {
            OnSelectedMessage?.Invoke(message);
        }
    }
}
