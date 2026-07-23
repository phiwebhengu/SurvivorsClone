using System;
using System.Collections.Generic;
using CloneGame.Combat;
using UnityEngine;
using UnityEngine.UIElements;

namespace CloneGame.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class LevelUpUI : MonoBehaviour
    {
        private UIDocument uiDocument;

        private VisualElement overlay;

        private Label title;
        private Label subtitle;

        private Button button1;
        private Button button2;
        private Button button3;

        private readonly List<WeaponUpgrade> currentChoices = new();

        private Action<WeaponUpgrade> onUpgradeSelected;

        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();

            VisualElement root = uiDocument.rootVisualElement;

            overlay = root.Q<VisualElement>("overlay");

            title = root.Q<Label>("title");
            subtitle = root.Q<Label>("subtitle");

            button1 = root.Q<Button>("upgrade-1");
            button2 = root.Q<Button>("upgrade-2");
            button3 = root.Q<Button>("upgrade-3");

            overlay.style.display = DisplayStyle.None;

            button1.clicked += () => SelectUpgrade(0);
            button2.clicked += () => SelectUpgrade(1);
            button3.clicked += () => SelectUpgrade(2);
        }

        public void Show(List<WeaponUpgrade> upgrades, Action<WeaponUpgrade> callback)
        {
            currentChoices.Clear();
            currentChoices.AddRange(upgrades);

            onUpgradeSelected = callback;

            title.text = "LEVEL UP!";
            subtitle.text = "Choose an Upgrade";

            SetupButton(button1, 0);
            SetupButton(button2, 1);
            SetupButton(button3, 2);

            overlay.style.display = DisplayStyle.Flex;

            Time.timeScale = 0f;
        }

        private void SetupButton(Button button, int index)
        {
            if (index >= currentChoices.Count)
            {
                button.style.display = DisplayStyle.None;
                return;
            }

            button.style.display = DisplayStyle.Flex;

            WeaponUpgrade upgrade = currentChoices[index];

            button.text =
                $"{upgrade.upgradeName}\n{upgrade.description}";
        }

        private void SelectUpgrade(int index)
        {
            if (index < 0 || index >= currentChoices.Count)
                return;

            onUpgradeSelected?.Invoke(currentChoices[index]);

            Hide();
        }

        public void Hide()
        {
            overlay.style.display = DisplayStyle.None;

            currentChoices.Clear();

            Time.timeScale = 1f;
        }
    }
}