using System.Collections.Generic;
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
using UnityEngine.InputSystem;
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
    [Inject] private UIManager uiManager;

    [ShowInInspector] private readonly Dictionary<CredentialType, CredentialUIDisplay> displayedCredentials = new();
    [ShowInInspector] private List<ClueUIDisplay> displayedClues = new();
    [ShowInInspector] private Dictionary<CredentialUIDisplay, ClueUIDisplay> correctMatches = new();

    private CredentialWallet criminalCredentialWallet;
    private MessagePopupWidget messagePopupWidget;

    protected override void OnShow()
    {
        InsertCredentials();
        RegisterClueListener();

        toggleDataPodButton.onClick.AddListener(OnToggleDataPodButtonClicked);

        messagePopupWidget = uiManager.GetWidget<MessagePopupWidget>();
    }

    protected override void OnHide()
    {
        toggleDataPodButton.onClick.RemoveListener(OnToggleDataPodButtonClicked);

        foreach (CredentialUIDisplay credentialUIDisplay in displayedCredentials.Values)
        {
            credentialUIDisplay.Dispose();
        }

        foreach (ClueUIDisplay clueUIDisplay in displayedClues)
        {
            clueUIDisplay.Dispose();
        }
    }

    private void InsertCredentials()
    {
        criminalCredentialWallet = dataRegistry.GetCriminalCredentials();

        foreach (CredentialEntry credential in criminalCredentialWallet.CredentialEntries)
        {
            AddNewCredential(credential.CredentialType);
        }
    }

    private void InsertExistingClues()
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
        credentialUIDisplay.Initialize();
        credentialUIDisplay.StoreCredentialType(credentialType);
        credentialUIDisplay.SetCredentialDisplay(credentialType.ToString(), false);

        displayedCredentials.Add(credentialType, credentialUIDisplay);

        credentialUIDisplay.CheckDroppedClueEvent += OnClueAttached;
    }

    private void OnClueAttached(CredentialUIDisplay credentialUIDisplay, ClueUIDisplay clueUIDisplay)
    {
        clueUIDisplay.FlagAsAttached(credentialUIDisplay);
        clueUIDisplay.transform.SetParent(credentialUIDisplay.ClueHolderTransform);

        clueUIDisplay.ClueDragStartedEvent += OnClueDetached;

        CheckCurrentClueMatches();
    }

    private void OnClueDetached(ClueUIDisplay clueUIDisplay)
    {
        clueUIDisplay.ClueDragStartedEvent -= OnClueDetached;

        clueUIDisplay.AttachedToCredentialUIDisplay.DetachClue();

        clueUIDisplay.FlagAsDetached();
        clueUIDisplay.ResetPosition();
        clueUIDisplay.transform.SetParent(clueDisplayParent);
    }

    private void CheckCurrentClueMatches()
    {
        Dictionary<CredentialUIDisplay, ClueUIDisplay> currentCorrectMatches = new();

        foreach (CredentialUIDisplay credentialUIDisplay in displayedCredentials.Values)
        {
            ClueUIDisplay clueUIDisplay = credentialUIDisplay.AttachedClueUIDisplay;

            if (clueUIDisplay == null)
            {
                continue;
            }

            if (credentialUIDisplay.CredentialType != clueUIDisplay.ClueData.CredentialType)
            {
                continue;
            }

            if (clueUIDisplay.ClueData != criminalCredentialWallet.GetMatchingClue(clueUIDisplay.ClueData.CredentialType))
            {
                continue;
            }

            if (correctMatches.ContainsKey(credentialUIDisplay))
            {
                return;
            }

            currentCorrectMatches.Add(credentialUIDisplay, clueUIDisplay);
        }

        if (currentCorrectMatches.Count < GameConfig.Instance.CorrectCluesBeforeLock)
        {
            return;
        }

        FoundNewCorrectMatches(currentCorrectMatches);
    }

    private void FoundNewCorrectMatches(Dictionary<CredentialUIDisplay, ClueUIDisplay> currentCorrectMatches)
    {
        List<ClueData> matchedClues = new();

        foreach (KeyValuePair<CredentialUIDisplay, ClueUIDisplay> keyValuePair in currentCorrectMatches)
        {
            CredentialUIDisplay credentialUIDisplay = keyValuePair.Key;
            ClueUIDisplay clueUIDisplay = keyValuePair.Value;

            matchedClues.Add(clueUIDisplay.ClueData);

            credentialUIDisplay.DisableDrop();
            clueUIDisplay.DisableDrag();

            credentialUIDisplay.SnapClueToHolder();

            Log.Write($"Clue {clueUIDisplay.ClueData.CredentialType.ToString()} locked in as correct!");

            correctMatches.Add(credentialUIDisplay, clueUIDisplay);
        }

        clueRegistry.RegisterMatchedClues(matchedClues);
    }

    public void OnClueFound(ClueData clueData)
    {
        ClueUIDisplay clueUIDisplay = Instantiate(clueDisplayPrefab, clueDisplayParent);
        clueUIDisplay.Initialize();
        clueUIDisplay.SetClueData(clueData);
        clueUIDisplay.SetClueText(clueData.ClueHeading);

        displayedClues.Add(clueUIDisplay);
    }

    public void OnMatchedClues(List<ClueData> matchedClues)
    {
        messagePopupWidget.DisplayMessage("You matched 3 clues!");
    }

    public void RegisterClueListener()
    {
        clueRegistry.RegisterListener(this);
    }
}
