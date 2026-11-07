using UnityEngine;

namespace Library.View
{
    public abstract class CommonScene : MonoBehaviour
    {
        public abstract void OnBackKey();

        protected virtual void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape)) OnBackKey();
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}