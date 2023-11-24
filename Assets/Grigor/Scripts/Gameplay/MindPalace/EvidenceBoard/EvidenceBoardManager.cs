using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data.Clues;
using Grigor.Gameplay.Clues;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.MindPalace.EvidenceBoard
{
    public class EvidenceBoardManager : CardboardCoreBehaviour, IClueListener
    {
        [SerializeField] private List<EvidenceBoardNote> clues;
        [SerializeField] private Transform contentsParent;
        [SerializeField, ColoredBoxGroup("References", false, 0.5f, 0.5f, 0.1f)] private EvidenceBoardNote evidenceStickyNotePrefab;
        [SerializeField, ColoredBoxGroup("References")] private EvidenceBoardNote evidencePicturePrefab;
        [SerializeField] private LineRenderer connectionLinePrefab;
        [SerializeField] private Vector2 boardFrameSize;
        [SerializeField] private Transform boardCenter;
        [SerializeField] private ClueData testClue;

        [Inject] ClueRegistry clueRegistry;

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(boardCenter.position, new Vector3(0, boardFrameSize.x, boardFrameSize.y));
        }

        protected override void OnInjected()
        {
            RegisterClueListener();
        }

        protected override void OnReleased()
        {

        }

        private void AddClue(EvidenceBoardNote note)
        {
            clues.Add(note);
        }

        private void RemoveClue(EvidenceBoardNote note)
        {
            clues.Remove(note);
        }

        public void OnClueFound(ClueData clueData)
        {
            Vector3 position = GetNewCluePosition();

            EvidenceBoardNote note = SpawnClue(clueData.EvidenceBoardNoteType, position);

            note.Initialize(clueData);
            note.SetHeadingText(clueData.ClueHeading);

            AddClue(note);
        }

        [Button]
        private void TestSpawnClue()
        {
            OnClueFound(testClue);
        }

        [Button]
        private void TestConnectToRandomClues()
        {
            EvidenceBoardNote firstNote = clues[Random.Range(0, clues.Count)];
            EvidenceBoardNote secondNote = clues[Random.Range(0, clues.Count)];

            ConnectClues(firstNote, secondNote);
        }

        [Button]
        private void ClearEvidenceFromList()
        {
            clues.Clear();
        }

        private Vector3 GetNewCluePosition()
        {
            return new Vector3(0, Random.Range(-boardFrameSize.x / 2, boardFrameSize.x / 2), Random.Range(-boardFrameSize.y / 2, boardFrameSize.y / 2));
        }

        public void RegisterClueListener()
        {
            clueRegistry.RegisterListener(this);
        }

        private EvidenceBoardNote SpawnClue(EvidenceBoardNoteType noteType, Vector3 spawnPosition)
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

        private void SpawnConnectionLine(EvidenceBoardNote firstNote, EvidenceBoardNote secondNote)
        {
            LineRenderer line = Instantiate(connectionLinePrefab, contentsParent);

            line.SetPosition(0, firstNote.PinTransform.position);
            line.SetPosition(1, secondNote.PinTransform.position);
        }

        private void ConnectClues(EvidenceBoardNote firstNote, EvidenceBoardNote secondNote)
        {
            SpawnConnectionLine(firstNote, secondNote);
        }
    }
}
