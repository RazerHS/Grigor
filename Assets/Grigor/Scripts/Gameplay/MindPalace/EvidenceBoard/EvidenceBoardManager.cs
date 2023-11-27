using System.Collections.Generic;
using System.Linq;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data;
using Grigor.Data.Clues;
using Grigor.Gameplay.Clues;
using Grigor.Utils;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Grigor.Gameplay.MindPalace.EvidenceBoard
{
    public class EvidenceBoardManager : CardboardCoreBehaviour, IClueListener
    {
        [SerializeField, ColoredBoxGroup("Board", false, true)] private List<EvidenceBoardNote> notes;
        [SerializeField, ColoredBoxGroup("Board", false, true)] private List<ConnectionLine> connections;
        [SerializeField, ColoredBoxGroup("Board")] private Vector2 boardFrameSize;

        [SerializeField, ColoredBoxGroup("References", false, true)] private Transform contentsParent;
        [SerializeField, ColoredBoxGroup("References")] private LineRenderer connectionLinePrefab;
        [SerializeField, ColoredBoxGroup("References")] private EvidenceBoardNote evidenceStickyNotePrefab;
        [SerializeField, ColoredBoxGroup("References")] private EvidenceBoardNote evidencePicturePrefab;

        [ColoredBoxGroup("Creating", false, true), Button(ButtonSizes.Large)] private void HideAllClues() => HideAllCluesOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large)] private void RevealAllClues() => RevealAllCluesOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large)] private void ConnectAllClues() => ConnectAllCluesOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large)] private void Refresh() => RefreshBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large), GUIColor(1f, 0f, 0f)] private void SpawnAllClues() => SpawnAllCluesOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large), GUIColor(1f, 0f, 0f)] private void ClearAllClues() => RemoveAllCluesFromBoard();

        [SerializeField, HideInInspector] private DataStorage dataStorage;

        [Inject] ClueRegistry clueRegistry;

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            if (dataStorage == null)
            {
                dataStorage = Helper.LoadAsset("DataStorage", dataStorage);
            }

            dataStorage.OnDataRefreshed += RefreshBoard;

            RefreshBoard();
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            if (!Helper.GetKeyPressed(out KeyCode keyCode))
            {
               return;
            }

            if (keyCode != KeyCode.S)
            {
                return;
            }

            RefreshBoard();
        }

        [OnInspectorDispose]
        private void OnInspectorDispose()
        {
            dataStorage.OnDataRefreshed -= RefreshBoard;
        }

        protected override void OnInjected()
        {
            RegisterClueListener();

            notes.ForEach(note => note.InitializeNoteContents());
        }

        protected override void OnReleased()
        {

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

        public void OnClueFound(ClueData clueData)
        {
            notes.FirstOrDefault(note => note.ClueData == clueData)?.RevealNote();
        }

        private void SpawnClue(ClueData clueData)
        {
            Vector3 position = GetNewCluePosition();

            EvidenceBoardNote note = SpawnClueOnBoard(clueData.EvidenceBoardNoteType, position);

            note.Initialize(clueData);

            AddClue(note);
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

            LineRenderer lineRenderer = Instantiate(connectionLinePrefab, contentsParent);

            ConnectionLine line = new ConnectionLine(lineRenderer, firstNote.PinTransform.position, secondNote.PinTransform.position, firstClue, secondClue);

            connections.Add(line);
        }

        private void SpawnAllCluesOnBoard()
        {
            if (dataStorage == null)
            {
                return;
            }

            foreach (ClueData clueData in dataStorage.ClueData)
            {
                SpawnClue(clueData);
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

        private void HideAllCluesOnBoard()
        {
            foreach (EvidenceBoardNote note in notes)
            {
                note.HideNote();
            }
        }

        private void RevealAllCluesOnBoard()
        {
            foreach (EvidenceBoardNote note in notes)
            {
                note.RevealNote();
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

                DestroyImmediate(line.CurrentLine.gameObject);
            }
        }

        private void RefreshBoard()
        {
            foreach (EvidenceBoardNote note in notes)
            {
                note.RefreshContents();
            }

            ConnectAllClues();

            Log.Write("Board refreshed!");
        }
    }
}
