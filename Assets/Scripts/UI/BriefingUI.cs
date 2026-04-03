// ─────────────────────────────────────────────────────────────────────────────
// FILE: BriefingUI.cs  (FULL)
// Place in: Assets/_Project/Scripts/UI/
// Purpose: Drives the Briefing screen (Image 1 in mockups).
//          Three tabs: Briefing | Experience | Reflection.
//          Reads from the currently selected SO_Scenario.
// ─────────────────────────────────────────────────────────────────────────────
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using EmpathyVR.Core;
using EmpathyVR.Data;
 
[RequireComponent(typeof(CanvasGroup))]
public class BriefingUI : MonoBehaviour
{
    // ── Header Card ─────────────────────────────────────────────────────────
    [Header("Header Card")]
    [SerializeField] private TextMeshProUGUI categoryTagText;    // "NATURE RESTORATION"
    [SerializeField] private TextMeshProUGUI scenarioTitleText;  // "The Farmer's Morning"
    [SerializeField] private TextMeshProUGUI descriptionText;
 
    // ── What You'll Do List ─────────────────────────────────────────────────
    [Header("Activity Bullets")]
    [SerializeField] private Transform bulletContainer;
    [SerializeField] private GameObject bulletItemPrefab;        // Prefab: dot + TMP label
 
    // ── Meta Footer ─────────────────────────────────────────────────────────
    [Header("Meta Footer")]
    [SerializeField] private TextMeshProUGUI durationAndComfortText;
 
    // ── Tabs ────────────────────────────────────────────────────────────────
    [Header("Tabs")]
    [SerializeField] private Button tabBriefing;
    [SerializeField] private Button tabExperience;
    [SerializeField] private Button tabReflection;
    [SerializeField] private GameObject panelBriefing;
    [SerializeField] private GameObject panelExperience;
    [SerializeField] private GameObject panelReflection;
 
    // ── CTA ─────────────────────────────────────────────────────────────────
    [Header("CTA")]
    [SerializeField] private Button beginExperienceButton;
 
    // ── Left Nav (Briefing / Experience / Reflection sidebar) ────────────────
    [Header("Left Navigation")]
    [SerializeField] private Button navBriefing;
    [SerializeField] private Button navExperience;
    [SerializeField] private Button navReflection;
 
    // ── Progress Bar ─────────────────────────────────────────────────────────
    [Header("Bottom Progress Steps")]
    [SerializeField] private List<GameObject> progressDots;   // 3 dots at bottom
 
    // ── Chapter Tracker ──────────────────────────────────────────────────────
    [Header("Chapter Tracker")]
    [SerializeField] private TextMeshProUGUI chapterTrackerText;  // "Chapter 1 of 2"
 
    private CanvasGroup _canvasGroup;
    private int _activeTab = 0;
 
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
 
    private void Start()
    {
        var scenario = GameManager.Instance?.CurrentScenario;
        if (scenario != null)
            Populate(scenario);
 
        // Wire tab buttons
        tabBriefing?.onClick.AddListener(()   => SwitchTab(0));
        tabExperience?.onClick.AddListener(() => SwitchTab(1));
        tabReflection?.onClick.AddListener(() => SwitchTab(2));
 
        navBriefing?.onClick.AddListener(()   => SwitchTab(0));
        navExperience?.onClick.AddListener(() => SwitchTab(1));
        navReflection?.onClick.AddListener(() => SwitchTab(2));
 
        // CTA
        if (beginExperienceButton != null)
        {
            beginExperienceButton.onClick.AddListener(OnBeginClicked);
            Debug.Log("[BriefingUI] Start: beginExperienceButton listener added.");
        }
        else
        {
            Debug.LogError("[BriefingUI] Start: beginExperienceButton is NOT ASSIGNED in the inspector!", this);
        }
 
        SwitchTab(0); // Default to Briefing tab
        FadeIn();
    }
 
    private void Populate(SO_Scenario scenario)
    {
        if (categoryTagText != null) categoryTagText.text = scenario.category;
        if (scenarioTitleText != null) scenarioTitleText.text = scenario.title;
        if (descriptionText != null) descriptionText.text = scenario.description;
        
        if (durationAndComfortText != null)
        {
            durationAndComfortText.text = $"Estimated duration: {scenario.estimatedMinutes} minutes. " +
                                          $"Comfort level: {scenario.comfortLevel}.";
        }
 
        if (chapterTrackerText != null && scenario.chapters != null)
            chapterTrackerText.text = $"Chapter 1 of {scenario.chapters.Count}";
 
        // Populate bullet points from chapter titles
        if (bulletContainer != null && bulletItemPrefab != null && scenario.chapters != null)
        {
            foreach (Transform child in bulletContainer)
                Destroy(child.gameObject);
 
            foreach (var chapter in scenario.chapters)
            {
                var item = Instantiate(bulletItemPrefab, bulletContainer);
                var label = item.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null) label.text = chapter.chapterTitle;
            }
        }
    }
 
    private void SwitchTab(int index)
    {
        _activeTab = index;
        
        if (panelBriefing != null) panelBriefing.SetActive(index == 0);
        if (panelExperience != null) panelExperience.SetActive(index == 1);
        if (panelReflection != null) panelReflection.SetActive(index == 2);
 
        // Update progress dots
        for (int i = 0; i < progressDots.Count; i++)
        {
            var img = progressDots[i]?.GetComponent<Image>();
            if (img != null)
                img.color = (i == index)
                    ? new Color(0.29f, 0.87f, 0.6f, 1f)   // accent green
                    : new Color(0.4f,  0.4f,  0.4f, 0.5f); // dim
        }
    }
 
    private void OnBeginClicked()
    {
        Debug.Log("[BriefingUI] OnBeginClicked: Button pressed!");
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("03_StorySelection");
        }
        else
        {
            Debug.LogWarning("[BriefingUI] SceneLoader.Instance is null! Falling back to SceneManager.LoadScene.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("03_StorySelection");
        }
    }
 
    private void FadeIn()
    {
        if (_canvasGroup == null) return;
        _canvasGroup.alpha = 0f;
        _canvasGroup.DOFade(1f, 0.6f);
    }
}