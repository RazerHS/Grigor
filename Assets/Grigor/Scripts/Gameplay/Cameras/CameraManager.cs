using CardboardCore.DI;
using Cinemachine;
using UnityEngine;

namespace Grigor.Gameplay.Cameras
{
    [Injectable, RequireComponent(typeof(CinemachineBrain))]
    public class CameraManager : MonoBehaviour
    {
        private CinemachineBrain cinemachineBrain;

        private CinemachineVirtualCamera activeVirtualCamera;
        private float defaultBlendTime;

        public CinemachineVirtualCamera ActiveVirtualCamera => activeVirtualCamera;
        public float CurrentBlendTime => cinemachineBrain.m_DefaultBlend.m_Time;

        private void Awake()
        {
            cinemachineBrain = GetComponent<CinemachineBrain>();

            defaultBlendTime = cinemachineBrain.m_DefaultBlend.m_Time;

            DontDestroyOnLoad(this);
        }

        public void SetActiveVirtualCameraFollow(Transform transform)
        {
            activeVirtualCamera.Follow = transform;
        }

        public void SetActiveVirtualCamera(CinemachineVirtualCamera camera)
        {
            if (activeVirtualCamera != null)
            {
                activeVirtualCamera.gameObject.SetActive(false);
            }

            camera.gameObject.SetActive(true);

            activeVirtualCamera = camera;
        }

        public void AdjustCurrentCameraFOV(float value, float min, float max)
        {
            activeVirtualCamera.m_Lens.FieldOfView += value;

            activeVirtualCamera.m_Lens.FieldOfView = Mathf.Clamp(activeVirtualCamera.m_Lens.FieldOfView, min, max);
        }

        public void SetCameraFOV(CinemachineVirtualCamera camera, float value)
        {
            camera.m_Lens.FieldOfView = value;
        }

        public void SetBlendTime(float value)
        {
            cinemachineBrain.m_DefaultBlend.m_Time = value;
        }

        public void ResetDefaultBlendTime()
        {
            cinemachineBrain.m_DefaultBlend.m_Time = 0.5f;
        }
    }
}
