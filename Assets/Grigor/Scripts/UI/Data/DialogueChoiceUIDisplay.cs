using System;
using Grigor.Utils.StoryGraph.Runtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Grigor.UI.Data
{
    [Serializable]
    public class DialogueChoiceUIDisplay : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI choiceText;

        [ShowInInspector, ReadOnly] private DialogueChoiceData choiceData;

        public event Action<DialogueChoiceData> ChoiceSelectedEvent;

        public void Initialize(DialogueChoiceData choiceData)
        {
            this.choiceData = choiceData;

            SetChoiceText(choiceData.Text);

            button.onClick.AddListener(OnChoiceClicked);
        }

        public void Dispose()
        {
            button.onClick.RemoveListener(OnChoiceClicked);

            Destroy(gameObject);
        }

        private void OnChoiceClicked()
        {
            ChoiceSelectedEvent?.Invoke(choiceData);
        }

        private void SetChoiceText(string text)
        {
            choiceText.text = text;
        }
    }
}
