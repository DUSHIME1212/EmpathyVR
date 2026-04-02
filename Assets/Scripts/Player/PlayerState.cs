using UnityEngine;

/// <summary>
/// Represents runtime state for the player during a session.
/// Does not persist across scenes — use SO_PlayerChoiceRecord for that.
/// </summary>
public class PlayerState : MonoBehaviour
{
    public float SessionStartTime { get; private set; }
    public bool IsInDecisionMode { get; set; }
    public bool IsNarrationPlaying { get; set; }

    private void Start()
    {
        SessionStartTime = Time.time;
    }

    public float GetSessionDuration()
    {
        return Time.time - SessionStartTime;
    }
}