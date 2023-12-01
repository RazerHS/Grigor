using System;
using UnityEngine.Serialization;

namespace Grigor.Utils.StoryGraph.Runtime
{
    [Serializable]
    public class NodeLinkData
    {
        public string OutputPortName;
        public string InputPortName;
        [FormerlySerializedAs("StartingNodeGuid")] public string OutputNodeGuid;
        [FormerlySerializedAs("TargetNodeGuid")] public string InputNodeGuid;
    }
}
