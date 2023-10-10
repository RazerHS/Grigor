using UnityEditor.Experimental.GraphView;

namespace RazerCore.Utils.Editor.DialogueGraph
{
    public class DialogueNode : Node
    {
        public string GUID;
        public string DialogueText;
        public bool EntryPoint = false;
    }
}
