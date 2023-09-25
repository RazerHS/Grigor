using CardboardCore.DI;
using Cinemachine;
using UnityEngine;

namespace Grigor.Overworld.Cameras
{
    [Injectable]
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineBrain cinemachineBrain;

        private CinemachineVirtualCamera activeVirtualCamera;

        public CinemachineVirtualCamera ActiveVirtualCamera => activeVirtualCamera;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void SetActiveVirtualCameraFollow(Transform transform)
        {
            activeVirtualCamera.Follow = transform;
        }

        public void SetActiveVirtualCamera(CinemachineVirtualCamera camera)
        {
            activeVirtualCamera = camera;
        }
    }
}
