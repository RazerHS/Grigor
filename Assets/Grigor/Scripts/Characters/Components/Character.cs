using System.Collections.Generic;
using System.Linq;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data;
using Grigor.Gameplay.Time;
using UnityEngine;

namespace Grigor.Characters.Components
{
    public class Character : CardboardCoreBehaviour, ITimeEffect
    {
        [SerializeField] private CharacterData characterData;

        [Inject] private TimeEffectRegistry timeEffectRegistry;

        private readonly List<CharacterComponent> characterComponents = new();

        protected string CharacterGuid;

        public CharacterData Data => characterData;

        protected override void OnInjected()
        {
            if (characterData == null)
            {
                throw Log.Exception($"Character data for character {name} is null!");
            }

            GetComponents(characterComponents);

            foreach (CharacterComponent component in characterComponents)
            {
                component.Initialize(this);
            }

            CharacterGuid = System.Guid.NewGuid().ToString();

            RegisterTimeEffect();

            OnInitialized();
        }

        protected override void OnReleased()
        {
            foreach (CharacterComponent component in characterComponents)
            {
                component.Dispose();
            }

            OnDisposed();
        }

        protected virtual void OnInitialized() { }

        protected virtual void OnDisposed() { }

        protected T GetCharacterComponent<T>() where T : CharacterComponent
        {
            T component = characterComponents.FirstOrDefault(component => component is T) as T;

            if (component == null)
            {
                throw Log.Exception($"Could not find component of type {typeof(T)} in characterData {name}!");
            }

            return component;
        }

        public void OnChangedToDay()
        {

        }

        public void OnChangedToNight()
        {

        }

        public void RegisterTimeEffect()
        {
            timeEffectRegistry.Register(this);
        }
    }
}
