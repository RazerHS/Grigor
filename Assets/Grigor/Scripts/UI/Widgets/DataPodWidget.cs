using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Characters;
using Grigor.Data.Credentials;
using Grigor.Overworld.Clues;
using Grigor.UI;
using Grigor.UI.Data;
using UnityEngine;
using UnityEngine.UI;

public class DataPodWidget : UIWidget, IClueListener
{
    [SerializeField] private CredentialUIDisplay credentialDisplayPrefab;
    [SerializeField] private GameObject dataPodView;
    [SerializeField] private Transform credentialDisplayParent;
    [SerializeField] private Button toggleDataPodButton;

    [Inject] private ClueRegistry clueRegistry;
    [Inject] private CredentialRegistry credentialRegistry;
    [Inject] private CharacterRegistry characterRegistry;

    private List<CredentialEntry> criminalCredentials;
    private readonly Dictionary<CredentialType, CredentialUIDisplay> displayedCredentials = new();

    protected override void OnShow()
    {
        Injector.Inject(this);

        // criminalCredentials = credentialRegistry.GetCredentials(criminalData);

        toggleDataPodButton.onClick.AddListener(OnToggleDataPodButtonClicked);
    }

    protected override void OnHide()
    {
        toggleDataPodButton.onClick.RemoveListener(OnToggleDataPodButtonClicked);

        Injector.Release(this);
    }

    private void OnToggleDataPodButtonClicked()
    {
        dataPodView.SetActive(!dataPodView.activeSelf);
    }

    public void AddNewCredential(CredentialType credentialType, string value)
    {
        if (displayedCredentials.ContainsKey(credentialType))
        {
            Log.Error($"Credential <b>{credentialType}</b> already exists in data pod. Maybe an update was intended instead?");
            return;
        }

        CredentialUIDisplay credential = Instantiate(credentialDisplayPrefab, credentialDisplayParent);
        credential.SetCredentialDisplay(credentialType.ToString(), value);

        displayedCredentials.Add(credentialType, credential);
    }

    public void UpdateCredential(CredentialType credentialType, string value)
    {
        if (!displayedCredentials.ContainsKey(credentialType))
        {
            Log.Error($"Credential <b>{credentialType}</b> does not exist in data pod. Maybe adding it was intended instead?");
            return;
        }

        displayedCredentials[credentialType].SetCredentialDisplay(credentialType.ToString(), value);
    }

    public void OnClueFound(CredentialType credentialType)
    {
        CredentialUIDisplay credential = displayedCredentials[credentialType];

        if (credential == null)
        {
            throw Log.Exception($"Found clue <b>{credentialType}</b> doesn't exist in data pod!");
        }

        credential.SetCredentialDisplay(credentialType.ToString(), credentialType.ToString());
    }
}
