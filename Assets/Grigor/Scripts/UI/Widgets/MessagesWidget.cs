using System.Collections.Generic;
using Grigor.Gameplay.Messages;
using Grigor.UI.Data;
using TMPro;
using UnityEngine;

namespace Grigor.UI.Widgets
{
    public class MessagesWidget : UIWidget
    {
        [SerializeField] private Transform messageDisplayParent;
        [SerializeField] private MessageUIDisplay messageUIDisplayPrefab;
        [SerializeField] private TextMeshProUGUI selectedMessageTitle;
        [SerializeField] private TextMeshProUGUI selectedMessageSender;
        [SerializeField] private TextMeshProUGUI selectedMessageText;

        private readonly List<MessageUIDisplay> displayedMessages = new();

        protected override void OnShow()
        {
            foreach (MessageUIDisplay messageUIDisplay in displayedMessages)
            {
                messageUIDisplay.OnSelectedMessage += OnMessageSelected;
            }

            selectedMessageTitle.text = "Select a message!";
            selectedMessageSender.text = "From: Your Dearest Developers.";
            selectedMessageText.text = "";
        }

        protected override void OnHide()
        {

        }

        public void OnMessageReceived(Message message)
        {
            MessageUIDisplay messageUIDisplay = Instantiate(messageUIDisplayPrefab, transform);

            messageUIDisplay.transform.SetParent(messageDisplayParent);

            messageUIDisplay.Initialize(message);

            if (displayedMessages.Contains(messageUIDisplay))
            {
                return;
            }

            displayedMessages.Add(messageUIDisplay);
        }

        private void OnMessageSelected(Message message)
        {
            selectedMessageTitle.text = message.Title;
            selectedMessageSender.text = $"From: {message.Sender.DisplayName}";
            selectedMessageText.text = message.MessageText;
        }
    }
}
