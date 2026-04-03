using UnityEngine;
using EmpathyVR.Core;
using EmpathyVR.Narrative;
using EmpathyVR.Decisions;
using EmpathyVR.Environment;
using EmpathyVR.UI;
using EmpathyVR.Data;

namespace EmpathyVR.Core
{
    /// <summary>
    /// Explicitly wires scene-local managers to the persistent GameManager.
    /// This resolves race conditions and ensures references are populated 
    /// before any chapter logic begins.
    /// </summary>
    public class SceneInitializer : MonoBehaviour
    {
        [Header("Testing")]
        [Tooltip("If assigned, this scenario will load automatically for editor testing.")]
        [SerializeField] private SO_Scenario testScenario;
        [SerializeField] private bool autoStartOnLoad = true;

        [Header("Scene-Local Managers")]
        [SerializeField] private NarrativeEngine narrativeEngine;
        [SerializeField] private DecisionManager decisionManager;
        [SerializeField] private EnvironmentManager environmentManager;
        [SerializeField] private HUDController hudController;

        private void Awake()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("[SceneInitializer] GameManager.Instance is null. Is the Bootstrapper missing?");
                return;
            }

            // Explicitly push references to GameManager
            if (narrativeEngine != null) GameManager.Instance.RegisterNarrativeEngine(narrativeEngine);
            if (decisionManager != null) GameManager.Instance.RegisterDecisionManager(decisionManager);
            if (environmentManager != null) GameManager.Instance.RegisterEnvironmentManager(environmentManager);
            if (hudController != null) GameManager.Instance.RegisterHUDController(hudController);

            Debug.Log("<color=green>[SceneInitializer]</color> Scene-local references wired to GameManager successfully.");

            // ── Auto-Start Logic ─────────────────────────────────────────
            if (testScenario != null && GameManager.Instance.CurrentScenario == null)
            {
                Debug.Log($"[SceneInitializer] Direct scene load detected. Loading test scenario: {testScenario.title}");
                GameManager.Instance.LoadScenario(testScenario);
            }

            if (autoStartOnLoad && GameManager.Instance.CurrentScenario != null)
            {
                // Delay slightly to ensure all OnEnable/Start calls are finished
                Invoke(nameof(TriggerStart), 0.1f);
            }
        }

        private void TriggerStart()
        {
            GameManager.Instance.StartExperience();
        }
    }
}
