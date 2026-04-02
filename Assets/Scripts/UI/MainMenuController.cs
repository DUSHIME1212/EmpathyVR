using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using EmpathyVR.Core;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject loadingSpinner;
    [SerializeField] private TextMeshProUGUI initLabel;
    [SerializeField] private Button startButton;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        startButton.gameObject.SetActive(false);
        StartCoroutine(InitSequence());
    }

    private IEnumerator InitSequence()
    {
        // Show spinner for 2 seconds
        yield return new WaitForSeconds(2f);

        // Swap spinner for Start button
        loadingSpinner.SetActive(false);
        initLabel.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);

        // Fade button in
        var cg = startButton.GetComponent<CanvasGroup>();
        if (cg == null) cg = startButton.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.DOFade(1f, 0.5f);

        startButton.onClick.AddListener(() =>
            SceneLoader.Instance.LoadScene("02_Briefing"));
    }
}