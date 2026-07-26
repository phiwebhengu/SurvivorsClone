using CloneGame.Player;
using UnityEngine;
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

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            xpBar = root.Q<ProgressBar>("xp-bar");
            levelLabel = root.Q<Label>("level-label");
            timerLabel = root.Q<Label>("timer-label");

            xpBar.title = "";
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
            if (elapsedTime >= gameDuration)
            {
                elapsedTime = gameDuration;
                timerLabel.text = "10:00";

                EndGame();
                enabled = false;
                return;
            }

            elapsedTime += Time.deltaTime;

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

        private void EndGame()
        {
            Debug.Log("Game Complete!");

            Time.timeScale = 0f;

        }
    }
}