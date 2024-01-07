using CardboardCore.DI;
using CardboardCore.Utilities;
using Grigor.Data.Tasks;
using Grigor.Gameplay.Tasks;
using UnityEngine;

namespace Grigor.Gameplay.Interacting.Components
{
    public class CompleteTaskInteractable : InteractableComponent
    {
        [SerializeField] private TaskData taskData;

        [Inject] private TaskManager taskManager;

        public TaskData TaskData => taskData;

        protected override void OnInitialized()
        {
            if (taskData == null)
            {
                throw Log.Exception($"Task data is not set for <b>{name}</b>!");
            }
        }

        protected override void OnInteractEffect()
        {
            taskManager.CompleteTask(taskData);

            EndInteract();
        }
    }
}
