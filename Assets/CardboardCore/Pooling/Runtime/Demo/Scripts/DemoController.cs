using System.Collections.Generic;
using System.Threading.Tasks;
using Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CardboardCore.Pooling.Demo
{
    public class DemoController : MonoBehaviour
    {
        // Even tho it's supported to have multiple pool managers, it's wise to keep a single pool manager around
        // An option would be to wrap it in a static instance, or use the Dependency Injection package from CardboardCore for the sake of ease
        private PoolManager poolManager;

        // A pool is created on runtime by the pool manager. It's based on a PoolConfig given by you
        private Pool demoPool;

        private readonly List<FooCube> fooCubes = new List<FooCube>();
        private readonly List<BarSphere> barSpheres = new List<BarSphere>();

        private async void Awake()
        {
            Debug.Log("Pool DemoController: Press \"1\" to spawn a cube, press \"2\" to spawn a sphere, press \"backspace\" to despawn all at once");

            // Create an instance of the PoolManager
            poolManager = new PoolManager();

            // Async retrieve a pool, if this is the first time this specific pool is requested
            Task<Pool> getPoolTask = poolManager.RequestPool("DemoPoolConfig");
            await getPoolTask;

            demoPool = getPoolTask.Result;
        }

        private void Update()
        {
            // Pop a FooCube from the pool, make it a child of this GameObject, and put it on a random position
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                FooCube fooCube = demoPool.Pop<FooCube>(DemoPoolConfigNames.FooCube, transform);

                fooCube.transform.position = new Vector3(
                    Random.Range(-10, 10),
                    Random.Range(-10, 10),
                    Random.Range(-10, 10));

                fooCubes.Add(fooCube);
            }

            // Pop a BarSphere from the pool, make it a child of this GameObject, and put it on a random position
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                BarSphere barSphere = demoPool.Pop<BarSphere>(DemoPoolConfigNames.BarSphere, transform);

                barSphere.transform.position = new Vector3(
                    Random.Range(-10, 10),
                    Random.Range(-10, 10),
                    Random.Range(-10, 10));

                barSpheres.Add(barSphere);
            }

            // Push all FooCubes and BarSpheres back in the pool
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                for (int i = fooCubes.Count - 1; i >= 0; i--)
                {
                    demoPool.Push(fooCubes[i]);
                }

                fooCubes.Clear();

                for (int i = barSpheres.Count - 1; i >= 0; i--)
                {
                    demoPool.Push(barSpheres[i]);
                }

                barSpheres.Clear();
            }
        }
    }
}
