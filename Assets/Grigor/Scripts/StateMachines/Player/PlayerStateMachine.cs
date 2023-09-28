using CardboardCore.StateMachines;
using Grigor.Characters.Components.Player;
using Grigor.StateMachines.Player.States;

namespace Grigor.StateMachines.Player
{
    public class PlayerStateMachine : StateMachine
    {
        public readonly PlayerController Owner;

        public PlayerStateMachine(PlayerController owner, bool enableDebugging) : base(enableDebugging)
        {
            Owner = owner;

            SetInitialState<IdleState>();

            AddStaticTransition<IdleState, FreeRoamState>();
            AddStaticTransition<FreeRoamState, BeginInteractionState>();

            AddFreeFlowTransition<BeginInteractionState, EndInteractionState>();

            AddStaticTransition<EndInteractionState, FreeRoamState>();
        }
    }
}
