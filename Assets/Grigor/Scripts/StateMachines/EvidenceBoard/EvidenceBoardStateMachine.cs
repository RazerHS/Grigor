using CardboardCore.StateMachines;
using Grigor.StateMachines.EvidenceBoard.States;

namespace Grigor.StateMachines.EvidenceBoard
{
    public class EvidenceBoardStateMachine : StateMachine
    {
        public EvidenceBoardStateMachine(bool enableDebugging) : base(enableDebugging)
        {
            SetInitialState<IdleState>();

            AddFreeFlowTransition<IdleState, SelectNoteState>();
            AddFreeFlowTransition<SelectNoteState, IdleState>();

            AddFreeFlowTransition<SelectNoteState, NoteSelectedState>();
            AddFreeFlowTransition<NoteSelectedState, SelectNoteState>();

        }
    }
}
