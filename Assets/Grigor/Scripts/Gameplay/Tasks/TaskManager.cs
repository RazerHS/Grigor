using System.Collections.Generic;
using CardboardCore.DI;
using Grigor.Data;
using Grigor.Data.Tasks;
using Grigor.UI;
using Grigor.UI.Widgets;
using Sirenix.OdinInspector;

namespace Grigor.Gameplay.Tasks
{
    [Injectable]
    public class TaskManager : CardboardCoreBehaviour
    {
        [Inject] private UIManager uiManager;

        [ShowInInspector, ReadOnly] private List<TaskData> currentTasks = new List<TaskData>();
        private TasksWidget tasksWidget;

        protected override void OnInjected()
        {
            tasksWidget = uiManager.GetWidget<TasksWidget>();

            DataStorage.Instance.TaskData.ForEach(taskData =>
            {
                taskData.ResetTask();

                taskData.TaskStartedInEditorEvent += OnTaskStartedInEditor;
                taskData.TaskCompletedInEditorEvent += OnTaskCompletedInEditor;
            });
        }

        protected override void OnReleased()
        {
            DataStorage.Instance.TaskData.ForEach(taskData =>
            {
                taskData.TaskStartedInEditorEvent -= OnTaskStartedInEditor;
                taskData.TaskCompletedInEditorEvent -= OnTaskCompletedInEditor;
            });
        }

        public void CompleteTask(TaskData taskData)
        {
            if (taskData.IsCompleted)
            {
                return;
            }

            if (!currentTasks.Contains(taskData))
            {
                return;
            }

            currentTasks.Remove(taskData);

            taskData.CompleteTask();
            tasksWidget.OnTaskCompleted(taskData);
        }

        public void StartTask(TaskData taskData)
        {
            if (taskData.IsCompleted)
            {
                return;
            }

            if (currentTasks.Contains(taskData))
            {
                return;
            }

            currentTasks.Add(taskData);

            taskData.StartTask();
            tasksWidget.OnTaskStarted(taskData);
        }

        private void OnTaskStartedInEditor(TaskData taskData)
        {
            StartTask(taskData);
        }

        private void OnTaskCompletedInEditor(TaskData taskData)
        {
            CompleteTask(taskData);
        }
    }
}
