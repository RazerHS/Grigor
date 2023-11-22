using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data;
using Grigor.Data.Clues;
using Grigor.Data.Credentials;
using Grigor.Gameplay.Clues;
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
    [Inject] private DataRegistry dataRegistry;

    private CredentialWallet criminalCredentialWallet;
    private readonly Dictionary<CredentialType, CredentialUIDisplay> displayedCredentials = new();

    protected override void OnShow()
    {
        InsertCredentials();
        RegisterClueListener();

        toggleDataPodButton.onClick.AddListener(OnToggleDataPodButtonClicked);
    }

    protected override void OnHide()
    {
        toggleDataPodButton.onClick.RemoveListener(OnToggleDataPodButtonClicked);
    }

    private void InsertCredentials()
    {
        criminalCredentialWallet = dataRegistry.GetCriminalCredentials();

        foreach (CredentialEntry credential in criminalCredentialWallet.CredentialEntries)
        {
            AddNewCredential(credential.CredentialType, credential.CredentialValue);
        }
    }

    private void OnToggleDataPodButtonClicked()
    {
        dataPodView.SetActive(!dataPodView.activeSelf);
    }

    private void RevealCredentialValue(CredentialType credentialType)
    {
        if (!displayedCredentials.ContainsKey(credentialType))
        {
            Log.Error($"Credential <b>{credentialType}</b> does not exist in data pod. Maybe adding it was intended instead?");
            return;
        }

        Log.Write($"<b>{credentialType}</b> value revealed: <b>{displayedCredentials[credentialType].StoredValue}</b>!");

        displayedCredentials[credentialType].SetCredentialDisplay(credentialType.ToString(), true);
    }

    public void AddNewCredential(CredentialType credentialType, string value)
    {
        if (displayedCredentials.ContainsKey(credentialType))
        {
            Log.Error($"Credential <b>{credentialType}</b> already exists in data pod. Maybe an update was intended instead?");
            return;
        }

        CredentialUIDisplay credential = Instantiate(credentialDisplayPrefab, credentialDisplayParent);
        credential.StoreValue(value);
        credential.SetCredentialDisplay(credentialType.ToString(), false);

        displayedCredentials.Add(credentialType, credential);
    }

    public void OnClueFound(ClueData clueData)
    {
        CredentialUIDisplay credential = displayedCredentials[clueData.CredentialType];

        if (credential == null)
        {
            throw Log.Exception($"Found clue <b>{clueData.CredentialType}</b> doesn't exist in data pod!");
        }

        RevealCredentialValue(clueData.CredentialType);
    }

    public void RegisterClueListener()
    {
        clueRegistry.RegisterListener(this);
    }
}
