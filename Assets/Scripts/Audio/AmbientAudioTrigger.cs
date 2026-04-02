// ─────────────────────────────────────────────────────────────────────────────
// FILE: AmbientAudioTrigger.cs  (FULL — replaces stub)
// Place in: Assets/_Project/Scripts/Audio/
// Purpose: Collider-based zone that swaps the ambient soundscape when the
//          player walks into it. Multiple zones can coexist in a scene.
// ─────────────────────────────────────────────────────────────────────────────
using UnityEngine;
using EmpathyVR.Audio;
 
[RequireComponent(typeof(Collider))]
public class AmbientAudioTrigger : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private string ambientTag;
    [Tooltip("If set, also play a one-shot SFX when entering (e.g. a specific sound cue).")]
    [SerializeField] private AudioClip entrySFX;
 
    [Header("Behaviour")]
    [SerializeField] private bool triggerOnce = false;
    [SerializeField] private bool restoreOnExit = false;
    [SerializeField] private string exitRestoreTag = "";
 
    private bool _hasTriggered = false;
    private string _previousTag = "";
 
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }
 
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_hasTriggered && triggerOnce) return;
 
        _hasTriggered = true;
        _previousTag = AudioManager.Instance?.CurrentAmbientTag ?? "";
 
        AudioManager.Instance?.PlayAmbient(ambientTag);
 
        if (entrySFX != null)
            AudioManager.Instance?.PlayOneShot(entrySFX);
    }
 
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!restoreOnExit) return;
 
        string restoreTag = string.IsNullOrEmpty(exitRestoreTag) ? _previousTag : exitRestoreTag;
        if (!string.IsNullOrEmpty(restoreTag))
            AudioManager.Instance?.PlayAmbient(restoreTag);
    }
 
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 0.5f, 0.2f);
        var col = GetComponent<BoxCollider>();
        if (col != null)
            Gizmos.DrawCube(transform.position + col.center, col.size);
    }
}