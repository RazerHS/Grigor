using System;
using Grigor.Data.Tasks;
using TMPro;
using UnityEngine;

namespace Grigor.UI.Data
{
    public class TaskUIDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI taskNameText;
        [SerializeField] private TextMeshProUGUI taskDescriptionText;

        private TaskData taskData;

        public TaskData TaskData => taskData;

        public void Initialize(TaskData taskData)
        {
            this.taskData = taskData;

            SetTaskName(taskData.TaskName);
            SetTaskDescription(taskData.TaskDescription);
        }

        public void SetTaskName(string taskName)
        {
            taskNameText.text = taskName;
        }

        public void SetTaskDescription(string taskDescription)
        {
            taskDescriptionText.text = taskDescription;
        }
    }
}
