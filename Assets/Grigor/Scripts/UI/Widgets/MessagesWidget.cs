using System.Collections.Generic;
using Grigor.Gameplay.Messages;
using Grigor.UI.Data;
using UnityEngine;

namespace Grigor.UI.Widgets
{
    public class MessagesWidget : UIWidget
    {
        [SerializeField] private Transform messageDisplayParent;
        [SerializeField] private MessageUIDisplay messageUIDisplayPrefab;

        private readonly List<MessageUIDisplay> displayedMessages = new();

        protected override void OnShow()
        {

        }

        protected override void OnHide()
        {

        }

        public void OnMessageReceived(Message message)
        {
            MessageUIDisplay messageUIDisplay = Instantiate(messageUIDisplayPrefab, transform);

            messageUIDisplay.transform.SetParent(messageDisplayParent);

            messageUIDisplay.Initialize(message.Sender.name, message.Title, message.MessageText);

            if (displayedMessages.Contains(messageUIDisplay))
            {
                return;
            }

            displayedMessages.Add(messageUIDisplay);
        }
    }
}
