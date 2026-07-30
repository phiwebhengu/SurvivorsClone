using CloneGame.Combat;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace CloneGame.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string gameScene = "MainScene";

        private Button playButton;
        private Button upgradeButton;
        private Button settingsButton;
        private Button creditsButton;
        private Button quitButton;

        private VisualElement homePanel;
        private VisualElement settingsPanel;
        private VisualElement upgradesPanel;
        private VisualElement creditsPanel;

        [SerializeField] private WeaponUpgrade[] upgrades;
        ScrollView upgradeList;

        private void Awake()
        {
            UIDocument document = GetComponent<UIDocument>();

            VisualElement root = document.rootVisualElement;

            playButton = root.Q<Button>("play-button");
            upgradeButton = root.Q<Button>("upgrade-button");
            settingsButton = root.Q<Button>("settings-button");
            creditsButton = root.Q<Button>("credits-button");
            quitButton = root.Q<Button>("quit-button");

            homePanel = root.Q<VisualElement>("home-panel");
            settingsPanel = root.Q<VisualElement>("settings-panel");
            upgradesPanel = root.Q<VisualElement>("upgrades-panel");
            creditsPanel = root.Q<VisualElement>("credits-panel");

            Button settingsBack = root.Q<Button>("settings-back");
            Button upgradeBack = root.Q<Button>("upgrade-back");
            Button creditsBack =root.Q<Button>("credits-back");

            upgradeList = root.Q<ScrollView>("upgrade-list");

            playButton.clicked += PlayGame;
            upgradeButton.clicked += OpenUpgrades;
            settingsButton.clicked += OpenSettings;
            creditsButton.clicked += OpenCredits;
            quitButton.clicked += QuitGame;

            settingsBack.clicked += () => ShowPanel(homePanel);
            upgradeBack.clicked += () => ShowPanel(homePanel);
            creditsBack.clicked += () => ShowPanel(homePanel);

            PopulateUpgradeList();
        }

        private void ShowPanel(VisualElement panel)
        {
            homePanel.style.display = DisplayStyle.None;
            settingsPanel.style.display = DisplayStyle.None;
            upgradesPanel.style.display = DisplayStyle.None;
            creditsPanel.style.display = DisplayStyle.None;

            panel.style.display = DisplayStyle.Flex;
        }

        private void PlayGame()
        {
            SceneManager.LoadScene(gameScene);
        }

        private void OpenSettings()
        {
            ShowPanel(settingsPanel);
        }

        private void OpenUpgrades()
        {
            ShowPanel(upgradesPanel);
        }

        private void OpenCredits()
        {
            ShowPanel(creditsPanel);
        }

        private void PopulateUpgradeList()
        {
            upgradeList.Clear();

            foreach (WeaponUpgrade upgrade in upgrades)
            {
                var card = new VisualElement();
                card.AddToClassList("upgrade-card");

                var title = new Label(upgrade.upgradeName);
                title.AddToClassList("upgrade-name");

                var desc = new Label(upgrade.description);

                var value = new Label($"{upgrade.type} +{upgrade.value}");

                card.Add(title);
                card.Add(desc);
                card.Add(value);

                upgradeList.Add(card);
            }
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}