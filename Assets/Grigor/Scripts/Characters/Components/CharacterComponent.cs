using CardboardCore.DI;
using UnityEngine;

namespace Grigor.Characters.Components
{
    public abstract class CharacterComponent : MonoBehaviour
    {
        protected bool IsPaused;
        protected CharacterController CharacterController;

        public CharacterController Owner { get; private set; }

        public void Initialize(CharacterController character)
        {
            Injector.Inject(this);

            Owner = character;

            OnInitialized();
        }

        public void Dispose()
        {
            OnDisposed();

            Injector.Release(this);
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Unpause()
        {
            IsPaused = false;
        }

        protected virtual void OnInitialized() { }

        protected virtual void OnDisposed() { }
    }

    public class PlayerCharacter
    {
    }
}
