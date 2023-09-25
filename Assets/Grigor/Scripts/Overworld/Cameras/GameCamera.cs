using CardboardCore.DI;
using Cinemachine;
using UnityEngine;

namespace Grigor.Overworld.Cameras
{
    public class GameCamera : CardboardCoreBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        [Inject] private CameraManager cameraManager;

        protected override InjectTiming MyInjectTiming => InjectTiming.OnEnable;

        protected override void OnInjected()
        {
            cameraManager.SetActiveVirtualCamera(virtualCamera);
        }

        protected override void OnReleased()
        {

        }
    }
}
