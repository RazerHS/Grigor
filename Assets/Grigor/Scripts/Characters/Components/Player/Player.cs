using CardboardCore.DI;
using Grigor.StateMachines.Player;

namespace Grigor.Characters.Components.Player
{
    public class Player : Character
    {
        [Inject] private CharacterRegistry characterRegistry;

        private PlayerStateMachine playerStateMachine;

        public PlayerMovement Movement { get; private set; }
        public PlayerLook Look { get; private set; }
        public PlayerInteract Interact { get; private set; }
        public PlayerEffects Effects { get; private set; }

        protected override void OnInitialized()
        {
            DontDestroyOnLoad(this);

            Movement = GetCharacterComponent<PlayerMovement>();
            Look = GetCharacterComponent<PlayerLook>();
            Interact = GetCharacterComponent<PlayerInteract>();
            Effects = GetCharacterComponent<PlayerEffects>();

            characterRegistry.RegisterCharacter(CharacterGuid, this);
        }

        protected override void OnDisposed()
        {
            characterRegistry.UnregisterCharacter(CharacterGuid);

            playerStateMachine?.Stop();
        }

        public void StartStateMachine()
        {
            playerStateMachine = new PlayerStateMachine(this, true);

            playerStateMachine.Start();
        }
    }
}
