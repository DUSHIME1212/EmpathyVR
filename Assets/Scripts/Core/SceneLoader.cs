using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

namespace EmpathyVR.Core
{
    /// <summary>
    /// Handles all scene transitions with a fade-to-black.
    /// The fade overlay is created in code so it is always DontDestroyOnLoad.
    /// Never call SceneManager.LoadScene directly — always go through here.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private float fadeDuration = 1.2f;

        private CanvasGroup _fadeOverlay;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);

            CreateFadeOverlay();
        }

        private void CreateFadeOverlay()
        {
            var overlayGO = new GameObject("FadeOverlay");
            overlayGO.transform.SetParent(transform, false);

            canvas = overlayGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;

            overlayGO.AddComponent<CanvasScaler>();
            overlayGO.AddComponent<GraphicRaycaster>();

            var imageGO = new GameObject("BlackPanel");
            imageGO.transform.SetParent(overlayGO.transform, false);

            var img = imageGO.AddComponent<Image>();
            img.color = Color.black;

            var rect = imageGO.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            _fadeOverlay = overlayGO.AddComponent<CanvasGroup>();
            _fadeOverlay.alpha = 0f;
            _fadeOverlay.blocksRaycasts = false;
            _fadeOverlay.interactable = false;

            overlayGO.SetActive(false); // Start inactive
            Debug.Log("[SceneLoader] FadeOverlay created programmatically and deactivated.");
        }

        private Canvas canvas; // Store to manage sorting if needed

        public void LoadScene(string sceneName)
        {
            StartCoroutine(TransitionRoutine(sceneName));
        }

        private IEnumerator TransitionRoutine(string sceneName)
        {
            if (_fadeOverlay == null)
            {
                Debug.LogWarning("[SceneLoader] _fadeOverlay is null! Loading scene immediately.");
                SceneManager.LoadScene(sceneName);
                yield break;
            }

            _fadeOverlay.gameObject.SetActive(true);
            _fadeOverlay.blocksRaycasts = true;

            // Fade to black
            yield return _fadeOverlay.DOFade(1f, fadeDuration).SetUpdate(true).WaitForCompletion();

            // Load new scene
            yield return SceneManager.LoadSceneAsync(sceneName);

            // Fade back in
            yield return _fadeOverlay.DOFade(0f, fadeDuration).SetUpdate(true).WaitForCompletion();

            _fadeOverlay.blocksRaycasts = false;
            _fadeOverlay.gameObject.SetActive(false);
        }
    }
}