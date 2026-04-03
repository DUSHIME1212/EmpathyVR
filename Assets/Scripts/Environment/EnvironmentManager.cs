using UnityEngine;
using System.Collections.Generic;
using EmpathyVR.Data;
using EmpathyVR.Core;

namespace EmpathyVR.Environment
{
    public class EnvironmentManager : MonoBehaviour
    {
        [Header("Sub-Systems")]
        [SerializeField] private WeatherController weatherController;

        [Header("Chapter Environment Roots")]
        [Tooltip("One root GameObject per chapter. Index matches chapter.chapterNumber - 1.")]
        [SerializeField] private List<GameObject> chapterRoots;

        [Header("Interactable Registry")]
        [SerializeField] private List<InteractableObject> allInteractables;

        public static EnvironmentManager Instance { get; private set; }
        private int _activeChapterIndex = -1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError($"<color=red>[EnvironmentManager]</color> CRITICAL: Duplicate EnvironmentManager found on {gameObject.name}. " +
                               "Please delete the duplicate! The current active one is on: " + Instance.gameObject.name);
                return;
            }
            Instance = this;
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.RegisterEnvironmentManager(this);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (GameManager.Instance != null)
                GameManager.Instance.UnregisterEnvironmentManager(this);
        }

        public void LoadChapterEnvironment(SO_NarrativeChapter chapter)
        {
            // Calculate index; Default to 0 if chapterNumber is not set (0)
            int idx = Mathf.Max(0, chapter.chapterNumber - 1);

            // ── 360-Photo / Skybox Support ──────────
            if (chapter.chapterSkybox != null)
            {
                RenderSettings.skybox = chapter.chapterSkybox;
                DynamicGI.UpdateEnvironment(); 
                Debug.Log($"<color=green>[EnvironmentManager]</color> Successfully swapped Skybox to: <b>{chapter.chapterSkybox.name}</b>");
            }
            else
            {
                Debug.LogWarning($"[EnvironmentManager] Chapter '{chapter.chapterId}' has no Skybox Material assigned.");
            }

            // CRITICAL: Check if chapterNumber was left as 0
            if (chapter.chapterNumber == 0)
            {
                Debug.LogError($"<color=red>[EnvironmentManager] DATA ERROR:</color> Chapter '{chapter.chapterId}' has Chapter Number = 0. " +
                               "Environment swapping will stay stuck on Chapter 1. Please set this to 1, 2, 3, etc. in the Inspector.");
            }

            // ── 3D Environments (Roots) ──────────────
            // Activate correct environment root, deactivate others
            if (chapterRoots != null && chapterRoots.Count > 0)
            {
                for (int i = 0; i < chapterRoots.Count; i++)
                {
                    if (chapterRoots[i] != null)
                        chapterRoots[i].SetActive(i == idx);
                }
            }

            // Advance weather with chapter progression
            var weatherState = idx switch
            {
                0 => WeatherController.WeatherState.Dawn,
                1 => WeatherController.WeatherState.MidMorning,
                2 => WeatherController.WeatherState.HarshMidday,
                _ => WeatherController.WeatherState.Overcast
            };
            weatherController?.SetWeatherState(weatherState);

            _activeChapterIndex = idx;
            Debug.Log($"[EnvironmentManager] Loaded environment for '{chapter.chapterId}' at effective index {idx}.");
        }

        /// <summary>
        /// Called by NarrativeSequencer to draw attention to a tagged object.
        /// </summary>
        public void HighlightByTag(string tag)
        {
            foreach (var obj in allInteractables)
            {
                if (obj != null && obj.CompareTag(tag))
                {
                    obj.ForceHighlight();
                    return;
                }
            }
            Debug.LogWarning($"[EnvironmentManager] No interactable found with tag '{tag}'.");
        }

        public void ClearAllHighlights()
        {
            foreach (var obj in allInteractables)
                obj?.ClearHighlight();
        }
    }
}