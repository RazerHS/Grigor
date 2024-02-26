using System.Collections.Generic;
using System.Linq;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data;
using Grigor.Data.Clues;
using Grigor.Data.Credentials;
using Grigor.Gameplay.Clues;
using Grigor.UI;
using Grigor.UI.Data;
using Grigor.UI.Widgets;
using Sirenix.OdinInspector;
using UnityEngine;

public class DataPodWidget : UIWidget
{
    [SerializeField] private CredentialUIDisplay credentialDisplayPrefab;
    [SerializeField] private GameObject dataPodView;
    [SerializeField] private Transform credentialDisplayParent;
    [SerializeField] private Transform playerCredentialDisplayParent;

    [Inject] private ClueRegistry clueRegistry;
    [Inject] private UIManager uiManager;

    [ShowInInspector] private readonly Dictionary<CredentialType, CredentialUIDisplay> displayedCredentials = new();

    private CredentialWallet criminalCredentialWallet;
    private MessagePopupWidget messagePopupWidget;

    protected override void OnShow()
    {
        InsertCredentials();

        HideDataPod();

        messagePopupWidget = uiManager.GetWidget<MessagePopupWidget>();
    }

    protected override void OnHide()
    {
        foreach (CredentialUIDisplay credentialUIDisplay in displayedCredentials.Values)
        {
            credentialUIDisplay.Dispose();
        }
    }

    private void InsertCredentials()
    {
        criminalCredentialWallet = DataStorage.Instance.GetCriminalCredentials();

        foreach (CredentialEntry credential in criminalCredentialWallet.CredentialEntries)
        {
            AddNewCredential(credential.CredentialType);
        }
    }

    public void ShowDataPod()
    {
        dataPodView.SetActive(true);
    }

    public void HideDataPod()
    {
        dataPodView.SetActive(false);
    }

    private void AddNewCredential(CredentialType credentialType)
    {
        if (displayedCredentials.ContainsKey(credentialType))
        {
            Log.Error($"Credential <b>{credentialType}</b> already exists in data pod. Maybe an update was intended instead?");
            return;
        }

        CredentialUIDisplay credentialUIDisplay = Instantiate(credentialDisplayPrefab, credentialDisplayParent);
        credentialUIDisplay.Initialize(criminalCredentialWallet, credentialType, criminalCredentialWallet.GetMatchingClue(credentialType));
        credentialUIDisplay.SetCredentialDisplay(credentialType.ToString(), false);

        displayedCredentials.Add(credentialType, credentialUIDisplay);

        credentialUIDisplay.CheckInputStringEvent += OnTypedClue;
    }

    private void AddNewPlayerCredential(CredentialType credentialType, string value)
    {
        CredentialUIDisplay credentialUIDisplay = Instantiate(credentialDisplayPrefab, playerCredentialDisplayParent);
        credentialUIDisplay.InitializePlayerCredential(credentialType, value);
    }

    private void OnTypedClue(CredentialUIDisplay credentialUIDisplay, string inputString)
    {
        if (inputString != credentialUIDisplay.HeldClue.EvidenceText)
        {
            Log.Write($"Typed clue <b>{inputString}</b> does not match clue <b>{credentialUIDisplay.HeldClue.EvidenceText}</b>!");

            return;
        }

        Log.Write($"Typed clue <b>{inputString}</b> matches clue <b>{credentialUIDisplay.HeldClue.EvidenceText}</b>!");

        ClueData matchedClue = criminalCredentialWallet.GetMatchingClue(credentialUIDisplay.CredentialType);

        clueRegistry.RegisterMatchedClue(matchedClue);

        credentialUIDisplay.DisableInputField();
        credentialUIDisplay.MarkAsCorrect();

        messagePopupWidget.DisplayMessage($"You have matched a clue! <b>{matchedClue.EvidenceText}</b> has been added to your evidence board!");
    }

    public void AddPlayerCredentials(List<PlayerCredential> playerCredentials)
    {
        playerCredentialDisplayParent.GetComponentsInChildren<CredentialUIDisplay>().ToList().ForEach(credentialUIDisplay => Destroy(credentialUIDisplay.gameObject));

        foreach (PlayerCredential playerCredential in playerCredentials)
        {
            AddNewPlayerCredential(playerCredential.CredentialType, playerCredential.Value);
        }
    }
}
