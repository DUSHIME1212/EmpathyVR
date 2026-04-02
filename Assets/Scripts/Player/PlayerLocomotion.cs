// ─────────────────────────────────────────────────────────────────────────────
// FILE: PlayerLocomotion.cs
// Place in: Assets/_Project/Scripts/Player/
// Purpose: Handles scripted teleport locomotion along a chapter path.
//          In VR, smooth locomotion causes motion sickness — instead the player
//          teleports to pre-set waypoints as narration progresses.
//          Also supports a "free look" mode where the player can rotate head freely.
// ─────────────────────────────────────────────────────────────────────────────
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
 
public class PlayerLocomotion : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Ordered list of Transform waypoints the player moves through.")]
    [SerializeField] private List<Transform> waypoints;
 
    [Header("Locomotion Settings")]
    [SerializeField] private float fadeDuration = 0.4f;   // Black flash on teleport
    [SerializeField] private CanvasGroup fadeOverlay;
 
    [Header("XR Rig Root")]
    [SerializeField] private Transform xrRigRoot;         // The XR Origin / OVRCameraRig root
 
    private int _currentWaypointIndex = 0;
    private bool _isMoving = false;
 
    // ── Public API ─────────────────────────────────────────────────────────
    /// <summary>Called by NarrativeEngine / GameManager to advance to next waypoint.</summary>
    public void AdvanceToNextWaypoint()
    {
        if (_isMoving) return;
 
        int next = _currentWaypointIndex + 1;
        if (next >= waypoints.Count)
        {
            Debug.Log("[PlayerLocomotion] No more waypoints.");
            return;
        }
 
        StartCoroutine(TeleportRoutine(next));
    }
 
    /// <summary>Jump directly to a specific waypoint index (e.g. on scene load).</summary>
    public void TeleportToWaypoint(int index)
    {
        if (index < 0 || index >= waypoints.Count) return;
        _currentWaypointIndex = index;
        xrRigRoot.position = waypoints[index].position;
        xrRigRoot.rotation = Quaternion.Euler(0f, waypoints[index].eulerAngles.y, 0f);
    }
 
    // ── Internal ────────────────────────────────────────────────────────────
    private IEnumerator TeleportRoutine(int targetIndex)
    {
        _isMoving = true;
 
        // Fade to black
        if (fadeOverlay != null)
            yield return fadeOverlay.DOFade(1f, fadeDuration).WaitForCompletion();
 
        // Move instantly while blacked out
        _currentWaypointIndex = targetIndex;
        xrRigRoot.position = waypoints[targetIndex].position;
        xrRigRoot.rotation = Quaternion.Euler(0f, waypoints[targetIndex].eulerAngles.y, 0f);
 
        // Small pause for comfort
        yield return new WaitForSeconds(0.1f);
 
        // Fade back in
        if (fadeOverlay != null)
            yield return fadeOverlay.DOFade(0f, fadeDuration).WaitForCompletion();
 
        _isMoving = false;
    }
 
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count < 2) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
        Gizmos.color = Color.yellow;
        foreach (var wp in waypoints)
            if (wp != null) Gizmos.DrawSphere(wp.position, 0.15f);
    }
}