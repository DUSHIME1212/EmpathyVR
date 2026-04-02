using System.Collections.Generic;
using UnityEngine;

namespace EmpathyVR.Data
{
    // Runtime-only ScriptableObject — lives in memory, reset between sessions
    [CreateAssetMenu(fileName = "PlayerChoiceRecord", menuName = "EmpathyVR/Player Choice Record")]
    public class SO_PlayerChoiceRecord : ScriptableObject
    {
        [System.Serializable]
        public struct ChoiceEntry
        {
            public string decisionId;
            public string chosenOptionId;
            public string chosenLabel;
            public string outcomeHeadline;
            public float timeToDecide;
            public bool decidedByTimeout; // true if Amara decided (timer ran out)
        }

        public List<ChoiceEntry> choices = new List<ChoiceEntry>();
        public float totalExperienceTime;

        public void RecordChoice(string decisionId, string optionId, string label, string headline, float time, bool timeout)
        {
            choices.Add(new ChoiceEntry
            {
                decisionId = decisionId,
                chosenOptionId = optionId,
                chosenLabel = label,
                outcomeHeadline = headline,
                timeToDecide = time,
                decidedByTimeout = timeout
            });
        }

        public void Reset()
        {
            choices.Clear();
            totalExperienceTime = 0f;
        }
    }
}