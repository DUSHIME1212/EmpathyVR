using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using EmpathyVR.Data;

namespace EmpathyVR.UI
{
    /// <summary>
    /// UI component for a single story card in the selection carousel.
    /// </summary>
    public class StoryCard : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image thumbnailImage;
        [SerializeField] private TextMeshProUGUI categoryText;
        [SerializeField] private TextMeshProUGUI durationText;
        [SerializeField] private TextMeshProUGUI comfortText;
        [SerializeField] private Button selectButton;

        private Action _onSelected;

        public void Initialize(SO_Scenario scenario, Action onSelectedCallback)
        {
            _onSelected = onSelectedCallback;

            if (titleText != null) titleText.text = scenario.title;
            if (descriptionText != null) descriptionText.text = scenario.description;
            if (thumbnailImage != null) thumbnailImage.sprite = scenario.thumbnailImage;
            if (categoryText != null) categoryText.text = scenario.category;
            if (durationText != null) durationText.text = $"{scenario.estimatedMinutes} MIN";
            if (comfortText != null) comfortText.text = scenario.comfortLevel;

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(() => _onSelected?.Invoke());
            }
        }
    }
}
