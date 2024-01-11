using UnityEngine;

namespace Grigor.Gameplay.Cats
{
    public class Catnip : MonoBehaviour
    {
        [SerializeField] private new Rigidbody rigidbody;

        public void ThrowCatnip(Vector3 direction, float force)
        {
            rigidbody.AddForce(direction.normalized + (Vector3.up * force), ForceMode.Impulse);
        }

        public void DestroyCatnip()
        {
            Destroy(gameObject);
        }
    }
}
