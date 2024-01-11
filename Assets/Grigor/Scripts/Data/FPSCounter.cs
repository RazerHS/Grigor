using UnityEngine;

namespace Grigor.Data
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private float updateInterval = 0.5f;

        private float accum = 0.0f;
        private int frames = 0;
        private float timeleft;
        private float fps;

        private GUIStyle textStyle = new GUIStyle();

        private void Awake()
        {
            timeleft = updateInterval;

            textStyle.fontStyle = FontStyle.Bold;
            textStyle.normal.textColor = Color.white;
        }

        private void Update()
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;

            ++frames;

            if (!(timeleft <= 0.0))
            {
                return;
            }

            fps = (accum / frames);
            timeleft = updateInterval;

            accum = 0.0f;
            frames = 0;
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(Screen.width - 60, 5, 100, 25), fps.ToString("F2"), textStyle);
        }
    }
}
