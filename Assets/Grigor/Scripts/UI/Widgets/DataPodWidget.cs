using System.Collections.Generic;
using CardboardCore.Utilities;
using Grigor.Data.Credentials;
using Grigor.UI;
using Grigor.UI.Data;
using UnityEngine;
using UnityEngine.UI;

public class DataPodWidget : UIWidget
{
    [SerializeField] private CredentialUIDisplay credentialDisplayPrefab;
    [SerializeField] private GameObject dataPodView;
    [SerializeField] private Transform credentialDisplayParent;
    [SerializeField] private Button toggleDataPodButton;

    private readonly Dictionary<CredentialType, CredentialUIDisplay> displayedCredentials = new();

    protected override void OnShow()
    {
        toggleDataPodButton.onClick.AddListener(OnToggleDataPodButtonClicked);
    }

    protected override void OnHide()
    {
        toggleDataPodButton.onClick.RemoveListener(OnToggleDataPodButtonClicked);
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
}
