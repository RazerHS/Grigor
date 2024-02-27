using System.Linq;
using CardboardCore.DI;
using Grigor.Characters;
using Grigor.Data.Credentials;
using Grigor.Gameplay.Interacting.Components;
using Grigor.UI;
using Grigor.UI.Widgets;
using Sirenix.Utilities;

namespace Grigor.Gameplay.World.Components
{
    public class DataVaultInteractable : InteractableComponent
    {
        [Inject] private UIManager uiManager;
        [Inject] private CharacterRegistry characterRegistry;

        private DataVaultWidget dataVaultWidget;
        private DataPodWidget dataPodWidget;
        private MessagePopupWidget messagePopupWidget;

        protected override void OnInitialized()
        {
            dataVaultWidget = uiManager.GetWidget<DataVaultWidget>();
            dataPodWidget = uiManager.GetWidget<DataPodWidget>();
            messagePopupWidget = uiManager.GetWidget<MessagePopupWidget>();
        }

        protected override void OnInteractEffect()
        {
            dataVaultWidget.Show();

            dataVaultWidget.InitializeCredentialList(characterRegistry.Player.Data.PlayerCredentials.Select(credential => credential.CredentialType).ToList());

            dataVaultWidget.CredentialChangedEvent += OnCredentialChanged;
            dataVaultWidget.BackButtonPressedEvent += OnBackButtonPressed;
        }

        private void OnBackButtonPressed()
        {
            LeaveVault();
        }

        private void OnCredentialChanged(CredentialType credentialType, string value)
        {
            characterRegistry.Player.Data.ReplacePlayerCredential(credentialType, value);

            dataPodWidget.AddPlayerCredentials(characterRegistry.Player.Data.PlayerCredentials);

            messagePopupWidget.DisplayMessage($"Credential {credentialType.ToString().SplitPascalCase()} updated to {value}!");

            LeaveVault();
        }

        private void LeaveVault()
        {
            dataVaultWidget.Hide();

            dataVaultWidget.CredentialChangedEvent -= OnCredentialChanged;
            dataVaultWidget.BackButtonPressedEvent -= OnBackButtonPressed;

            EndInteract();
        }
    }
}
