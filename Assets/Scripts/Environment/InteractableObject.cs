
// ─────────────────────────────────────────────────────────────────────────────
// FILE: InteractableObject.cs  (FULL — replaces stub, adds ForceHighlight)
// Place in: Assets/_Project/Scripts/Environment/
// ─────────────────────────────────────────────────────────────────────────────
using UnityEngine;
using DG.Tweening;
using EmpathyVR.UI;
using EmpathyVR.Audio;
 
public class InteractableObject : MonoBehaviour, IInteractable
{
    [Header("Narration")]
    [SerializeField] private string speakerName = "KOFI (THOUGHT)";
    [TextArea(3, 6)]
    [SerializeField] private string narrationText;
    [SerializeField] private AudioClip narrationClip;
 
    [Header("Visual Feedback")]
    [SerializeField] private GameObject highlightEffect;   // Child GO with glow/outline
    [SerializeField] private Renderer objectRenderer;
 
    [Header("Behaviour")]
    [SerializeField] private bool canTriggerMultipleTimes = false;
 
    private bool _hasTriggered = false;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
 
    public void OnGazeEnter()
    {
        if (highlightEffect != null) highlightEffect.SetActive(true);
 
        if (objectRenderer != null)
        {
            objectRenderer.material.EnableKeyword("_EMISSION");
            objectRenderer.material.DOColor(
                new Color(0.2f, 0.8f, 0.4f) * 1.5f, EmissionColor, 0.25f);
        }
    }
 
    public void OnGazeExit()
    {
        if (highlightEffect != null) highlightEffect.SetActive(false);
 
        if (objectRenderer != null)
        {
            objectRenderer.material.DOColor(Color.black, EmissionColor, 0.25f);
        }
    }
 
    public void OnDwellComplete()
    {
        if (_hasTriggered && !canTriggerMultipleTimes) return;
        _hasTriggered = true;
        TriggerNarration();
    }
 
    /// <summary>Called by EnvironmentManager to draw attention programmatically.</summary>
    public void ForceHighlight()
    {
        OnGazeEnter();
        DOVirtual.DelayedCall(3f, ClearHighlight);
    }
 
    public void ClearHighlight()
    {
        OnGazeExit();
    }
 
    private void TriggerNarration()
    {
        HUDController.Instance?.ShowSubtitle(speakerName, narrationText);
        if (narrationClip != null)
            AudioManager.Instance?.PlayOneShot(narrationClip);
    }
}