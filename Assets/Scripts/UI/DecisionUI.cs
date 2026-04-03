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
    /// Hidden at scene start — only shown by DecisionManager when a chapter ends.
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

        private void Awake()
        {
            // Always hidden at scene start — only shown by DecisionManager when a chapter ends
            gameObject.SetActive(false);
        }

        public void Show(SO_DecisionData decision, Action<DecisionOptionData> callback)
        {
            _currentDecision = decision;
            _onOptionSelected = callback;

            if (promptText != null) promptText.text = decision.promptText;

            var opt0 = decision.options[0];
            var opt1 = decision.options[1];

            if (optionLeftTitle != null)  optionLeftTitle.text  = opt0.label;
            if (optionLeftDesc != null)   optionLeftDesc.text   = opt0.consequencePreview;

            if (optionRightTitle != null) optionRightTitle.text = opt1.label;
            if (optionRightDesc != null)  optionRightDesc.text  = opt1.consequencePreview;

            // Wire buttons
            if (optionLeftButton != null)
            {
                optionLeftButton.onClick.RemoveAllListeners();
                optionLeftButton.onClick.AddListener(() => _onOptionSelected?.Invoke(opt0));
            }

            if (optionRightButton != null)
            {
                optionRightButton.onClick.RemoveAllListeners();
                optionRightButton.onClick.AddListener(() => _onOptionSelected?.Invoke(opt1));
            }

            if (timerProgressBar != null) timerProgressBar.fillAmount = 0f;

            // ── Activate FIRST, then animate ──
            gameObject.SetActive(true);

            if (panelGroup == null) panelGroup = GetComponent<CanvasGroup>();
            if (panelGroup != null)
            {
                panelGroup.DOKill();
                panelGroup.alpha = 0f;
                panelGroup.interactable = true;
                panelGroup.blocksRaycasts = true;
                panelGroup.DOFade(1f, 0.5f);
            }

            // Add hover visuals if not already present
            AddHoverEffect(optionLeftButton);
            AddHoverEffect(optionRightButton);
        }

        private void AddHoverEffect(Button btn)
        {
            if (btn == null) return;

            var trigger = btn.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (trigger == null) trigger = btn.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

            var enter = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter };
            enter.callback.AddListener((data) => btn.transform.DOScale(1.05f, 0.2f));
            trigger.triggers.Add(enter);

            var exit = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit };
            exit.callback.AddListener((data) => btn.transform.DOScale(1.0f, 0.2f));
            trigger.triggers.Add(exit);
        }

        public void UpdateTimerProgress(float normalized)
        {
            if (_currentDecision == null) return;
            timerProgressBar.fillAmount = normalized;
            int secondsLeft = Mathf.CeilToInt(_currentDecision.timeLimitSeconds * (1f - normalized));
            timerLabel.text = $"CHOOSE WITHIN {secondsLeft} SECONDS OR AMARA DECIDES";
        }

        /// <summary>
        /// Instantly removes the panel with no animation. Use when something must show immediately after.
        /// </summary>
        public void ForceHide()
        {
            if (panelGroup != null)
            {
                panelGroup.DOKill();
                panelGroup.interactable = false;
                panelGroup.blocksRaycasts = false;
            }
            gameObject.SetActive(false);
        }

        public void Hide()
        {
            if (panelGroup == null)
            {
                gameObject.SetActive(false);
                return;
            }

            // Immediately stop interaction so clicks don't bleed through
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
            panelGroup.DOKill();
            panelGroup.DOFade(0f, 0.3f).OnComplete(() => gameObject.SetActive(false));
        }
    }
}