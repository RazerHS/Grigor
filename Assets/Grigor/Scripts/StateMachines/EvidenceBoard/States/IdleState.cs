using CardboardCore.DI;
using CardboardCore.StateMachines;
using Grigor.Gameplay.EvidenceBoard;
using Grigor.UI;
using Grigor.UI.Widgets;

namespace Grigor.StateMachines.EvidenceBoard.States
{
    public class IdleState : State<EvidenceBoardStateMachine>
    {
        [Inject] private EvidenceBoardManager evidenceBoardManager;
        [Inject] private UIManager uiManager;

        private EvidenceBoardWidget evidenceBoardWidget;

        protected override void OnEnter()
        {
            evidenceBoardWidget = uiManager.GetWidget<EvidenceBoardWidget>();

            evidenceBoardManager.InteractWithBoardEvent += OnInteractWithBoard;

            evidenceBoardManager.LeaveBoard();

            evidenceBoardWidget.Hide();
        }

        protected override void OnExit()
        {
            evidenceBoardManager.InteractWithBoardEvent -= OnInteractWithBoard;

            evidenceBoardWidget.Show();
        }

        private void OnInteractWithBoard()
        {
            owningStateMachine.ToState<SelectNoteState>();
        }
    }
}
