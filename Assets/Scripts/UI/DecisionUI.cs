using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using EmpathyVR.Data;

namespace EmpathyVR.UI
{
    /// <summary>
    /// Populates and animates the decision panel.
    /// Communicates back to DecisionManager only via the callback delegate.
    /// </summary>
    public class DecisionUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private CanvasGroup panelGroup;
        [SerializeField] private TextMeshProUGUI promptText;

        [Header("Option Left")]
        [SerializeField] private TextMeshProUGUI optionLeftTitle;
        [SerializeField] private TextMeshProUGUI optionLeftDesc;
        [SerializeField] private Button optionLeftButton;

        [Header("Option Right")]
        [SerializeField] private TextMeshProUGUI optionRightTitle;
        [SerializeField] private TextMeshProUGUI optionRightDesc;
        [SerializeField] private Button optionRightButton;

        [Header("Timer Bar")]
        [SerializeField] private Image timerProgressBar;
        [SerializeField] private TextMeshProUGUI timerLabel;

        private SO_DecisionData _currentDecision;
        private Action<DecisionOptionData> _onOptionSelected;

        public void Show(SO_DecisionData decision, Action<DecisionOptionData> callback)
        {
            _currentDecision = decision;
            _onOptionSelected = callback;

            promptText.text = decision.promptText;

            var opt0 = decision.options[0];
            var opt1 = decision.options[1];

            optionLeftTitle.text = opt0.label;
            optionLeftDesc.text = opt0.consequencePreview;

            optionRightTitle.text = opt1.label;
            optionRightDesc.text = opt1.consequencePreview;

            // Wire buttons
            optionLeftButton.onClick.RemoveAllListeners();
            optionLeftButton.onClick.AddListener(() => _onOptionSelected?.Invoke(opt0));

            optionRightButton.onClick.RemoveAllListeners();
            optionRightButton.onClick.AddListener(() => _onOptionSelected?.Invoke(opt1));

            timerProgressBar.fillAmount = 0f;

            panelGroup.alpha = 0;
            gameObject.SetActive(true);
            panelGroup.DOFade(1f, 0.5f);
        }

        public void UpdateTimerProgress(float normalized)
        {
            if (_currentDecision == null) return;
            timerProgressBar.fillAmount = normalized;
            int secondsLeft = Mathf.CeilToInt(_currentDecision.timeLimitSeconds * (1f - normalized));
            timerLabel.text = $"CHOOSE WITHIN {secondsLeft} SECONDS OR AMARA DECIDES";
        }

        public void Hide()
        {
            panelGroup.DOFade(0f, 0.4f).OnComplete(() => gameObject.SetActive(false));
        }
    }
}