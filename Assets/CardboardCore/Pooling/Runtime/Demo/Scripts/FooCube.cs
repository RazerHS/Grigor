using UnityEngine;

namespace CardboardCore.Pooling.Demo
{
    public class FooCube : MonoBehaviour, IPoolable
    {
        public void OnPop()
        {
            Debug.Log("I'm an instance of Foo, I just Popped out of the pool");
        }

        public void OnPush()
        {
            Debug.Log("I'm an instance of Foo, I just got pushed back in to the pool");
        }
    }
}
