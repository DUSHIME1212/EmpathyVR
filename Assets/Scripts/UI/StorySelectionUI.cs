using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using EmpathyVR.Data;
using EmpathyVR.Core;

namespace EmpathyVR.UI
{
    /// <summary>
    /// Populates the story selection carousel (Image 2 in design).
    /// Instantiates a StoryCard per scenario.
    /// </summary>
    public class StorySelectionUI : MonoBehaviour
    {
        [SerializeField] private SO_ScenarioLibrary scenarioLibrary;
        [SerializeField] private GameObject storyCardPrefab;
        [SerializeField] private Transform cardContainer;
        [SerializeField] private CanvasGroup panelGroup;

        private void Start()
        {
            PopulateCards();
        }

        private void PopulateCards()
        {
            if (scenarioLibrary == null || scenarioLibrary.scenarios == null) return;

            int scenarioCount = scenarioLibrary.scenarios.Count;
            int childCount = cardContainer.childCount;

            for (int i = 0; i < scenarioCount; i++)
            {
                GameObject cardObj;
                if (i < childCount)
                {
                    // Reuse existing child (e.g., the template in the scene)
                    cardObj = cardContainer.GetChild(i).gameObject;
                }
                else
                {
                    // Create new one if we have more scenarios than existing cards
                    cardObj = Instantiate(storyCardPrefab, cardContainer);
                }

                cardObj.SetActive(true);
                var storyCard = cardObj.GetComponent<StoryCard>();
                if (storyCard != null)
                {
                    var scenario = scenarioLibrary.scenarios[i];
                    storyCard.Initialize(scenario, () => OnScenarioSelected(scenario));
                }
            }

            // Hide any extra placeholder cards that might be in the container
            for (int i = scenarioCount; i < cardContainer.childCount; i++)
            {
                cardContainer.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void OnScenarioSelected(SO_Scenario scenario)
        {
            Debug.Log($"[StorySelectionUI] OnScenarioSelected: {scenario.title}");

            if (GameManager.Instance != null)
                GameManager.Instance.LoadScenario(scenario);
            else
                Debug.LogWarning("[StorySelectionUI] GameManager.Instance is null! Scenario data not loaded to GameManager.");

            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadScene(scenario.sceneName);
            }
            else
            {
                Debug.LogWarning($"[StorySelectionUI] SceneLoader.Instance is null! Falling back to SceneManager.LoadScene with scene: {scenario.sceneName}");
                UnityEngine.SceneManagement.SceneManager.LoadScene(scenario.sceneName);
            }
        }
    }
}