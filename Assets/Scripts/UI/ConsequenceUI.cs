using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using EmpathyVR.Data;

namespace EmpathyVR.UI
{
    /// <summary>
    /// Shows the outcome of the player's decision with emotional impact text
    /// and real-world statistics.
    /// </summary>
    public class ConsequenceUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private CanvasGroup panelGroup;

        [Header("Content")]
        [SerializeField] private TextMeshProUGUI youChoseLabel;   // "YOU CHOSE:"
        [SerializeField] private TextMeshProUGUI choiceQuoteText; // The option they picked
        [SerializeField] private TextMeshProUGUI outcomeHeadline; // Big bold consequence
        [SerializeField] private TextMeshProUGUI realImpactStat;  // Stat text
        [SerializeField] private TextMeshProUGUI nextChapterText; // "Next: Chapter II — The Resilience"

        [Header("Buttons")]
        [SerializeField] private UnityEngine.UI.Button continueButton;

        private Action _onAcknowledged;

        public void Show(DecisionOptionData choice, string realImpact, Action onAcknowledged)
        {
            _onAcknowledged = onAcknowledged;

            choiceQuoteText.text = choice.label;
            outcomeHeadline.text = choice.outcomeHeadline;
            realImpactStat.text = realImpact;

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() => _onAcknowledged?.Invoke());

            panelGroup.alpha = 0;
            gameObject.SetActive(true);

            // Staggered reveal for emotional impact
            panelGroup.DOFade(1f, 0.6f);
            outcomeHeadline.transform.localScale = Vector3.one * 0.8f;
            outcomeHeadline.transform.DOScale(1f, 0.5f).SetDelay(0.3f).SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            panelGroup.DOFade(0f, 0.3f).OnComplete(() => gameObject.SetActive(false));
        }
    }
}