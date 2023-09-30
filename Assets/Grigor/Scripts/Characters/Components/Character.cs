using System.Collections.Generic;
using System.Linq;
using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data.Characters;
using UnityEngine;
using UnityEngine.Serialization;

namespace Grigor.Characters.Components
{
    public class Character : CardboardCoreBehaviour
    {
        [SerializeField] private CharacterData characterData;

        private readonly List<CharacterComponent> characterComponents = new();

        protected string CharacterGuid;

        public CharacterData Data => characterData;

        protected override void OnInjected()
        {
            GetComponents(characterComponents);

            foreach (CharacterComponent component in characterComponents)
            {
                component.Initialize(this);
            }

            CharacterGuid = System.Guid.NewGuid().ToString();

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
    }
}
