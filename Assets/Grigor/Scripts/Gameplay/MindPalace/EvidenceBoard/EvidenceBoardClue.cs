using UnityEngine;

namespace Grigor.Gameplay.MindPalace.EvidenceBoard
{
    public class EvidenceBoardClue : MonoBehaviour
    {
        [SerializeField] private Transform pin;

        public Vector3 PinPosition => pin.localPosition;
    }
}
