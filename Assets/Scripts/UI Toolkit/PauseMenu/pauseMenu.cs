using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace CloneGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class PauseMenu : MonoBehaviour
    {
        private UIDocument document;

        private VisualElement pauseOverlay;

        private Button resumeButton;
        private Button restartButton;
        private Button menuButton;

        private bool isPaused;

        public static PauseMenu Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            document = GetComponent<UIDocument>();

            BindUIElements();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (resumeButton != null)
                resumeButton.clicked -= ResumeGame;

            if (restartButton != null)
                restartButton.clicked -= RestartGame;

            if (menuButton != null)
                menuButton.clicked -= MainMenu;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            document = GetComponent<UIDocument>();
            BindUIElements();

            isPaused = false;
            Time.timeScale = 1f;
        }

        private void BindUIElements()
        {
            if (document == null)
                return;

            var root = document.rootVisualElement;

            pauseOverlay = root.Q<VisualElement>("pause-overlay");

            if (pauseOverlay != null)
            {
                pauseOverlay.AddToClassList("hidden");
                pauseOverlay.focusable = true;
            }

            resumeButton = root.Q<Button>("resume-button");
            restartButton = root.Q<Button>("restart-button");
            menuButton = root.Q<Button>("menu-button");

            if (resumeButton != null)
            {
                resumeButton.clicked -= ResumeGame;
                resumeButton.clicked += ResumeGame;
            }

            if (restartButton != null)
            {
                restartButton.clicked -= RestartGame;
                restartButton.clicked += RestartGame;
            }

            if (menuButton != null)
            {
                menuButton.clicked -= MainMenu;
                menuButton.clicked += MainMenu;
            }
        }

        // Assign this to the Pause action in your PlayerControls
        public void OnPause(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            TogglePause();
        }

        public void TogglePause()
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                Time.timeScale = 0f;

                if (pauseOverlay != null)
                {
                    pauseOverlay.RemoveFromClassList("hidden");
                    pauseOverlay.BringToFront();
                    pauseOverlay.Focus();
                }

            }
            else
            {
                ResumeGame();
            }
        }

        private void ResumeGame()
        {
            isPaused = false;

            Time.timeScale = 1f;

            if (pauseOverlay != null)
                pauseOverlay.AddToClassList("hidden");

            
        }

        private void RestartGame()
        {
            Time.timeScale = 1f;
            isPaused = false;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void MainMenu()
        {
            Time.timeScale = 1f;
            isPaused = false;

            SceneManager.LoadScene("MainMenu");
        }
    }
}