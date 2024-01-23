using UnityEngine;

namespace Grigor.Gameplay.Objects
{
    public class SodaCan : MonoBehaviour
    {
        [SerializeField] private new Rigidbody rigidbody;

        public Rigidbody Rigidbody => rigidbody;
    }
}
