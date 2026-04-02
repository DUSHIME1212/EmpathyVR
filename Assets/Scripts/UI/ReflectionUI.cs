 
// ─────────────────────────────────────────────────────────────────────────────
// FILE: ReflectionUI.cs  (FULL)
// Place in: Assets/_Project/Scripts/UI/
// Purpose: Final scene (06_Reflection). Shows a summary of all 3 decisions
//          the player made, the outcomes, a reflective prompt, and
//          options to replay or share.
// ─────────────────────────────────────────────────────────────────────────────
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using EmpathyVR.Core;
using EmpathyVR.Data;
 
public class ReflectionUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SO_PlayerChoiceRecord playerChoiceRecord;
 
    [Header("Header")]
    [SerializeField] private TextMeshProUGUI headlineText;       // "Your Choices"
    [SerializeField] private TextMeshProUGUI reflectionPrompt;   // Dynamic question
 
    [Header("Decision Summary List")]
    [SerializeField] private Transform summaryContainer;
    [SerializeField] private GameObject summaryItemPrefab;       // Prefab with 2x TMP labels
 
    [Header("Stats Strip")]
    [SerializeField] private TextMeshProUGUI totalTimeText;
    [SerializeField] private TextMeshProUGUI decisionsCountText;
    [SerializeField] private TextMeshProUGUI timeoutCountText;
 
    [Header("Buttons")]
    [SerializeField] private Button replayButton;
    [SerializeField] private Button shareButton;
 
    [Header("Animation")]
    [SerializeField] private CanvasGroup rootCanvasGroup;
    [SerializeField] private float itemStaggerDelay = 0.25f;
 
    private void Start()
    {
        rootCanvasGroup.alpha = 0f;
 
        var summary = ConsequenceResolver.Resolve(playerChoiceRecord);
 
        headlineText.text         = "Your Choices in Amara's World";
        reflectionPrompt.text     = ConsequenceResolver.GetReflectionPrompt(summary);
        decisionsCountText.text   = $"{summary.decidedByPlayer} of {summary.totalDecisions} decided by you";
        timeoutCountText.text     = summary.decidedByTimeout > 0
                                    ? $"Amara decided {summary.decidedByTimeout}×"
                                    : "You made every call.";
 
        if (playerChoiceRecord != null)
        {
            float mins = Mathf.FloorToInt(playerChoiceRecord.totalExperienceTime / 60f);
            float secs = playerChoiceRecord.totalExperienceTime % 60f;
            totalTimeText.text = $"{mins:0}m {secs:00}s in Amara's shoes";
        }
 
        replayButton?.onClick.AddListener(() => SceneLoader.Instance?.LoadScene("01_MainMenu"));
        shareButton?.onClick.AddListener(OnShareClicked);
 
        StartCoroutine(AnimateIn(summary));
    }
 
    private IEnumerator AnimateIn(ConsequenceResolver.ConsequenceSummary summary)
    {
        yield return new WaitForSeconds(0.3f);
        rootCanvasGroup.DOFade(1f, 0.8f);
 
        yield return new WaitForSeconds(0.6f);
 
        // Spawn summary cards with stagger
        foreach (var choice in playerChoiceRecord.choices)
        {
            var item = Instantiate(summaryItemPrefab, summaryContainer);
            var labels = item.GetComponentsInChildren<TextMeshProUGUI>();
 
            if (labels.Length >= 1) labels[0].text = choice.chosenLabel;
            if (labels.Length >= 2) labels[1].text = choice.outcomeHeadline;
 
            // Animate each item sliding in
            var cg = item.GetComponent<CanvasGroup>();
            if (cg == null) cg = item.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            item.transform.localScale = new Vector3(0.95f, 0.95f, 1f);
 
            cg.DOFade(1f, 0.35f);
            item.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
 
            yield return new WaitForSeconds(itemStaggerDelay);
        }
    }
 
    private void OnShareClicked()
    {
        // On mobile/WebGL: open native share sheet
        // On Quest: copy to clipboard (no native share API in standalone)
        string shareText = $"I just experienced Amara's life in EmpathyVR. " +
                           $"My choices: {BuildChoiceSummaryText()}";
 
        GUIUtility.systemCopyBuffer = shareText;
        Debug.Log($"[ReflectionUI] Copied to clipboard: {shareText}");
 
        // Optionally show a "Copied!" toast notification here
    }
 
    private string BuildChoiceSummaryText()
    {
        var sb = new System.Text.StringBuilder();
        foreach (var c in playerChoiceRecord.choices)
            sb.AppendLine($"• {c.chosenLabel} → {c.outcomeHeadline}");
        return sb.ToString();
    }
}