using UnityEngine;

namespace cowsins
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance { get; private set; }

        public static bool isPaused { get; private set; }

        [HideInInspector]public PlayerStats stats;

        [SerializeField] private CanvasGroup menu;

        [SerializeField] private float fadeSpeed;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this);
            else Instance = this;

            isPaused = false;
            menu.gameObject.SetActive(false);
            menu.alpha = 0;
        }
        
        private void Update()
        {
            if (InputManager.pausing) isPaused = !isPaused;

            if (!isPaused) stats.CheckIfCanGrantControl();
            else stats.LoseControl();
            
            if (isPaused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true; 
                if (!menu.gameObject.activeSelf)
                {
                    menu.gameObject.SetActive(true);
                    menu.alpha = 0; 
                }
                if (menu.alpha < 1) menu.alpha += Time.deltaTime * fadeSpeed;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                menu.alpha -= Time.deltaTime * fadeSpeed;
                if (menu.alpha <= 0) menu.gameObject.SetActive(false);
            }
        }

        public void UnPause() => isPaused = false;

        public void QuitGame() => Application.Quit();

        public static void TogglePause() =>isPaused = !isPaused;

    }
}
