using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Gameplay.Cameras;
using Grigor.Gameplay.EvidenceBoard;
using Grigor.UI;
using Grigor.UI.Widgets;

namespace Grigor.StateMachines.EvidenceBoard.States
{
    public class SelectNoteState : State<EvidenceBoardStateMachine>
    {
        [Inject] private EvidenceBoardManager evidenceBoardManager;
        [Inject] private UIManager uiManager;
        [Inject] private CameraManager cameraManager;

        private EvidenceBoardWidget evidenceBoardWidget;

        protected override void OnEnter()
        {
            evidenceBoardWidget = uiManager.GetWidget<EvidenceBoardWidget>();

            evidenceBoardWidget.OnBackButtonClickedEvent += OnBackButtonClicked;

            foreach (EvidenceBoardNote note in evidenceBoardManager.Notes)
            {
                note.NoteSelectedEvent += OnNoteSelected;
            }

            evidenceBoardManager.EnableNoteSelection();
        }

        protected override void OnExit()
        {
            evidenceBoardWidget.OnBackButtonClickedEvent -= OnBackButtonClicked;

            foreach (EvidenceBoardNote note in evidenceBoardManager.Notes)
            {
                note.NoteSelectedEvent -= OnNoteSelected;
            }
        }

        private void OnNoteSelected(EvidenceBoardNote note)
        {
            evidenceBoardManager.SelectNote(note);

            evidenceBoardManager.EvidenceBoardVirtualCamera.gameObject.SetActive(false);

            owningStateMachine.ToState<NoteSelectedState>();
        }

        private void OnBackButtonClicked()
        {
            owningStateMachine.ToState<IdleState>();
        }
    }
}
