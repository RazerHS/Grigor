using TMPro;
using UnityEngine;

namespace Grigor.UI.Data
{
    public class MessageUIDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI senderText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;

        public void Initialize(string sender, string title, string message)
        {
            senderText.text = sender;
            titleText.text = title;
            messageText.text = message;
        }
    }
}
