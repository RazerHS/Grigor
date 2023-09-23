using CardboardCore.DI;

namespace Grigor.Characters.Components
{
    public abstract class CharacterComponent : CardboardCoreBehaviour
    {
        protected override InjectTiming MyInjectTiming => InjectTiming.Start;

        public CharacterController Owner { get; private set; }

        public void Initialize(CharacterController character)
        {
            Owner = character;
        }
    }
}
