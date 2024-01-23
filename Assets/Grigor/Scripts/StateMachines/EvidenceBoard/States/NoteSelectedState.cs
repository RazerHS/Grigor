using System;
using CardboardCore.DI;
using CardboardCore.StateMachines;
using Cinemachine;
using DG.Tweening;
using Grigor.Gameplay.Cameras;
using Grigor.Gameplay.EvidenceBoard;
using Grigor.Input;
using Grigor.UI;
using Grigor.UI.Widgets;
using Grigor.Utils;

namespace Grigor.StateMachines.EvidenceBoard.States
{
    public class NoteSelectedState : State<EvidenceBoardStateMachine>
    {
        [Inject] private EvidenceBoardManager evidenceBoardManager;
        [Inject] private UIManager uiManager;
        [Inject] private PlayerInput playerInput;
        [Inject] private CameraManager cameraManager;

        private bool zoomEnabled;
        private EvidenceBoardWidget evidenceBoardWidget;

        protected override void OnEnter()
        {
            evidenceBoardWidget = uiManager.GetWidget<EvidenceBoardWidget>();

            evidenceBoardWidget.OnBackButtonClickedEvent += OnBackButtonClicked;

            playerInput.ScrollInputStartedEvent += OnScrollInputStarted;

            cameraManager.SetActiveVirtualCamera(evidenceBoardManager.CurrentlySelectedNote.NoteVirtualCamera);

            evidenceBoardManager.DisableNoteSelection();

            Helper.Delay(cameraManager.CurrentBlendTime, EnableZoom);
        }

        protected override void OnExit()
        {
            evidenceBoardWidget.OnBackButtonClickedEvent -= OnBackButtonClicked;

            playerInput.ScrollInputStartedEvent -= OnScrollInputStarted;

            zoomEnabled = false;

            CinemachineVirtualCamera noteCamera = cameraManager.ActiveVirtualCamera;

            cameraManager.SetActiveVirtualCamera(evidenceBoardManager.EvidenceBoardVirtualCamera);

            cameraManager.SetCameraFOV(noteCamera, evidenceBoardManager.MaxFOV);
        }

        private void OnBackButtonClicked()
        {
            evidenceBoardManager.DeselectNote();

            if (Math.Abs(cameraManager.ActiveVirtualCamera.m_Lens.FieldOfView - evidenceBoardManager.MaxFOV) < 0.1f)
            {
                owningStateMachine.ToState<SelectNoteState>();

                return;
            }

            DOVirtual.Float(cameraManager.ActiveVirtualCamera.m_Lens.FieldOfView, evidenceBoardManager.MaxFOV, 0.2f, SetCameraFOV).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                owningStateMachine.ToState<SelectNoteState>();
            });
        }

        private void OnScrollInputStarted(float value)
        {
            if (!zoomEnabled)
            {
                return;
            }

            cameraManager.AdjustCurrentCameraFOV(evidenceBoardManager.ScrollIncrement / value, evidenceBoardManager.MinFOV, evidenceBoardManager.MaxFOV);
        }

        private void EnableZoom()
        {
            zoomEnabled = true;
        }

        private void SetCameraFOV(float value)
        {
            cameraManager.SetCameraFOV(cameraManager.ActiveVirtualCamera, value);
        }
    }
}
