using System;
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
using UnityEngine;
using Random = UnityEngine.Random;

namespace Grigor.Gameplay.EvidenceBoard
{
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

        [ColoredBoxGroup("Creating", false, true), Button(ButtonSizes.Large)] private void HideAllClues() => HideAllElementsOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large)] private void RevealAllClues() => RevealAllCluesOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large)] private void ConnectAllClues() => ConnectAllCluesOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large)] private void HideAllLines() => HideAllLinesOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large)] private void Refresh() => RefreshBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large), GUIColor(1f, 0f, 0f)] private void SpawnAllClues() => SpawnAllCluesOnBoard();
        [ColoredBoxGroup("Creating"), Button(ButtonSizes.Large), GUIColor(1f, 0f, 0f)] private void ClearAllClues() => RemoveAllCluesFromBoard();

        [Inject] ClueRegistry clueRegistry;

        protected override void OnInjected()
        {
            RegisterClueListener();

            notes.ForEach(note => note.InitializeNoteContents());

            HideAllElementsOnBoard();
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

        public void OnMatchedClues(List<ClueData> matchedClues)
        {
            foreach (EvidenceBoardNote note in notes)
            {
                if (!note.IsRevealed)
                {
                    continue;
                }

                List<ClueData> cluesToConnect = note.ClueData.CluesToConnectTo.Intersect(matchedClues).ToList();

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

        private void SpawnClue(ClueData clueData)
        {
            Vector3 position = GetNewCluePosition();

            EvidenceBoardNote note = SpawnClueOnBoard(clueData.EvidenceBoardNoteType, position);

            note.Initialize(clueData);
            note.RevealNote();

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
    }
}
