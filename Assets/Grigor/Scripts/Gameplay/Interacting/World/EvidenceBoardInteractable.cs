using CardboardCore.DI;
using Cinemachine;
using Grigor.Gameplay.Cameras;
using Grigor.Gameplay.EvidenceBoard;
using Grigor.Gameplay.Interacting.Components;
using Grigor.Utils;
using RazerCore.Utils.Attributes;
using UnityEngine;

namespace Grigor.Gameplay.World.Components
{
    public class EvidenceBoardInteractable : InteractableComponent
    {
        [SerializeField, ColoredBoxGroup("Evidence Board", false, true)] private CinemachineVirtualCamera evidenceBoardVirtualCamera;
        [SerializeField, ColoredBoxGroup("Evidence Board")] private EvidenceBoardManager evidenceBoardManager;
        [SerializeField, ColoredBoxGroup("Evidence Board"), Range(0f, 5f)] private float cameraBlendTime = 0.5f;

        [Inject] private CameraManager cameraManager;

        private CinemachineVirtualCamera playerVirtualCamera;

        protected override void OnInitialized()
        {
            evidenceBoardManager.SetEvidenceBoardVirtualCamera(evidenceBoardVirtualCamera);
        }

        protected override void OnInteractEffect()
        {
            playerVirtualCamera = cameraManager.ActiveVirtualCamera;

            Cursor.visible = true;

            cameraManager.SetBlendTime(cameraBlendTime);

            cameraManager.SetActiveVirtualCamera(evidenceBoardVirtualCamera);

            evidenceBoardManager.OnInteractWithBoard();

            parentInteractable.PauseInteractable();

            evidenceBoardManager.LeaveBoardEvent += OnLeaveBoard;
        }

        private void OnLeaveBoard()
        {
            evidenceBoardManager.LeaveBoardEvent -= OnLeaveBoard;

            parentInteractable.UnpauseInteractable();

            Helper.Delay(cameraBlendTime, EndInteract);

            cameraManager.SetActiveVirtualCamera(playerVirtualCamera);

            cameraManager.ResetDefaultBlendTime();
        }
    }
}
