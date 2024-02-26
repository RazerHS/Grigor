using System.Collections.Generic;
using System.Linq;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Characters;
using Grigor.Data.Credentials;
using Grigor.Gameplay.Interacting.Components;
using Grigor.UI;
using Grigor.UI.Widgets;
using Sirenix.Utilities;

namespace Grigor.Gameplay.World.Components
{
    public class CredentialSelectInteractable : InteractableComponent
    {
        [Inject] private CharacterRegistry characterRegistry;
        [Inject] private UIManager uiManager;

        private List<CredentialType> credentialsToSelect = new();
        private List<PlayerCredential> selectedCredentials = new();
        private int currentCredentialIndex;

        private CredentialSelectWidget credentialSelectWidget;
        private DataPodWidget dataPodWidget;

        protected override void OnInitialized()
        {
            credentialSelectWidget = uiManager.GetWidget<CredentialSelectWidget>();
            dataPodWidget = uiManager.GetWidget<DataPodWidget>();

            foreach (PlayerCredential credential in characterRegistry.Player.Data.PlayerCredentialValues)
            {
                credentialsToSelect.Add(credential.CredentialType);
            }

            credentialsToSelect = credentialsToSelect.Distinct().ToList();
        }

        protected override void OnInteractEffect()
        {
            credentialSelectWidget.Show();

            SetupOptions();
        }

        private void SetupOptions()
        {
            List<string> options = characterRegistry.Player.Data.PlayerCredentialValues.Where(credential => credential.CredentialType == credentialsToSelect[currentCredentialIndex]).Select(credential => credential.Value).ToList();

            if (options.Count != 2)
            {
                Log.Error($"There are not exactly two options for the credential type {credentialsToSelect[currentCredentialIndex]}!");
            }

            string optionOne = options[0];
            string optionTwo = options[1];

            credentialSelectWidget.SetCurrentCredential(credentialsToSelect[currentCredentialIndex].ToString().SplitPascalCase(), optionOne, optionTwo);

            credentialSelectWidget.OptionSelectedEvent += OnOptionSelected;
        }

        private void OnOptionSelected(string option)
        {
            selectedCredentials.Add(new PlayerCredential(credentialsToSelect[currentCredentialIndex], option));

            currentCredentialIndex++;

            credentialSelectWidget.OptionSelectedEvent -= OnOptionSelected;

            if (currentCredentialIndex >= credentialsToSelect.Count)
            {
                AllCredentialsSelected();
            }
            else
            {
                SetupOptions();
            }
        }

        private void AllCredentialsSelected()
        {
            characterRegistry.Player.Data.SetPlayerCredentials(selectedCredentials);

            dataPodWidget.AddPlayerCredentials(selectedCredentials);

            credentialSelectWidget.Hide();

            EndInteract();
        }
    }
}
