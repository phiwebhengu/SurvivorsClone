using CloneGame.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace CloneGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class HUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerExperience playerExperience;

        private ProgressBar xpBar;
        private Label levelLabel;
        private Label timerLabel;

        private float elapsedTime;

        public float ElapsedTime => elapsedTime;
        public int Minutes => Mathf.FloorToInt(elapsedTime / 60f);
        public int Seconds => Mathf.FloorToInt(elapsedTime % 60f);

        [SerializeField] private float gameDuration = 600f;

        private VisualElement victoryOverlay;

        private Label victoryTime;
        private Label victoryLevel;
        private Label victoryKills;
        private Label victoryXP;

        private Button restartButton;
        private Button quitButton;


        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            xpBar = root.Q<ProgressBar>("xp-bar");
            levelLabel = root.Q<Label>("level-label");
            timerLabel = root.Q<Label>("timer-label");

            victoryOverlay = root.Q<VisualElement>("victory-overlay");

            victoryTime = root.Q<Label>("time-value");
            victoryLevel = root.Q<Label>("level-value");
            victoryKills = root.Q<Label>("kills-value");
            victoryXP = root.Q<Label>("xp-value");

            restartButton = root.Q<Button>("restart-button");
            quitButton = root.Q<Button>("quit-button");

            xpBar.title = "";
            restartButton.clicked += RestartGame;
            quitButton.clicked += QuitGame;
        }

        private void OnEnable()
        {
            if (playerExperience != null)
            {
                playerExperience.OnExperienceChanged += UpdateExperience;
                playerExperience.OnLevelUp += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (playerExperience != null)
            {
                playerExperience.OnExperienceChanged -= UpdateExperience;
                playerExperience.OnLevelUp -= UpdateLevel;
            }
        }

        private void Start()
        {
            UpdateLevel(playerExperience.CurrentLevel);
            UpdateExperience(
                playerExperience.CurrentXp,
                playerExperience.XpToNextLevel);
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= gameDuration)
            {
                elapsedTime = gameDuration;

                ShowVictoryScreen();

                enabled = false;
                return;
            }

            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);

            timerLabel.text = $"{minutes:00}:{seconds:00}";
        }

        private void UpdateExperience(float currentXP, float maxXP)
        {
            xpBar.highValue = maxXP;
            xpBar.value = currentXP;
        }

        private void UpdateLevel(int level)
        {
            levelLabel.text = $"Lv. {level}";
        }

        private void ShowVictoryScreen()
        {
            timerLabel.text = "10:00";

            victoryTime.text = timerLabel.text;
            victoryLevel.text = playerExperience.CurrentLevel.ToString();

            victoryKills.text = "0";
            victoryXP.text = playerExperience.CurrentXp.ToString("0");

            victoryOverlay.RemoveFromClassList("hidden");

            Time.timeScale = 0f;
        }

        private void RestartGame()
        {
            Time.timeScale = 1f;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void QuitGame()
        {
            Time.timeScale = 1f;

            SceneManager.LoadScene("MainMenu");
        }
    }
}