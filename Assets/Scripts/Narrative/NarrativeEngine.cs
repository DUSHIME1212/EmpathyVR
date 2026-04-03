using UnityEngine;
using System.Collections;
using EmpathyVR.Data;
using EmpathyVR.UI;
using EmpathyVR.Core;

namespace EmpathyVR.Narrative
{
    /// <summary>
    /// Plays narration audio and drives subtitle display.
    /// Fires callback to GameManager when narration ends.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class NarrativeEngine : MonoBehaviour
    {
        [SerializeField] private HUDController hudController;

        public static NarrativeEngine Instance { get; private set; }
        private AudioSource _audioSource;
        private SO_NarrativeChapter _currentChapter;
        private Coroutine _playbackCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError($"<color=red>[NarrativeEngine]</color> CRITICAL: Duplicate NarrativeEngine found on {gameObject.name}. " +
                               "Please delete the duplicate! The current active one is on: " + Instance.gameObject.name);
                return;
            }
            Instance = this;

            _audioSource = GetComponent<AudioSource>();
            _audioSource.spatialBlend = 0f; // 2D narration — always clear
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.RegisterNarrativeEngine(this);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            if (GameManager.Instance != null)
                GameManager.Instance.UnregisterNarrativeEngine(this);
        }

        public void PlayChapter(SO_NarrativeChapter chapter)
        {
            _currentChapter = chapter;
            if (_playbackCoroutine != null) StopCoroutine(_playbackCoroutine);
            _playbackCoroutine = StartCoroutine(PlayNarrationRoutine(chapter));
        }

        public void SkipCurrent()
        {
            if (_playbackCoroutine != null) StopCoroutine(_playbackCoroutine);
            _audioSource.Stop();

            if (hudController != null)
                hudController.ClearSubtitle();
            else
                Debug.LogWarning("[NarrativeEngine] HUDController is unassigned in Inspector! Subtitles will not clear.");

            GameManager.Instance.OnChapterNarrationComplete(_currentChapter);
        }

        private IEnumerator PlayNarrationRoutine(SO_NarrativeChapter chapter)
        {
            // Show speaker name and text
            if (hudController != null)
                hudController.ShowSubtitle(chapter.speakerName, chapter.narrativeText);
            else
                Debug.LogError("[NarrativeEngine] HUDController is MISSING! Skipping subtitles display.");

            // Play audio if available
            if (chapter.narrationAudio != null)
            {
                _audioSource.clip = chapter.narrationAudio;
                _audioSource.Play();
                yield return new WaitWhile(() => _audioSource.isPlaying);
            }
            else
            {
                // Fallback: derive read time from text length (~180 wpm)
                float readTime = chapter.narrativeText.Length / 15f;
                yield return new WaitForSeconds(readTime);
            }

            if (hudController != null)
                hudController.ClearSubtitle();

            // Slight pause before triggering next step
            yield return new WaitForSeconds(0.8f);

            GameManager.Instance.OnChapterNarrationComplete(chapter);
        }
    }
}