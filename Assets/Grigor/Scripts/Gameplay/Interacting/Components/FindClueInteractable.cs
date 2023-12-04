using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Gameplay.Clues;
using Grigor.UI;
using Grigor.UI.Widgets;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class FindClueInteractable : InteractableComponent
    {
        [SerializeField] private Clue clue;
        [SerializeField] private bool duringNightOnly;

        [Inject] private UIManager uiManager;

        private MessagePopupWidget messagePopupWidget;

        protected override void OnInitialized()
        {
            messagePopupWidget = uiManager.GetWidget<MessagePopupWidget>();
        }

        protected override void OnInteractEffect()
        {
            FindClue();

            EndInteract();
        }

        [Button]
        private void FindClue()
        {
            Log.Write($"Found clue: <b>{clue.ClueData.CredentialType}</b>");

            clue.FindClue();

            messagePopupWidget.DisplayMessage($"Found clue {clue.name}!");
        }
    }
}
