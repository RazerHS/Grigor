using CardboardCore.StateMachines;
using Grigor.StateMachines.Player.States;

namespace Grigor.StateMachines.Player
{
    public class PlayerStateMachine : StateMachine
    {
        public readonly Characters.Components.Player.Player Owner;

        public PlayerStateMachine(Characters.Components.Player.Player owner, bool enableDebugging) : base(enableDebugging)
        {
            Owner = owner;

            SetInitialState<IdleState>();

            AddStaticTransition<IdleState, FreeRoamState>();
            AddStaticTransition<FreeRoamState, BeginInteractionState>();
            AddStaticTransition<BeginInteractionState, EndInteractionState>();

            AddFreeFlowTransition<EndInteractionState, BeginInteractionState>();
            AddFreeFlowTransition<EndInteractionState, FreeRoamState>();
        }
    }
}
