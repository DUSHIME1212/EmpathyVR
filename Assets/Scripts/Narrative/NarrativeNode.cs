// ─────────────────────────────────────────────────────────────────────────────
// FILE: NarrativeNode.cs
// Place in: Assets/_Project/Scripts/Narrative/
// Purpose: A single node in the narrative graph — one beat of story.
//          Chapters are made of ordered NarrativeNodes.
// ─────────────────────────────────────────────────────────────────────────────
using UnityEngine;
 
[System.Serializable]
public class NarrativeNode
{
    [Header("Identity")]
    public string nodeId;
    public int orderIndex;               // Position within a chapter
 
    [Header("Content")]
    public string speakerName;           // e.g. "AMARA (INNER VOICE)"
    [TextArea(3, 8)]
    public string dialogueText;          // Subtitle shown on screen
    public AudioClip voiceClip;          // Optional recorded narration
 
    [Header("Timing")]
    public float preDelay = 0.3f;        // Pause before this node plays
    public float postDelay = 0.5f;       // Pause after this node ends
    [Tooltip("If no voiceClip, display for this many seconds instead.")]
    public float fallbackDisplayTime = 4f;
 
    [Header("Environment Reaction")]
    [Tooltip("Tag of an InteractableObject to highlight when this node plays.")]
    public string highlightObjectTag = "";
    [Tooltip("Ambient SFX tag to trigger on this node.")]
    public string sfxTag = "";
 
    [Header("Flow")]
    public bool isLastInChapter = false;
}
 