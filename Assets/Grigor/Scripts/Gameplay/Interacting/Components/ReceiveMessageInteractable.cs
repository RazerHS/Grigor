using CardboardCore.DI;
using Grigor.Gameplay.Messages;
using Grigor.UI;
using Grigor.UI.Widgets;
using Grigor.Utils;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class ReceiveMessageInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("Message", false, true), HideLabel] private Message message;
        [SerializeField, ColoredBoxGroup("Message")] private float delay;

        [Inject] private UIManager uiManager;

        private MessagesWidget messagesWidget;

        protected override void OnInitialized()
        {
            messagesWidget = uiManager.GetWidget<MessagesWidget>();
        }

        protected override void OnInteractEffect()
        {
            Helper.Delay(delay, ReceiveMessage);
        }

        private void ReceiveMessage()
        {
            messagesWidget.OnMessageReceived(message);

            EndInteract();
        }
    }
}
