using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data.Clues;
using Grigor.Gameplay.Clues;
using Grigor.UI;
using Grigor.UI.Widgets;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class FindClueInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("Clue", false, true)] private ClueData clueToFind;

        [Inject] private ClueRegistry clueRegistry;
        [Inject] private UIManager uiManager;

        private bool clueFound;
        private MessagePopupWidget messagePopupWidget;

        [ColoredBoxGroup("Clue"), Button] private void ForceFindClue() => FindClue();

        protected override void OnInitialized()
        {
            messagePopupWidget = uiManager.GetWidget<MessagePopupWidget>();

            if (clueToFind == null)
            {
                throw Log.Exception($"Clue to find not set in interactable {name}!");
            }

            clueRegistry.RegisterClue(clueToFind);

            FindClue();
        }

        protected override void OnInteractEffect()
        {
            if (!clueFound)
            {
                FindClue();
            }

            EndInteract();
        }

        private void FindClue()
        {
            Log.Write($"Found clue: <b>{clueToFind.CredentialType}</b>");

            clueToFind.OnClueFound();

            clueRegistry.UnregisterClue(clueToFind);

            clueFound = true;

            messagePopupWidget.DisplayMessage($"Found {clueToFind.name}!");
        }
    }
}
