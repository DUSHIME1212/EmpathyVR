using UnityEngine;
using System.Collections;
using EmpathyVR.Data;
using EmpathyVR.UI;
using EmpathyVR.Core;

namespace EmpathyVR.Decisions
{
    /// <summary>
    /// Shows decision UI, runs the countdown timer, records the player's choice,
    /// then triggers the consequence display.
    /// </summary>
    public class DecisionManager : MonoBehaviour
    {
        [SerializeField] private DecisionUI decisionUI;
        [SerializeField] private ConsequenceUI consequenceUI;
        [SerializeField] private SO_PlayerChoiceRecord playerChoiceRecord;

        private SO_DecisionData _currentDecision;
        private Coroutine _timerCoroutine;
        public static DecisionManager Instance { get; private set; }
        private float _decisionStartTime;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError($"<color=red>[DecisionManager]</color> CRITICAL: Duplicate DecisionManager found on {gameObject.name}. " +
                               "Please delete the duplicate! The current active one is on: " + Instance.gameObject.name);
                return;
            }
            Instance = this;

            // ── Inspector Reference Validation ──
            if (decisionUI == null)
                Debug.LogError("[DecisionManager] 'decisionUI' is NOT assigned in the Inspector! Drag the Decision_Panel here.", this);
            if (consequenceUI == null)
                Debug.LogError("[DecisionManager] 'consequenceUI' is NOT assigned in the Inspector! Drag the Consequence_Panel here.", this);
            if (playerChoiceRecord == null)
                Debug.LogError("[DecisionManager] 'playerChoiceRecord' is NOT assigned in the Inspector! Drag the SO_PlayerChoiceRecord asset here.", this);
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.RegisterDecisionManager(this);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (GameManager.Instance != null)
                GameManager.Instance.UnregisterDecisionManager(this);
        }

        public void PresentDecision(SO_DecisionData decision)
        {
            _currentDecision = decision;
            _decisionStartTime = Time.time;
            decisionUI.Show(decision, OnOptionSelected);

            if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
            _timerCoroutine = StartCoroutine(DecisionTimerRoutine(decision.timeLimitSeconds));
        }

        // Called by DecisionUI buttons
        public void OnOptionSelected(DecisionOptionData chosenOption)
        {
            Debug.Log($"[DecisionManager] Option selected: '{chosenOption.label}'");

            if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);

            float timeToDecide = Time.time - _decisionStartTime;

            if (playerChoiceRecord != null)
            {
                playerChoiceRecord.RecordChoice(
                    _currentDecision.decisionId,
                    chosenOption.optionId,
                    chosenOption.label,
                    chosenOption.outcomeHeadline,
                    timeToDecide,
                    false
                );
            }

            // ForceHide immediately (no fade delay) so ConsequencePanel is never blocked
            decisionUI.ForceHide();

            if (consequenceUI == null)
            {
                Debug.LogError("[DecisionManager] consequenceUI is null! Skipping to next chapter.");
                GameManager.Instance.AdvanceToNextChapter();
                return;
            }

            Debug.Log("[DecisionManager] Showing consequence panel...");
            consequenceUI.Show(chosenOption, _currentDecision.realWorldImpactText, OnConsequenceAcknowledged);
        }

        private void OnConsequenceAcknowledged()
        {
            Debug.Log("[DecisionManager] Consequence acknowledged. Advancing chapter...");
            consequenceUI.Hide();
            GameManager.Instance.AdvanceToNextChapter();
        }

        private IEnumerator DecisionTimerRoutine(float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                decisionUI.UpdateTimerProgress(elapsed / duration);
                yield return null;
            }

            // Time ran out — Amara decides (pick first option as default)
            var defaultOption = _currentDecision.options[0];
            float timeToDecide = duration;
            playerChoiceRecord.RecordChoice(
                _currentDecision.decisionId,
                defaultOption.optionId,
                defaultOption.label,
                defaultOption.outcomeHeadline,
                timeToDecide,
                true
            );

            decisionUI.Hide();
            consequenceUI.Show(defaultOption, _currentDecision.realWorldImpactText, OnConsequenceAcknowledged);
        }
    }
}