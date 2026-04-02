 
// ─────────────────────────────────────────────────────────────────────────────
// FILE: DecisionPoint.cs
// Place in: Assets/_Project/Scripts/Decisions/
// Purpose: MonoBehaviour component placed on a world-space trigger volume.
//          When the player enters this zone, it signals DecisionManager to
//          present its assigned decision. Used for proximity-based triggers
//          (e.g. walking up to the water point spawns the water decision).
// ─────────────────────────────────────────────────────────────────────────────
using UnityEngine;
using EmpathyVR.Data;
using EmpathyVR.Decisions;
 
[RequireComponent(typeof(Collider))]
public class DecisionPoint : MonoBehaviour
{
    [Header("Decision to Trigger")]
    [SerializeField] private SO_DecisionData decisionData;
 
    [Header("Behaviour")]
    [SerializeField] private bool triggerOnce = true;
 
    private bool _hasTriggered = false;
    private DecisionManager _decisionManager;
 
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
        _decisionManager = FindFirstObjectByType<DecisionManager>();
    }
 
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_hasTriggered && triggerOnce) return;
        if (decisionData == null)
        {
            Debug.LogWarning($"[DecisionPoint] '{gameObject.name}' has no SO_DecisionData assigned.", this);
            return;
        }
 
        _hasTriggered = true;
        _decisionManager?.PresentDecision(decisionData);
    }
 
    // Draw the trigger zone in the editor so designers can see it
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.3f);
        var col = GetComponent<BoxCollider>();
        if (col != null)
            Gizmos.DrawCube(transform.position + col.center, col.size);
    }
}
 
