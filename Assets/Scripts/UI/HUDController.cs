using UnityEngine;
using TMPro;
using DG.Tweening;
using EmpathyVR.Core;


namespace EmpathyVR.UI
{
    /// <summary>
    /// Controls all persistent HUD elements: subtitle bar, chapter label, timer circle.
    /// All UI transitions go through here to keep visual logic centralized.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        public static HUDController Instance { get; private set; }

        [Header("Subtitle Bar")]
        [SerializeField] private CanvasGroup subtitleGroup;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;

        [Header("Chapter Info")]
        [SerializeField] private TextMeshProUGUI chapterLabel;    // "CHAPTER 1 OF 2"
        [SerializeField] private TextMeshProUGUI sceneTitleLabel; // "The Farmer's Morning"

        [Header("Timer")]
        [SerializeField] private UnityEngine.UI.Image timerCircleFill;

        private Tweener _subtitleFadeTweener;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.RegisterHUDController(this);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.UnregisterHUDController(this);
        }

        public void ShowSubtitle(string speaker, string dialogue)
        {
            speakerNameText.text = speaker.ToUpper();
            dialogueText.text = $"\"{dialogue}\"";

            _subtitleFadeTweener?.Kill();
            subtitleGroup.alpha = 0;
            _subtitleFadeTweener = subtitleGroup.DOFade(1f, 0.4f);
        }

        public void ClearSubtitle()
        {
            _subtitleFadeTweener?.Kill();
            _subtitleFadeTweener = subtitleGroup.DOFade(0f, 0.6f);
        }

        public void UpdateChapterLabel(int current, int total)
        {
            chapterLabel.text = $"CHAPTER {current} OF {total}";
        }

        public void UpdateSceneTitle(string title)
        {
            sceneTitleLabel.text = title;
        }

        public void SetTimerFill(float normalized) // 0..1
        {
            timerCircleFill.fillAmount = normalized;
        }
    }
}