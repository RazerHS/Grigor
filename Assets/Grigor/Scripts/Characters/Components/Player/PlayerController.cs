using CardboardCore.DI;
using Grigor.StateMachines.Player;

namespace Grigor.Characters.Components.Player
{
    public class PlayerController : CharacterController
    {
        [Inject] private CharacterRegistry characterRegistry;

        private PlayerStateMachine playerStateMachine;

        public PlayerMovement Movement { get; private set; }
        public PlayerLook Look { get; private set; }
        public PlayerInteract Interact { get; private set; }

        protected override void OnInitialized()
        {
            DontDestroyOnLoad(this);

            Movement = GetCharacterComponent<PlayerMovement>();
            Look = GetCharacterComponent<PlayerLook>();
            Interact = GetCharacterComponent<PlayerInteract>();

            characterRegistry.RegisterCharacter(CharacterGuid, this);
        }

        protected override void OnDisposed()
        {
            playerStateMachine.Stop();

            characterRegistry.UnregisterCharacter(CharacterGuid);
        }

        public void StartStateMachine()
        {
            playerStateMachine = new PlayerStateMachine(this, true);

            playerStateMachine.Start();
        }
    }
}
