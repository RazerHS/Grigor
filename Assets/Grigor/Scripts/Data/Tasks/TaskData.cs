using System;
using RazerCore.Utils.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Data.Tasks
{
    public class TaskData : ScriptableObjectData
    {
        [SerializeField, ColoredBoxGroup("Task Data", false, true), LabelText("Name")] private string taskName;
        [SerializeField, ColoredBoxGroup("Task Data"), LabelText("Description")] private string taskDescription;

        [ShowInInspector, ColoredBoxGroup("Completion", false, true), OnValueChanged(nameof(ToggleStartTask))] private bool started;
        [ShowInInspector, ColoredBoxGroup("Completion"), OnValueChanged(nameof(ToggleCompleteTask))] private bool isCompleted;

        public string TaskName => taskName;
        public string TaskDescription => taskDescription;
        public bool Started => started;
        public bool IsCompleted => isCompleted;

        public event Action TaskStartedEvent;
        public event Action TaskCompleteEvent;

        public event Action<TaskData> TaskStartedInEditorEvent;
        public event Action<TaskData> TaskCompletedInEditorEvent;

        public void ResetTask()
        {
            started = false;
            isCompleted = false;
        }

        public void CompleteTask()
        {
            if (isCompleted)
            {
                return;
            }

            isCompleted = true;

            TaskCompleteEvent?.Invoke();
        }

        public void StartTask()
        {
            if (started)
            {
                return;
            }

            started = true;

            TaskStartedEvent?.Invoke();
        }

        private void ToggleStartTask()
        {
            if (!started)
            {
                return;
            }

            TaskStartedEvent?.Invoke();

            TaskStartedInEditorEvent?.Invoke(this);
        }

        private void ToggleCompleteTask()
        {
            if (!isCompleted)
            {
                return;
            }

            TaskCompleteEvent?.Invoke();

            TaskCompletedInEditorEvent?.Invoke(this);
        }
    }
}
