using UnityEngine;
using System;
using EmpathyVR.Data;
using EmpathyVR.UI;
using EmpathyVR.Narrative;
using EmpathyVR.Decisions;
using EmpathyVR.Environment;
using EmpathyVR.Audio;

namespace EmpathyVR.Core
{
    /// <summary>
    /// Central coordinator. Uses singleton pattern. Does NOT contain game logic —
    /// only orchestrates other systems via events and direct references.
    /// Follows Single Responsibility Principle.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Data")]
        [SerializeField] private SO_Scenario currentScenario;
        [SerializeField] private SO_PlayerChoiceRecord playerChoiceRecord;

        [Header("Persistent System References (assign in Inspector)")]
        [SerializeField] private AudioManager audioManager;

        // Scene-local references — populated via Register() calls from each scene's Awake()
        // Do NOT assign these in the Inspector; they register automatically.
        private NarrativeEngine narrativeEngine;
        private DecisionManager decisionManager;
        private EnvironmentManager environmentManager;
        private HUDController hudController;

        // ── Events (other systems subscribe, not poll) ──────────────
        public static event Action<SO_Scenario> OnScenarioLoaded;
        public static event Action<int> OnChapterChanged;       // chapter index
        public static event Action OnExperienceComplete;

        public SO_Scenario CurrentScenario => currentScenario;
        public SO_PlayerChoiceRecord PlayerChoiceRecord => playerChoiceRecord;

        private int _currentChapterIndex = 0;

        // ── Lifecycle ───────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Validate only the persistent (DDOL) references that must be set in Inspector
            if (audioManager == null)
                Debug.LogError("[GameManager] 'AudioManager' is not assigned in Inspector!", this);
            if (playerChoiceRecord == null)
                Debug.LogError("[GameManager] 'PlayerChoiceRecord' is not assigned in Inspector!", this);

            if (playerChoiceRecord != null)
                playerChoiceRecord.Reset();

            // currentScenario is only loaded by the gameplay scene — not at boot
            // The GameScene will call LoadScenario() directly.
        }

        // ── Register / Unregister (called by scene-local MonoBehaviours) ────────
        public void RegisterNarrativeEngine(NarrativeEngine engine)     { narrativeEngine = engine;       Debug.Log("[GameManager] NarrativeEngine registered."); }
        public void UnregisterNarrativeEngine(NarrativeEngine engine)   { if (narrativeEngine == engine) narrativeEngine = null; }

        public void RegisterDecisionManager(DecisionManager manager)    { decisionManager = manager;      Debug.Log("[GameManager] DecisionManager registered."); }
        public void UnregisterDecisionManager(DecisionManager manager)  { if (decisionManager == manager) decisionManager = null; }

        public void RegisterEnvironmentManager(EnvironmentManager mgr)  { environmentManager = mgr;       Debug.Log("[GameManager] EnvironmentManager registered."); }
        public void UnregisterEnvironmentManager(EnvironmentManager m)  { if (environmentManager == m) environmentManager = null; }

        public void RegisterHUDController(HUDController hud)           { hudController = hud;            Debug.Log("[GameManager] HUDController registered."); }
        public void UnregisterHUDController(HUDController hud)         { if (hudController == hud) hudController = null; }

        // ── Public API ──────────────────────────────────────────────
        public void LoadScenario(SO_Scenario scenario)
        {
            currentScenario = scenario;
            _currentChapterIndex = 0;
            OnScenarioLoaded?.Invoke(scenario);
            BeginChapter(0);
        }

        public void BeginChapter(int index)
        {
            if (currentScenario == null)
            {
                Debug.LogError("[GameManager] Cannot BeginChapter: currentScenario is null.");
                return;
            }

            if (index < 0 || index >= currentScenario.chapters.Count)
            {
                Debug.LogError($"[GameManager] Cannot BeginChapter: Index {index} out of range (Total Chapters: {currentScenario.chapters.Count}).");
                return;
            }

            _currentChapterIndex = index;
            var chapter = currentScenario.chapters[index];

            // ── Invoke Chapter Loading on Subsystems (with null safety) ──────────────
            if (environmentManager != null) environmentManager.LoadChapterEnvironment(chapter);
            if (audioManager != null) audioManager.PlayAmbient(chapter.ambientAudioTag);
            if (narrativeEngine != null) narrativeEngine.PlayChapter(chapter);
            if (hudController != null) hudController.UpdateChapterLabel(chapter.chapterNumber, currentScenario.chapters.Count);

            OnChapterChanged?.Invoke(index);
        }

        public void OnChapterNarrationComplete(SO_NarrativeChapter chapter)
        {
            if (chapter.triggerDecisionAfter && chapter.nextDecision != null)
            {
                decisionManager.PresentDecision(chapter.nextDecision);
            }
            else
            {
                AdvanceToNextChapter();
            }
        }

        public void AdvanceToNextChapter()
        {
            int next = _currentChapterIndex + 1;
            if (next < currentScenario.chapters.Count)
                BeginChapter(next);
            else
                CompleteExperience();
        }

        public void CompleteExperience()
        {
            OnExperienceComplete?.Invoke();
            SceneLoader.Instance.LoadScene("06_Reflection");
        }
    }
}