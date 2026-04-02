using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EmpathyVR.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource ambientSource;
        [SerializeField] private AudioSource sfxSource;

        [System.Serializable]
        public struct TaggedClip { public string tag; public AudioClip clip; }

        [SerializeField] private List<TaggedClip> ambientClips;
        [SerializeField] private List<TaggedClip> sfxClips;

        public string CurrentAmbientTag { get; private set; } = "";

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }

        public void PlayBGM(AudioClip clip, float volume = 0.4f)
        {
            if (clip == null || (bgmSource.clip == clip && bgmSource.isPlaying)) return;
            bgmSource.clip = clip;
            bgmSource.volume = volume;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        public void PlayAmbient(string tag)
        {
            if (string.IsNullOrEmpty(tag) || ambientSource == null) return;
            var entry = ambientClips.Find(c => c.tag == tag);
            if (entry.clip == null) { Debug.LogWarning($"[AudioManager] Ambient '{tag}' not found."); return; }
            if (CurrentAmbientTag == tag && ambientSource.isPlaying) return;
            CurrentAmbientTag = tag;
            ambientSource.clip = entry.clip;
            ambientSource.loop = true;
            ambientSource.Play();
        }

        public void PlaySFX(string tag)
        {
            var entry = sfxClips.Find(c => c.tag == tag);
            if (entry.clip != null) sfxSource.PlayOneShot(entry.clip);
            else Debug.LogWarning($"[AudioManager] SFX '{tag}' not found.");
        }

        public void PlayOneShot(AudioClip clip)
        {
            if (clip != null) sfxSource.PlayOneShot(clip);
        }

        public void FadeOutBGM(float duration = 1.5f) => StartCoroutine(FadeRoutine(bgmSource, 0f, duration));
        public void FadeInBGM(float targetVol = 0.4f, float duration = 1.5f) => StartCoroutine(FadeRoutine(bgmSource, targetVol, duration));

        private IEnumerator FadeRoutine(AudioSource src, float target, float dur)
        {
            float start = src.volume, elapsed = 0f;
            while (elapsed < dur)
            {
                src.volume = Mathf.Lerp(start, target, elapsed / dur);
                elapsed += Time.deltaTime;
                yield return null;
            }
            src.volume = target;
            if (target <= 0f) src.Stop();
        }
    }
}