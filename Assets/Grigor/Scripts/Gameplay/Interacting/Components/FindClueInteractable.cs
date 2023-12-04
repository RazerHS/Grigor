using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data.Clues;
using Grigor.Gameplay.Clues;
using Grigor.UI;
using Grigor.UI.Widgets;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class FindClueInteractable : InteractableComponent
    {
        [SerializeField] private ClueData clueToFind;

        [Inject] private ClueRegistry clueRegistry;
        [Inject] private UIManager uiManager;

        private MessagePopupWidget messagePopupWidget;

        protected override void OnInitialized()
        {
            messagePopupWidget = uiManager.GetWidget<MessagePopupWidget>();

            if (clueToFind == null)
            {
                throw Log.Exception($"Clue to find not set in interactable {name}!");
            }
            clueRegistry.RegisterClue(clueToFind);
        }

        protected override void OnInteractEffect()
        {
            FindClue();

            EndInteract();
        }

        [Button]
        private void FindClue()
        {
            Log.Write($"Found clue: <b>{clueToFind.CredentialType}</b>");

            clueToFind.OnClueFound();

            messagePopupWidget.DisplayMessage($"Found {clueToFind.name}!");
        }
    }
}
