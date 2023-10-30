using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Grigor.Utils.StoryGraph.Runtime
{
    [Serializable]
    public class DialogueNodeData
    {
        public string NodeGuid;
        public string DialogueText;
        public Vector2 Position;
    }
}
