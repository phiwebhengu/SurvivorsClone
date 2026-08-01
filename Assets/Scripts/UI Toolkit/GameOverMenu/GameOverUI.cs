using CloneGame.Combat;
using CloneGame.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace CloneGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class GameOverUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Health playerHealth;
        [SerializeField] private PlayerExperience playerExperience;
        [SerializeField] private HUD hud;

        private VisualElement overlay;
        private VisualElement root;

        private Label timeValue;
        private Label levelValue;
        private Label killsValue;
        private Label xpValue;

        private Button retryButton;
        private Button menuButton;

        private void Awake()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            overlay = root.Q<VisualElement>("game-over-overlay");

            timeValue = root.Q<Label>("time-value");
            levelValue = root.Q<Label>("level-value");
            killsValue = root.Q<Label>("kills-value");
            xpValue = root.Q<Label>("xp-value");

            retryButton = root.Q<Button>("retry-button");
            menuButton = root.Q<Button>("menu-button");

            if (overlay != null)
            {
                overlay.AddToClassList("hidden");
                root.AddToClassList("hidden");
            }

                if (retryButton != null)
                retryButton.clicked += Retry;

            if (menuButton != null)
                menuButton.clicked += MainMenu;
        }

        private void OnEnable()
        {
            if (playerHealth != null)
                playerHealth.OnDied += HandlePlayerDied;
        }

        private void OnDisable()
        {
            if (playerHealth != null)
                playerHealth.OnDied -= HandlePlayerDied;
        }

        private void OnDestroy()
        {
            if (retryButton != null)
                retryButton.clicked -= Retry;

            if (menuButton != null)
                menuButton.clicked -= MainMenu;
        }

        private void HandlePlayerDied()
        {
            if (hud != null && timeValue != null)
            {
                timeValue.text = $"{hud.Minutes:00}:{hud.Seconds:00}";
            }

            if (playerExperience != null)
            {
                if (levelValue != null)
                    levelValue.text = playerExperience.CurrentLevel.ToString();

                if (xpValue != null)
                    xpValue.text = playerExperience.CurrentXp.ToString("0");
            }

        
            if (killsValue != null)
                killsValue.text = "0";

            if (overlay != null)
            {
                overlay.BringToFront();
                overlay.RemoveFromClassList("hidden");
                root.RemoveFromClassList("hidden");
            }

            Time.timeScale = 0f;
        }

        private void Retry()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void MainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}