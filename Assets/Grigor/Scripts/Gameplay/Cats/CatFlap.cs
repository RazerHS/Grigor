using System;
using UnityEngine;

namespace Grigor.Gameplay.Cats
{
    public class CatFlap : MonoBehaviour
    {
        [SerializeField] private float clampAngle = 80f;
        [SerializeField] private new Rigidbody rigidbody;

        private Vector3 initialRotation;
        private Vector3 originalPosition;

        private Transform flapTransform;

        private void Awake()
        {
            flapTransform = transform;

            initialRotation = flapTransform.rotation.eulerAngles;
            originalPosition = flapTransform.position;
        }

        private void Update()
        {
            if (flapTransform.rotation.x >= clampAngle || flapTransform.rotation.x >= -clampAngle)
            {
                rigidbody.velocity *= -1;
            }

            float clampedX = Mathf.Clamp(flapTransform.rotation.eulerAngles.x, -clampAngle, clampAngle);

            flapTransform.rotation = Quaternion.Euler(clampedX, initialRotation.y, initialRotation.z);

            flapTransform.position = originalPosition;
        }
    }
}
