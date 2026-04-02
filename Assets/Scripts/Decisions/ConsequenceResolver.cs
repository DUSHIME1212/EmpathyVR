 
// ─────────────────────────────────────────────────────────────────────────────
// FILE: ConsequenceResolver.cs
// Place in: Assets/_Project/Scripts/Decisions/
// Purpose: Translates a player's choice history into a consequence summary
//          used by the Reflection scene. Stateless utility class.
// ─────────────────────────────────────────────────────────────────────────────
using UnityEngine;
using System.Collections.Generic;
using EmpathyVR.Data;
 
public class ConsequenceResolver : MonoBehaviour
{
    [System.Serializable]
    public struct ConsequenceSummary
    {
        public int totalDecisions;
        public int decidedByPlayer;
        public int decidedByTimeout;          // "Amara decided"
        public float averageDecisionTime;
        public List<string> outcomeSentences; // For the reflection display
    }
 
    /// <summary>
    /// Builds a readable summary from the player's choice record.
    /// Call this in ReflectionUI.
    /// </summary>
    public static ConsequenceSummary Resolve(SO_PlayerChoiceRecord record)
    {
        var summary = new ConsequenceSummary
        {
            outcomeSentences = new List<string>()
        };
 
        if (record == null || record.choices.Count == 0)
            return summary;
 
        float totalTime = 0f;
 
        foreach (var choice in record.choices)
        {
            summary.totalDecisions++;
            totalTime += choice.timeToDecide;
 
            if (choice.decidedByTimeout)
                summary.decidedByTimeout++;
            else
                summary.decidedByPlayer++;
 
            summary.outcomeSentences.Add(choice.outcomeHeadline);
        }
 
        summary.averageDecisionTime = totalTime / summary.totalDecisions;
        return summary;
    }
 
    /// <summary>
    /// Returns a reflection message based on how quickly the player decided.
    /// Encourages empathy discussion in group settings.
    /// </summary>
    public static string GetReflectionPrompt(ConsequenceSummary summary)
    {
        if (summary.decidedByTimeout > 0)
            return "You let Amara decide for you on some choices. " +
                   "What made those decisions hard to make?";
 
        if (summary.averageDecisionTime < 3f)
            return "You decided quickly. Would you make the same choices " +
                   "if you had more time to think?";
 
        return "Every choice had a cost. Which decision sat heaviest with you — and why?";
    }
}
