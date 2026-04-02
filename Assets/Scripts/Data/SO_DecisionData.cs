using System.Collections.Generic;
using UnityEngine;

namespace EmpathyVR.Data
{
    [CreateAssetMenu(fileName = "Decision_", menuName = "EmpathyVR/Decision")]
    public class SO_DecisionData : ScriptableObject
    {
        [Header("Identity")]
        public string decisionId;
        [TextArea(2, 4)]
        public string promptText;          // "Your water reserves are almost gone. What do you do?"
        public float timeLimitSeconds = 8f;

        [Header("Options")]
        public List<DecisionOptionData> options;

        [Header("Real Impact")]
        [TextArea(2, 4)]
        public string realWorldImpactText; // "1 in 3 Rwandan farmers faced this choice in 2023."
    }

    [System.Serializable]
    public class DecisionOptionData
    {
        public string optionId;
        public string label;               // "Use the water on the crops"
        public string consequencePreview;  // "Your family may go thirsty today"
        public string icon;                // Name of icon sprite in Resources
        [TextArea(2, 5)]
        public string outcomeHeadline;     // "The crops survive. Your children drink less water today."
        [TextArea(2, 4)]
        public string outcomeDetail;
        public ConsequenceType consequenceType;
    }

    public enum ConsequenceType
    {
        FamilyWelfare,
        EconomicSurvival,
        LongTermGrowth,
        ChildrenFuture
    }
}