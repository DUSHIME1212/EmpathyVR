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
        private float _decisionStartTime;

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.RegisterDecisionManager(this);
        }

        private void OnDestroy()
        {
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
            if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);

            float timeToDecide = Time.time - _decisionStartTime;
            bool wasTimeout = false;

            playerChoiceRecord.RecordChoice(
                _currentDecision.decisionId,
                chosenOption.optionId,
                chosenOption.label,
                chosenOption.outcomeHeadline,
                timeToDecide,
                wasTimeout
            );

            decisionUI.Hide();
            consequenceUI.Show(chosenOption, _currentDecision.realWorldImpactText, OnConsequenceAcknowledged);
        }

        private void OnConsequenceAcknowledged()
        {
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