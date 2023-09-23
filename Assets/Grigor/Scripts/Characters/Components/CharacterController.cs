﻿using System.Collections.Generic;
using System.Linq;
using CardboardCore.DI;
using CardboardCore.Utilities;

namespace Grigor.Characters.Components
{
    public class CharacterController : CardboardCoreBehaviour
    {
        private readonly List<CharacterComponent> characterComponents = new();

        protected override void OnInjected()
        {
            GetComponents(characterComponents);

            foreach (CharacterComponent component in characterComponents)
            {
                component.Initialize(this);
            }
        }

        protected override void OnReleased()
        {

        }

        protected T GetCharacterComponent<T>() where T : CharacterComponent
        {
            T component = characterComponents.FirstOrDefault(component => component is T) as T;

            if (component == null)
            {
                throw Log.Exception($"Could not find component of type {typeof(T)} in character {name}!");
            }

            return component;
        }
    }
}