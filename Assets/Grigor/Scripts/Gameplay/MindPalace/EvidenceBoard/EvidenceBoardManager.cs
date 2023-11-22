using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data.Clues;
using Grigor.Gameplay.Clues;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Grigor.Gameplay.MindPalace.EvidenceBoard
{
    public class EvidenceBoardManager : CardboardCoreBehaviour, IClueListener
    {
        [SerializeField] private List<EvidenceBoardClue> clues;
        [SerializeField] private Transform contentsParent;
        [SerializeField] private StickyNoteClue stickyNoteCluePrefab;
        [SerializeField] private ImageClue imageCluePrefab;
        [SerializeField] private LineRenderer connectionLinePrefab;
        [SerializeField] private float connectionLineYOffset;
        [SerializeField] private Vector2 boardFrameSize;
        [SerializeField] private Transform boardCenter;

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

        private void AddClue(EvidenceBoardClue clue)
        {
            clues.Add(clue);
        }

        private void RemoveClue(EvidenceBoardClue clue)
        {
            clues.Remove(clue);
        }

        public void OnClueFound(ClueData clueData)
        {
            Vector3 position = GetNewCluePosition();

            EvidenceBoardClue clue = SpawnClue(clueData.EvidenceBoardClueType, position);

            AddClue(clue);
        }

        [Button]
        private void TestSpawnClue()
        {
            Vector3 position = GetNewCluePosition();
            ;
            EvidenceBoardClue clue = SpawnClue(EvidenceBoardClueType.StickyNote, position);

            AddClue(clue);
        }

        [Button]
        private void TestConnectToRandomClues()
        {
            EvidenceBoardClue firstClue = clues[UnityEngine.Random.Range(0, clues.Count)];
            EvidenceBoardClue secondClue = clues[UnityEngine.Random.Range(0, clues.Count)];

            ConnectClues(firstClue, secondClue);
        }

        [Button]
        private void ClearEvidenceFromList()
        {
            clues.Clear();
        }

        private Vector3 GetNewCluePosition()
        {
            return new Vector3(0, UnityEngine.Random.Range(-boardFrameSize.x / 2, boardFrameSize.x / 2), UnityEngine.Random.Range(-boardFrameSize.y / 2, boardFrameSize.y / 2));
        }

        public void RegisterClueListener()
        {
            clueRegistry.RegisterListener(this);
        }

        private EvidenceBoardClue SpawnClue(EvidenceBoardClueType clueType, Vector3 spawnPosition)
        {
            EvidenceBoardClue clue = null;

            switch (clueType)
            {
                case EvidenceBoardClueType.StickyNote:
                    clue = Instantiate(stickyNoteCluePrefab, contentsParent);
                    break;

                case EvidenceBoardClueType.Image:
                    clue = Instantiate(imageCluePrefab, contentsParent);
                    break;

                default:
                    throw Log.Exception($"Clue prefab for type {clueType} is not set!");
            }

            clue.transform.localPosition = spawnPosition;

            return clue;
        }

        private void SpawnConnectionLine(EvidenceBoardClue firstClue, EvidenceBoardClue secondClue)
        {
            LineRenderer line = Instantiate(connectionLinePrefab, contentsParent);

            //getting the position of the pin in the local space of the sticky note
            Vector3 lineStartPosition = contentsParent.InverseTransformPoint(firstClue.transform.TransformPoint(firstClue.PinPosition));
            Vector3 lineEndPosition = contentsParent.InverseTransformPoint(secondClue.transform.TransformPoint(secondClue.PinPosition));

            //offset is required because the y-position is affected by the pin's local position
            lineStartPosition = new Vector3(0, lineStartPosition.y + connectionLineYOffset, lineStartPosition.z);
            lineEndPosition = new Vector3(0, lineEndPosition.y + connectionLineYOffset, lineEndPosition.z);

            line.SetPosition(0, lineStartPosition);
            line.SetPosition(1, lineEndPosition);
        }

        private void ConnectClues(EvidenceBoardClue firstClue, EvidenceBoardClue secondClue)
        {
            SpawnConnectionLine(firstClue, secondClue);
        }
    }
}
