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

        private AudioSource _audioSource;
        private SO_NarrativeChapter _currentChapter;
        private Coroutine _playbackCoroutine;

        private void Awake()
        {
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
            hudController.ClearSubtitle();
            GameManager.Instance.OnChapterNarrationComplete(_currentChapter);
        }

        private IEnumerator PlayNarrationRoutine(SO_NarrativeChapter chapter)
        {
            // Show speaker name and text
            hudController.ShowSubtitle(chapter.speakerName, chapter.narrativeText);

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

            hudController.ClearSubtitle();

            // Slight pause before triggering next step
            yield return new WaitForSeconds(0.8f);

            GameManager.Instance.OnChapterNarrationComplete(chapter);
        }
    }
}