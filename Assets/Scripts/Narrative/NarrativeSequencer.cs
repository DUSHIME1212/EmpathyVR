
// ─────────────────────────────────────────────────────────────────────────────
// FILE: NarrativeSequencer.cs
// Place in: Assets/_Project/Scripts/Narrative/
// Purpose: Iterates through NarrativeNodes in a chapter one-by-one.
//          NarrativeEngine owns this and calls it per chapter.
// ─────────────────────────────────────────────────────────────────────────────
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EmpathyVR.UI;
using EmpathyVR.Audio;
using EmpathyVR.Environment;
 
public class NarrativeSequencer : MonoBehaviour
{
    // ── Dependencies injected by NarrativeEngine ───────────────────────────
    private HUDController _hud;
    private AudioManager _audio;
    private EnvironmentManager _environment;
 
    private Coroutine _sequenceCoroutine;
 
    public void Inject(HUDController hud, AudioManager audio, EnvironmentManager env)
    {
        _hud = hud;
        _audio = audio;
        _environment = env;
    }
 
    /// <summary>
    /// Plays all nodes in order. Fires onComplete when the last node finishes.
    /// </summary>
    public void PlaySequence(List<NarrativeNode> nodes, AudioSource audioSource, Action onComplete)
    {
        if (_sequenceCoroutine != null) StopCoroutine(_sequenceCoroutine);
        _sequenceCoroutine = StartCoroutine(SequenceRoutine(nodes, audioSource, onComplete));
    }
 
    public void StopSequence()
    {
        if (_sequenceCoroutine != null)
        {
            StopCoroutine(_sequenceCoroutine);
            _sequenceCoroutine = null;
        }
        _hud?.ClearSubtitle();
    }
 
    private IEnumerator SequenceRoutine(List<NarrativeNode> nodes, AudioSource audioSource, Action onComplete)
    {
        foreach (var node in nodes)
        {
            // Pre-delay
            if (node.preDelay > 0f)
                yield return new WaitForSeconds(node.preDelay);
 
            // Show subtitle
            _hud?.ShowSubtitle(node.speakerName, node.dialogueText);
 
            // Trigger optional environment reactions
            if (!string.IsNullOrEmpty(node.highlightObjectTag))
                _environment?.HighlightByTag(node.highlightObjectTag);
 
            if (!string.IsNullOrEmpty(node.sfxTag))
                _audio?.PlaySFX(node.sfxTag);
 
            // Play audio or wait fallback time
            if (node.voiceClip != null)
            {
                audioSource.clip = node.voiceClip;
                audioSource.Play();
                yield return new WaitWhile(() => audioSource.isPlaying);
            }
            else
            {
                yield return new WaitForSeconds(node.fallbackDisplayTime);
            }
 
            // Clear subtitle between nodes
            _hud?.ClearSubtitle();
 
            // Post-delay
            if (node.postDelay > 0f)
                yield return new WaitForSeconds(node.postDelay);
        }
 
        onComplete?.Invoke();
    }
}
 