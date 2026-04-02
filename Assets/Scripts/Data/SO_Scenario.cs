using System.Collections.Generic;
using UnityEngine;

namespace EmpathyVR.Data
{
    [CreateAssetMenu(fileName = "Scenario_", menuName = "EmpathyVR/Scenario")]
    public class SO_Scenario : ScriptableObject
    {
        [Header("Identity")]
        public string scenarioId;
        public string title;
        [TextArea(3, 6)]
        public string description;
        public Sprite thumbnailImage;
        public string category; // "NATURE", "PERSPECTIVE", "ABSTRACT"
        public int estimatedMinutes;

        [Header("Content")]
        public List<SO_NarrativeChapter> chapters;
        public List<SO_DecisionData> decisions;
        public string sceneName; // The Unity scene to load for this scenario

        [Header("Meta")]
        public string comfortLevel; // "Sitting or Standing"
        public bool isUnlocked = true;
    }
}