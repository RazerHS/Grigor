using System;
using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data;
using Grigor.Data.Clues;
using Grigor.Data.Credentials;
using Grigor.Gameplay.Clues;
using Grigor.UI;
using Grigor.UI.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class DataPodWidget : UIWidget, IClueListener
{
    [SerializeField] private CredentialUIDisplay credentialDisplayPrefab;
    [SerializeField] private ClueUIDisplay clueDisplayPrefab;
    [SerializeField] private GameObject dataPodView;
    [SerializeField] private Transform credentialDisplayParent;
    [SerializeField] private Transform clueDisplayParent;
    [SerializeField] private Button toggleDataPodButton;

    [Inject] private ClueRegistry clueRegistry;
    [Inject] private DataRegistry dataRegistry;

    [ShowInInspector] private readonly Dictionary<CredentialType, CredentialUIDisplay> displayedCredentials = new();
    [ShowInInspector] private List<ClueUIDisplay> displayedClues = new();
    [ShowInInspector] private Dictionary<CredentialUIDisplay, ClueUIDisplay> correctMatches = new();

    private CredentialWallet criminalCredentialWallet;

    public Transform ClueDisplayParent => clueDisplayParent;

    protected override void OnShow()
    {
        InsertCredentials();
        InsertClues();
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
            AddNewCredential(credential.CredentialType);
        }
    }

    private void InsertClues()
    {
        foreach (ClueData clueData in dataRegistry.ClueData)
        {
            ClueUIDisplay clueUIDisplay = Instantiate(clueDisplayPrefab, clueDisplayParent);
            clueUIDisplay.SetClueData(clueData);
            clueUIDisplay.SetClueText(clueData.ClueHeading);

            displayedClues.Add(clueUIDisplay);
        }
    }

    private void OnToggleDataPodButtonClicked()
    {
        dataPodView.SetActive(!dataPodView.activeSelf);
    }

    private void AddNewCredential(CredentialType credentialType)
    {
        if (displayedCredentials.ContainsKey(credentialType))
        {
            Log.Error($"Credential <b>{credentialType}</b> already exists in data pod. Maybe an update was intended instead?");
            return;
        }

        CredentialUIDisplay credentialUIDisplay = Instantiate(credentialDisplayPrefab, credentialDisplayParent);
        credentialUIDisplay.StoreCredentialType(credentialType);
        credentialUIDisplay.SetCredentialDisplay(credentialType.ToString(), false);

        displayedCredentials.Add(credentialType, credentialUIDisplay);

        credentialUIDisplay.CheckDroppedClueEvent += OnClueAttached;
    }

    private void OnClueAttached(CredentialUIDisplay credentialUIDisplay, ClueUIDisplay clueUIDisplay)
    {
        if (credentialUIDisplay.CredentialType != clueUIDisplay.ClueData.CredentialType)
        {
            return;
        }

        Log.Write($"matched clue {clueUIDisplay.ClueData.CredentialType.ToString()}!");

        CheckCurrentClueMatches();
    }

    private void CheckCurrentClueMatches()
    {
        Dictionary<CredentialUIDisplay, ClueUIDisplay> currentCorrectMatches = new();

        foreach (CredentialUIDisplay credentialUIDisplay in displayedCredentials.Values)
        {
            if (credentialUIDisplay.AttachedClueUIDisplay == null)
            {
                continue;
            }

            if (credentialUIDisplay.CredentialType != credentialUIDisplay.AttachedClueUIDisplay.ClueData.CredentialType)
            {
                continue;
            }

            if (correctMatches.ContainsKey(credentialUIDisplay))
            {
                return;
            }

            currentCorrectMatches.Add(credentialUIDisplay, credentialUIDisplay.AttachedClueUIDisplay);
        }

        if (currentCorrectMatches.Count < GameConfig.Instance.CorrectCluesBeforeLock)
        {
            return;
        }

        FoundNewCorrectMatches(currentCorrectMatches);
    }

    private void FoundNewCorrectMatches(Dictionary<CredentialUIDisplay, ClueUIDisplay> currentCorrectMatches)
    {
        foreach (KeyValuePair<CredentialUIDisplay, ClueUIDisplay> keyValuePair in currentCorrectMatches)
        {
            CredentialUIDisplay credentialUIDisplay = keyValuePair.Key;
            ClueUIDisplay clueUIDisplay = keyValuePair.Value;

            credentialUIDisplay.DisableDrop();
            clueUIDisplay.DisableDrag();

            credentialUIDisplay.SnapClueToHolder();

            Log.Write($"matched clue {clueUIDisplay.ClueData.ClueHeading}!");

            correctMatches.Add(credentialUIDisplay, clueUIDisplay);
        }
    }

    public void OnClueFound(ClueData clueData)
    {
        // TO-DO: add clues as they are found
    }

    public void RegisterClueListener()
    {
        clueRegistry.RegisterListener(this);
    }
}
