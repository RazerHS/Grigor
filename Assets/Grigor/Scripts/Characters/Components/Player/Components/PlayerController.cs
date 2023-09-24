using CardboardCore.DI;

namespace Grigor.Characters.Components.Player.Components
{
    public class PlayerController : CharacterController
    {
        [Inject] private CharacterRegistry characterRegistry;

        public PlayerMovement Movement { get; private set; }

        protected override void OnInitialized()
        {
            DontDestroyOnLoad(this);

            Movement = GetCharacterComponent<PlayerMovement>();

            characterRegistry.RegisterCharacter(CharacterGuid, this);
        }
    }
}
