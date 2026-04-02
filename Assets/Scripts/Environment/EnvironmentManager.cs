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

        private int _activeChapterIndex = -1;

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.RegisterEnvironmentManager(this);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.UnregisterEnvironmentManager(this);
        }

        public void LoadChapterEnvironment(SO_NarrativeChapter chapter)
        {
            // chapterNumber is 1-based; guard against 0 (unset SO field)
            if (chapter.chapterNumber <= 0)
            {
                Debug.LogWarning($"[EnvironmentManager] Chapter '{chapter.chapterId}' has chapterNumber={chapter.chapterNumber}. " +
                                 "It should be 1-based. Defaulting to index 0. Please set chapterNumber in the Inspector.");
            }

            int idx = Mathf.Max(0, chapter.chapterNumber - 1);

            // Activate correct environment root, deactivate others
            for (int i = 0; i < chapterRoots.Count; i++)
            {
                if (chapterRoots[i] != null)
                    chapterRoots[i].SetActive(i == idx);
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
            Debug.Log($"[EnvironmentManager] Loaded chapter environment index {idx}: {chapter.chapterId}");
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