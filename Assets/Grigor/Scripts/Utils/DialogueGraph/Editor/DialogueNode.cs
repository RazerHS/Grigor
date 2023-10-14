using UnityEditor.Experimental.GraphView;

namespace RazerCore.Utils.DialogueGraph.Editor
{
    public class DialogueNode : Node
    {
        public string GUID;
        public string DialogueText;
        public bool EntryPoint = false;
    }
}
