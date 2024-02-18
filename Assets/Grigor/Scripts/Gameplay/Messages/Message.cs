using System;
using Grigor.Characters;
using UnityEngine;

namespace Grigor.Gameplay.Messages
{
    [Serializable]
    public class Message
    {
        [SerializeField] private CharacterData sender;
        [SerializeField] private string title;
        [SerializeField]  private string message;

        public CharacterData Sender => sender;
        public string Title => title;
        public string MessageText => message;
    }
}
