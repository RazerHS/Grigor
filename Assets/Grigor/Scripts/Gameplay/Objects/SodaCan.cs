using UnityEngine;

namespace Grigor.Gameplay.Objects
{
    public class SodaCan : MonoBehaviour
    {
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private new Renderer renderer;

        public Rigidbody Rigidbody => rigidbody;
        public Renderer Renderer => renderer;
    }
}
