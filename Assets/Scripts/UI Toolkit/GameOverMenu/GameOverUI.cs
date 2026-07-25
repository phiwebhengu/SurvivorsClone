using CloneGame.Combat;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace CloneGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private Health playerHealth;

        private VisualElement overlay;
        private Button retryButton;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            overlay = root.Q<VisualElement>("game-over-overlay");
            retryButton = root.Q<Button>("retry-button");

            overlay.style.display = DisplayStyle.None;

            retryButton.clicked += Retry;
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

        private void HandlePlayerDied()
        {
            overlay.style.display = DisplayStyle.Flex;
            Time.timeScale = 0f;
        }

        private void Retry()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
