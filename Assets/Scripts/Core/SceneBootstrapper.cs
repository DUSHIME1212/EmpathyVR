using UnityEngine;
using EmpathyVR.Core;
using EmpathyVR.Audio;

namespace EmpathyVR.Core
{
    /// <summary>
    /// A helper component to ensure core managers are spawned when starting a scene directly in the Editor.
    /// This is NOT needed if starting from the 00_Boot scene.
    /// Place this in your scene and assign the manager prefabs.
    /// </summary>
    public class SceneBootstrapper : MonoBehaviour
    {
        [Header("Manager Prefabs")]
        [SerializeField] private GameObject gameManagerPrefab;
        [SerializeField] private GameObject audioManagerPrefab;
        [SerializeField] private GameObject sceneLoaderPrefab;

        private void Awake()
        {
            // If the ApplicationBootstrapper already ran (e.g. we came from 00_Boot),
            // these singletons will already exist and we should do nothing.
            
            SpawnIfMissing<GameManager>(gameManagerPrefab, "GameManager");
            SpawnIfMissing<AudioManager>(audioManagerPrefab, "AudioManager");
            SpawnIfMissing<SceneLoader>(sceneLoaderPrefab, "SceneLoader");

            // Ensure we have an EventSystem for UI interaction
            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var eventSystemGO = new GameObject("EventSystem (Bootstrapped)");
                eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                
                // Note: Using the new Input System module if available, otherwise fallback
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
                eventSystemGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
                eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
#endif
                Debug.Log("[SceneBootstrapper] Bootstrapped missing EventSystem.");
            }
        }

        private void SpawnIfMissing<T>(GameObject prefab, string label) where T : MonoBehaviour
        {
            if (Object.FindFirstObjectByType<T>() != null) return;

            if (prefab == null)
            {
                Debug.LogWarning($"[SceneBootstrapper] {label} prefab is not assigned! Manager will not be spawned.");
                return;
            }

            var instance = Instantiate(prefab);
            instance.name = $"[{label}] (Bootstrapped)";
            // The managers' own Awake/Start handle DontDestroyOnLoad
            Debug.Log($"[SceneBootstrapper] Bootstrapped manager: {label}");
        }
    }
}
