using UnityEngine;

namespace CardboardCore.Pooling.Demo
{
    public class BarSphere : MonoBehaviour, IPoolable
    {
        public void OnPop()
        {
            Debug.Log("I'm an instance of Bar, I just Popped out of the pool");
        }

        public void OnPush()
        {
            Debug.Log("I'm an instance of Bar, I just got pushed back in to the pool");
        }
    }
}
