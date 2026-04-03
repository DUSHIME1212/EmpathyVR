 
// ─────────────────────────────────────────────────────────────────────────────
// FILE: ApplicationBootstrapper.cs
// Place in: Assets/_Project/Scripts/Core/
// Purpose: The very first script that runs in scene 00_Boot.
//          Ensures all singleton managers exist before any other scene loads.
//          If a manager prefab is missing it logs a clear error — no silent failures.
// ─────────────────────────────────────────────────────────────────────────────
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using EmpathyVR.Core;
using EmpathyVR.Audio;
 
public class ApplicationBootstrapper : MonoBehaviour
{
    [Header("Manager Prefabs (drag from Prefabs/Managers/)")]
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] private GameObject audioManagerPrefab;
    [SerializeField] private GameObject sceneLoaderPrefab;
 
    [Header("Boot Settings")]
    [SerializeField] private string firstSceneName = "01_MainMenu";
    [SerializeField] private float minimumSplashTime = 1.5f;  // Seconds to show logo
 
    private void Awake()
    {
        SpawnIfMissing<GameManager>(gameManagerPrefab, "GameManager");
        SpawnIfMissing<AudioManager>(audioManagerPrefab, "AudioManager");
        SpawnIfMissing<SceneLoader>(sceneLoaderPrefab, "SceneLoader");
    }
 
    private void Start()
    {
        StartCoroutine(BootSequence());
    }
 
    private IEnumerator BootSequence()
    {
        // Wait for minimum splash display and all managers to initialise
        yield return new WaitForSeconds(minimumSplashTime);
 
        // Validate critical systems before allowing the game to continue
        if (GameManager.Instance == null)
        {
            Debug.LogError("[Bootstrapper] CRITICAL: GameManager failed to initialise. Check prefab assignment.");
            yield break;
        }
 
        if (AudioManager.Instance == null)
            Debug.LogWarning("[Bootstrapper] AudioManager not found — audio will be silent.");
 
        SceneLoader.Instance?.LoadScene(firstSceneName);
    }
 
    private void SpawnIfMissing<T>(GameObject prefab, string label) where T : MonoBehaviour
    {
        // Use FindObjectsInactive.Include to catch managers that might be disabled (e.g. in a loading state)
        if (Object.FindAnyObjectByType<T>(FindObjectsInactive.Include) != null) return; 
 
        if (prefab == null)
        {
            Debug.LogError($"[Bootstrapper] '{label}' prefab not assigned. " +
                           $"Drag it into the Bootstrapper inspector field.");
            return;
        }
 
        var instance = Instantiate(prefab);
        instance.name = $"[{label}]";
        DontDestroyOnLoad(instance);
        Debug.Log($"[Bootstrapper] Spawned: {label}");
    }
}
 
 