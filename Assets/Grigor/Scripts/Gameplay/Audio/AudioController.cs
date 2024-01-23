using System;
using System.Collections.Generic;
using CardboardCore.DI;
using CardboardCore.Utilities;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Grigor.Gameplay.Audio
{
    [Injectable]
    public class AudioController : MonoBehaviour
    {
        [ShowInInspector] private Dictionary<string, EventInstance> currentInstances = new();

        private Bus masterBus;

        private void Awake()
        {
            masterBus = RuntimeManager.GetBus("bus:/");
        }

        public int GetInstanceLength(EventInstance instance)
        {
            instance.getDescription(out EventDescription eventDescription);

            eventDescription.getLength(out int instanceLength);

            float ceil = (float)instanceLength / 1000;

            return Mathf.CeilToInt(ceil);
        }

        public void PlaySound(string name)
        {
            string instanceName = name.Contains("event:/") ? name : $"event:/{name}";
            RuntimeManager.PlayOneShot(instanceName);
        }

        public void PlaySoundWithParameters(string name, params (string parameterName, float value)[] parameters)
        {
            string instanceName = name.Contains("event:/") ? name : $"event:/{name}";

            //doing everything manually without our StartInstance method to not include this key in _instances
            EventInstance instance = RuntimeManager.CreateInstance(instanceName);

            foreach ((string parameterName, float value) in parameters)
            {
                instance.setParameterByName(parameterName, value);
            }

            instance.start();
            instance.release();
        }

        public EventInstance PlaySoundLooping(string name)
        {
            if (!TryCreateInstance(name, out EventInstance instance))
            {
                return default;
            }

            StartInstance(name, instance);

            return instance;
        }

        public EventInstance PlaySound3D(string name, Transform transform)
        {
            if (!TryCreateInstance(name, out EventInstance instance))
            {
                return default;
            }

            instance.set3DAttributes(transform.To3DAttributes());

            StartInstance(name, instance);

            instance.release();

            return instance;
        }

        public EventInstance PlaySound3DLooping(string name, Transform transform)
        {
            if (!TryCreateInstance(name, out EventInstance instance))
            {
                return default;
            }

            instance.set3DAttributes(transform.To3DAttributes());

            StartInstance(name, instance);

            return instance;
        }

        private bool TryCreateInstance(string name, out EventInstance eventInstance)
        {
            eventInstance = default;

            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (currentInstances.ContainsKey(name))
            {
                string instanceName = name.Contains("event:/") ? name : $"event:/{name}";

                currentInstances[name] = RuntimeManager.CreateInstance(instanceName);
                eventInstance = currentInstances[name];

                return true;
            }

            try
            {
                string instanceName = name.Contains("event:/") ? name : $"event:/{name}";
                eventInstance = RuntimeManager.CreateInstance(instanceName);

                return true;
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }

            return default;
        }

        private void StartInstance(string name, EventInstance eventInstance)
        {
            if (!currentInstances.TryGetValue(name, out EventInstance instance))
            {
                currentInstances.Add(name, eventInstance);
            }

            eventInstance.start();
        }

        private bool TryGetEventInstanceFromName(string name, out EventInstance eventInstance)
        {
            if (!currentInstances.ContainsKey(name))
            {
                eventInstance = default;

                return false;
            }

            eventInstance = currentInstances[name];

            return true;
        }

        public void MuteSound(string name, bool fade, float duration = 0.5f)
        {
            if (!TryGetEventInstanceFromName(name, out EventInstance instance))
            {
                return;
            }

            if (fade)
            {
                FadeSound(instance, duration);

                return;
            }

            SetInstanceVolume(instance, 0f);
        }

        public void UnmuteSound(string name, bool unfade, float duration = 0.5f)
        {
            if (!TryGetEventInstanceFromName(name, out EventInstance instance))
            {
                return;
            }

            if (unfade)
            {
                UnfadeSound(instance, duration);

                return;
            }

            SetInstanceVolume(instance, 1f);
        }

        private void FadeSound(EventInstance instance, float duration)
        {
            instance.getVolume(out float currentVolume);

            DOVirtual.Float(currentVolume, 0f, duration, delegate { SetInstanceVolume(instance, currentVolume); });
        }

        private void SetInstanceVolume(EventInstance instance, float value)
        {
            instance.setVolume(value);
        }

        private void UnfadeSound(EventInstance instance, float duration)
        {
            instance.getVolume(out float currentVolume);

            DOVirtual.Float(currentVolume, 1f, duration, delegate { SetInstanceVolume(instance, currentVolume); });
        }

        public void SetMasterVolume(float volume)
        {
            masterBus.setVolume(volume);
        }

        public void StopSound(string name, bool fade = false, float duration = 0.5f)
        {
            if (!TryGetEventInstanceFromName(name, out EventInstance instance))
            {
                return;
            }

            if (fade)
            {
                FadeSound(instance, duration);

                return;
            }

            StopInstance(instance);
        }

        private void StopInstance(EventInstance instance)
        {
            instance.release();
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public void StopAllInstances()
        {
            foreach (KeyValuePair<string, EventInstance> instance in currentInstances)
            {
                StopInstance(instance.Value);
            }
        }

        public void SetInstanceParameter(string name, string parameter, float state)
        {
            if (!TryGetEventInstanceFromName(name, out EventInstance instance))
            {
                return;
            }

            instance.setParameterByName(parameter, state);
        }
    }
}
