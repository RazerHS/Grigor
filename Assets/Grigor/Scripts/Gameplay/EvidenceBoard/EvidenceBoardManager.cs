using System;
using System.Collections.Generic;
using System.Linq;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Cinemachine;
using Grigor.Data;
using Grigor.Data.Clues;
using Grigor.Gameplay.Clues;
using Grigor.StateMachines.EvidenceBoard;
using Grigor.Utils;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Grigor.Gameplay.EvidenceBoard
{
    [Injectable]
    public class EvidenceBoardManager : CardboardCoreBehaviour, IClueListener
    {
        [SerializeField, ColoredBoxGroup("Board", false, true)] private List<EvidenceBoardNote> notes;
        [SerializeField, ColoredBoxGroup("Board", false, true)] private List<ConnectionLine> connections;
        [SerializeField, ColoredBoxGroup("Board")] private Vector2 boardFrameSize;

        [SerializeField, ColoredBoxGroup("References", false, true)] private Transform contentsParent;
        [SerializeField, ColoredBoxGroup("References")] private Transform connectionsParent;
        [SerializeField, ColoredBoxGroup("References")] private LineRenderer connectionLinePrefab;
        [SerializeField, ColoredBoxGroup("References")] private EvidenceBoardNote evidenceStickyNotePrefab;
        [SerializeField, ColoredBoxGroup("References")] private EvidenceBoardNote evidencePicturePrefab;

        [SerializeField, ColoredBoxGroup("Zooming", false, true)] private float scrollIncrement;
        [SerializeField, ColoredBoxGroup("Zooming")] private float maxFOV;
        [SerializeField, ColoredBoxGroup("Zooming")] private float minFOV;

        [SerializeField, HideInInspector] private DataStorage dataStorage;

        [Inject] ClueRegistry clueRegistry;

        [ColoredBoxGroup("Creating", false, true), Button(ButtonSizes.Large)] private void HideAllClues() => HideAllElementsOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large)] private void RevealAllClues() => RevealAllCluesOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large)] private void ConnectAllClues() => ConnectAllCluesOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large)] private void HideAllLines() => HideAllLinesOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large)] private void Refresh() => RefreshBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large), GUIColor(1f, 0f, 0f)] private void SpawnAllClues() => SpawnAllCluesOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large), GUIColor(1f, 0f, 0f)] private void ClearAllClues() => RemoveAllCluesFromBoard();

        private EvidenceBoardNote currentlySelectedNote;
        private bool canSelectNote;
        private EvidenceBoardStateMachine evidenceBoardStateMachine;
        private CinemachineVirtualCamera evidenceBoardVirtualCamera;

        public List<EvidenceBoardNote> Notes => notes;
        public float ScrollIncrement => scrollIncrement;
        public float MaxFOV => maxFOV;
        public float MinFOV => minFOV;
        public EvidenceBoardNote CurrentlySelectedNote => currentlySelectedNote;
        public bool CanSelectNote => canSelectNote;
        public CinemachineVirtualCamera EvidenceBoardVirtualCamera => evidenceBoardVirtualCamera;

        public event Action InteractWithBoardEvent;
        public event Action LeaveBoardEvent;

#if UNITY_EDITOR
        [OnInspectorInit]
        private void OnInspectorInit()
        {
            if (dataStorage == null)
            {
                dataStorage = Helper.LoadAsset("DataStorage", dataStorage);
            }
        }
#endif

        protected override void OnInjected()
        {
            RegisterClueListener();

            notes.ForEach(note =>
            {
                note.InitializeNoteContents();
            });

            evidenceBoardStateMachine = new EvidenceBoardStateMachine(true);
            evidenceBoardStateMachine.Start();

            HideAllElementsOnBoard();
        }

        protected override void OnReleased()
        {
            notes.ForEach(note =>
            {
                note.Dispose();
            });

            evidenceBoardStateMachine.Stop();
        }

        private void AddClue(EvidenceBoardNote note)
        {
            notes.Add(note);
        }

        private void RemoveAllCluesFromBoard()
        {
            DestroyAllConnections();

            for (int i = notes.Count - 1; i >= 0; i--)
            {
                EvidenceBoardNote note = notes[i];

                notes.Remove(note);

                DestroyImmediate(note.gameObject);
            }
        }

        private void OnReplaceNote(EvidenceBoardNote note)
        {
            Vector3 position = note.transform.localPosition;
            ClueData clueData = note.ClueData;

            note.ReplaceNoteEvent -= OnReplaceNote;
            notes.Remove(note);
            DestroyImmediate(note.gameObject);

            SpawnClue(clueData, true, position);

            ConnectAllClues();
        }

        public void OnClueFound(ClueData clueData)
        {
            notes.FirstOrDefault(note => note.ClueData == clueData)?.RevealNote();
        }

        public void OnMatchedClue(ClueData matchedClue)
        {
            foreach (EvidenceBoardNote note in notes)
            {
                if (!note.IsRevealed)
                {
                    continue;
                }

                // NOTE: not changing this in order to not break it
                List<ClueData> cluesToConnect = note.ClueData.CluesToConnectTo.Intersect(new List<ClueData> { matchedClue } ).ToList();

                if (!cluesToConnect.Any())
                {
                    continue;
                }

                foreach (ClueData clue in cluesToConnect)
                {
                    if (!TryGetConnectionLine(note.ClueData, clue, out ConnectionLine line))
                    {
                        throw Log.Exception($"Connection line between clues <b>{note.ClueData.name}</b> and <b>{clue.name}</b> not found!");
                    }

                    Log.Write($"Connected clues <b>{note.ClueData.name}</b> and <b>{clue.name}</b> on the evidence board!");

                    line.ShowLine();
                }
            }
        }

        private void SpawnClue(ClueData clueData, bool spawnWithPreviousPosition, Vector3 previousPosition = default)
        {
            Vector3 position = GetNewCluePosition();

            if (spawnWithPreviousPosition)
            {
                position = previousPosition;
            }

            EvidenceBoardNote note = SpawnClueOnBoard(clueData.EvidenceBoardNoteType, position);

            note.Initialize(clueData, this);
            note.RevealNote();

            AddClue(note);

            note.ReplaceNoteEvent += OnReplaceNote;
        }

        private Vector3 GetNewCluePosition()
        {
            return new Vector3(0, Random.Range(-boardFrameSize.x / 2, boardFrameSize.x / 2), Random.Range(-boardFrameSize.y / 2, boardFrameSize.y / 2));
        }

        public void RegisterClueListener()
        {
            clueRegistry.RegisterListener(this);
        }

        private EvidenceBoardNote SpawnClueOnBoard(EvidenceBoardNoteType noteType, Vector3 spawnPosition)
        {
            EvidenceBoardNote note = null;

            switch (noteType)
            {
                case EvidenceBoardNoteType.StickyNote:
                    note = Instantiate(evidenceStickyNotePrefab, contentsParent);
                    break;

                case EvidenceBoardNoteType.Picture:
                    note = Instantiate(evidencePicturePrefab, contentsParent);
                    break;

                default:
                    throw Log.Exception($"Clue prefab for type {noteType} is not set!");
            }

            note.transform.localPosition = spawnPosition;

            return note;
        }

        private void ConnectClues(ClueData firstClue, ClueData secondClue)
        {
            EvidenceBoardNote firstNote = GetNoteByClue(firstClue);
            EvidenceBoardNote secondNote = GetNoteByClue(secondClue);

            LineRenderer lineRenderer = Instantiate(connectionLinePrefab, connectionsParent);

            ConnectionLine line = new ConnectionLine(lineRenderer, firstNote.PinTransform.position, secondNote.PinTransform.position, firstClue, secondClue);

            line.ShowLine();

            connections.Add(line);
        }

        private void SpawnAllCluesOnBoard()
        {
            foreach (ClueData clueData in DataStorage.Instance.ClueData)
            {
                if (notes.Any(note => note.ClueData == clueData))
                {
                    continue;
                }

                SpawnClue(clueData, false);
            }
        }

        private EvidenceBoardNote GetNoteByClue(ClueData clueData)
        {
            foreach (EvidenceBoardNote note in notes)
            {
                if (note.ClueData != clueData)
                {
                    continue;
                }

                return note;
            }

            throw Log.Exception($"Note with clue {clueData.ClueHeading} not found!");
        }

        private void HideAllElementsOnBoard()
        {
            HideAllNotesOnBoard();
            HideAllLinesOnBoard();
        }

        private void HideAllLinesOnBoard()
        {
            foreach (ConnectionLine line in connections)
            {
                line.HideLine();
            }
        }

        private void HideAllNotesOnBoard()
        {
            foreach (EvidenceBoardNote note in notes)
            {
                if (note.ClueData.StartsOnEvidenceBoard)
                {
                    continue;
                }

                note.HideNote();
            }
        }

        private void RevealAllCluesOnBoard()
        {
            foreach (EvidenceBoardNote note in notes)
            {
                note.RevealNote();
            }

            foreach (ConnectionLine line in connections)
            {
                line.ShowLine();
            }
        }

        private void ConnectAllCluesOnBoard()
        {
            DestroyAllConnections();

            foreach (EvidenceBoardNote note in notes)
            {
                if (note == null)
                {
                    continue;
                }

                foreach (ClueData secondClue in note.ClueData.CluesToConnectTo)
                {
                    ConnectClues(note.ClueData, secondClue);
                }
            }
        }

        private void DestroyAllConnections()
        {
            for (int i = connections.Count - 1; i >= 0; i--)
            {
                ConnectionLine line = connections[i];

                connections.Remove(line);

                try
                {
                    DestroyImmediate(line.CurrentLine.gameObject);
                }
                catch (Exception e)
                {
                    Log.Warn($"Suppressed: {e}");
                }
            }
        }

        private void RefreshBoard()
        {
            foreach (EvidenceBoardNote note in notes)
            {
                note.RefreshContents();
            }

            ConnectAllCluesOnBoard();

            Log.Write("Board refreshed!");
        }

        private bool TryGetConnectionLine(ClueData firstCLue, ClueData secondClue, out ConnectionLine connectionLine)
        {
            connectionLine = null;

            foreach (ConnectionLine line in connections)
            {
                bool lineExists = (line.FirstClue == firstCLue && line.SecondClue == secondClue) || (line.FirstClue == secondClue && line.SecondClue == firstCLue);

                if (!lineExists)
                {
                    continue;
                }

                connectionLine = line;

                return true;
            }

            return false;
        }

        public void OnInteractWithBoard()
        {
            InteractWithBoardEvent?.Invoke();
        }

        public void SelectNote(EvidenceBoardNote note)
        {
             currentlySelectedNote = note;
        }

        public void DeselectNote()
        {
            currentlySelectedNote = null;
        }

        public void EnableNoteSelection()
        {
            canSelectNote = true;
        }

        public void DisableNoteSelection()
        {
            canSelectNote = false;
        }

        public void LeaveBoard()
        {
            LeaveBoardEvent?.Invoke();
        }

        public void SetEvidenceBoardVirtualCamera(CinemachineVirtualCamera virtualCamera)
        {
            evidenceBoardVirtualCamera = virtualCamera;
        }
    }
}
