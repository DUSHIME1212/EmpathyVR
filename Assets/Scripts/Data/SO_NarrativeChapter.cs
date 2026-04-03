using UnityEngine;

namespace EmpathyVR.Data
{
    [CreateAssetMenu(fileName = "Chapter_", menuName = "EmpathyVR/Narrative Chapter")]
    public class SO_NarrativeChapter : ScriptableObject
    {
        [Header("Identity")]
        public string chapterId;
        public string chapterTitle;

        [Header("Content")]
        public string speakerName;
        [TextArea(3, 10)]
        public string narrativeText;
        public AudioClip narrationAudio;

        [Header("Sequence")]
        public int chapterNumber;
        public string ambientAudioTag;
        public Material chapterSkybox;       // New: Allows for 360 VR photos per chapter
        public bool triggerDecisionAfter;
        public SO_DecisionData nextDecision;
        public bool isFinalChapter;
    }
}
