using System;
using Grigor.Data.Clues;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Grigor.Gameplay.MindPalace.EvidenceBoard
{
    [Serializable]
    public class ConnectionLine
    {
        [SerializeField] private LineRenderer currentLine;
        [SerializeField, ReadOnly, LabelText("Origin Clue")] private ClueData firstClue;
        [SerializeField, ReadOnly, LabelText("Connected To")] private ClueData secondClue;

        public ClueData FirstClue => firstClue;
        public ClueData SecondClue => secondClue;
        public LineRenderer CurrentLine => currentLine;

        public ConnectionLine(LineRenderer currentLine, Vector3 startPosition, Vector3 endPosition, ClueData firstClue, ClueData secondClue)
        {
            this.currentLine = currentLine;
            this.firstClue = firstClue;
            this.secondClue = secondClue;

            SetLineStartAndEnd(startPosition, endPosition);
        }

        public void HideLine()
        {
            currentLine.gameObject.SetActive(false);
        }

        public void ShowLine()
        {
            currentLine.gameObject.SetActive(true);
        }

        private void SetLineStartAndEnd(Vector3 startPosition, Vector3 endPosition)
        {
            currentLine.SetPosition(0, startPosition);
            currentLine.SetPosition(1, endPosition);
        }
    }
}
