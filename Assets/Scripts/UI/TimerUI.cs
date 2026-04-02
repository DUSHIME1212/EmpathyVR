
// ─────────────────────────────────────────────────────────────────────────────
// FILE: TimerUI.cs
// Place in: Assets/_Project/Scripts/UI/
// Purpose: Standalone timer component used inside the Decision Panel.
//          Drives both the circular arc and the text countdown label.
//          DecisionManager calls SetProgress() every frame during countdown.
// ─────────────────────────────────────────────────────────────────────────────
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
 
public class TimerUI : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Image arcFill;           // Image type = Filled, Radial 360
    [SerializeField] private TextMeshProUGUI countdownLabel;
    [SerializeField] private TextMeshProUGUI hintLabel; // "Choose within X seconds or Amara decides"
 
    [Header("Color Progression")]
    [SerializeField] private Color startColor  = new Color(0.29f, 0.87f, 0.6f);  // green
    [SerializeField] private Color urgentColor = new Color(0.95f, 0.4f, 0.3f);   // red
 
    [Header("Pulse on Urgency")]
    [SerializeField] private float urgencyThreshold = 0.7f;  // normalized — triggers pulse at 70% elapsed
    private bool _pulsing = false;
 
    private float _totalDuration;
 
    public void Initialize(float totalSeconds)
    {
        _totalDuration = totalSeconds;
        _pulsing = false;
 
        if (arcFill != null)
        {
            arcFill.fillAmount = 0f;
            arcFill.color = startColor;
        }
        if (countdownLabel != null)
            countdownLabel.text = Mathf.CeilToInt(totalSeconds).ToString();
        if (hintLabel != null)
            hintLabel.text = $"CHOOSE WITHIN {Mathf.CeilToInt(totalSeconds)} SECONDS OR AMARA DECIDES";
    }
 
    /// <summary>
    /// Call every frame. normalized = elapsed / total (0..1).
    /// </summary>
    public void SetProgress(float normalized)
    {
        normalized = Mathf.Clamp01(normalized);
        float remaining = _totalDuration * (1f - normalized);
 
        if (arcFill != null)
        {
            arcFill.fillAmount = normalized;
            arcFill.color = Color.Lerp(startColor, urgentColor, normalized);
        }
 
        if (countdownLabel != null)
            countdownLabel.text = Mathf.CeilToInt(remaining).ToString();
 
        if (hintLabel != null)
            hintLabel.text = $"CHOOSE WITHIN {Mathf.CeilToInt(remaining)} SECONDS OR AMARA DECIDES";
 
        // Start pulsing arc when time is running out
        if (normalized >= urgencyThreshold && !_pulsing)
        {
            _pulsing = true;
            if (arcFill != null)
                arcFill.transform.DOPunchScale(Vector3.one * 0.06f, 0.4f, 5, 0.5f)
                    .SetLoops(-1, LoopType.Restart);
        }
 
        if (normalized >= 1f) StopPulse();
    }
 
    public void StopPulse()
    {
        _pulsing = false;
        if (arcFill != null)
        {
            DOTween.Kill(arcFill.transform);
            arcFill.transform.localScale = Vector3.one;
        }
    }
 
    public void Hide()
    {
        StopPulse();
        gameObject.SetActive(false);
    }
}
 