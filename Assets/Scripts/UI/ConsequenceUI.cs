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

        private void Awake()
        {
            // Always hidden at scene start — only shown by DecisionManager after a choice is made
            gameObject.SetActive(false);
        }

        public void Show(DecisionOptionData choice, string realImpact, Action onAcknowledged, string nextChapter = "")
        {
            _onAcknowledged = onAcknowledged;

            if (youChoseLabel != null)   youChoseLabel.text   = "YOU CHOSE:";
            if (choiceQuoteText != null)  choiceQuoteText.text  = choice.label;
            if (outcomeHeadline != null)  outcomeHeadline.text  = choice.outcomeHeadline;
            if (realImpactStat != null)   realImpactStat.text   = realImpact;

            // Show next chapter label if provided
            if (nextChapterText != null)
                nextChapterText.text = string.IsNullOrEmpty(nextChapter)
                    ? ""
                    : $"Next: {nextChapter}";

            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() => _onAcknowledged?.Invoke());

            // ── Activate FIRST, then animate ──
            // SetActive must happen before DOFade or DOTween has nothing to animate
            gameObject.SetActive(true);

            if (panelGroup == null) panelGroup = GetComponent<CanvasGroup>();
            if (panelGroup != null)
            {
                panelGroup.DOKill();          // cancel any in-flight tweens
                panelGroup.alpha = 0f;
                panelGroup.interactable = true;
                panelGroup.blocksRaycasts = true;
                panelGroup.DOFade(1f, 0.6f);
            }

            // Staggered reveal for emotional impact
            if (outcomeHeadline != null)
            {
                outcomeHeadline.transform.DOKill();
                outcomeHeadline.transform.localScale = Vector3.one * 0.8f;
                outcomeHeadline.transform.DOScale(1f, 0.5f).SetDelay(0.3f).SetEase(Ease.OutBack);
            }

            AddHoverEffect(continueButton);
        }

        private void AddHoverEffect(UnityEngine.UI.Button btn)
        {
            if (btn == null) return;
            var trigger = btn.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (trigger == null) trigger = btn.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            trigger.triggers.Clear();

            var enter = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter };
            enter.callback.AddListener((data) => btn.transform.DOScale(1.05f, 0.2f));
            trigger.triggers.Add(enter);

            var exit = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit };
            exit.callback.AddListener((data) => btn.transform.DOScale(1.0f, 0.2f));
            trigger.triggers.Add(exit);
        }

        public void Hide()
        {
            if (panelGroup == null)
            {
                gameObject.SetActive(false);
                return;
            }

            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
            panelGroup.DOKill();
            panelGroup.DOFade(0f, 0.3f).OnComplete(() => gameObject.SetActive(false));
        }
    }
}