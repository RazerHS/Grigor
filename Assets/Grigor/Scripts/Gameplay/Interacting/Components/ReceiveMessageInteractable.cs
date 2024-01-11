using CardboardCore.DI;
using Grigor.Gameplay.Messages;
using Grigor.UI;
using Grigor.UI.Widgets;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class ReceiveMessageInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("Message", false, true), HideLabel] private Message message;

        [Inject] private UIManager uiManager;

        private MessagesWidget messagesWidget;

        protected override void OnInitialized()
        {
            messagesWidget = uiManager.GetWidget<MessagesWidget>();
        }

        protected override void OnInteractEffect()
        {
            messagesWidget.OnMessageReceived(message);

            EndInteract();
        }
    }
}
