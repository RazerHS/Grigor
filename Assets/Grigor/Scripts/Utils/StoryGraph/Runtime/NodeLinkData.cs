using System;

namespace Grigor.Utils.StoryGraph.Runtime
{
    [Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string PortName;
        public string TargetNodeGuid;
    }
}
