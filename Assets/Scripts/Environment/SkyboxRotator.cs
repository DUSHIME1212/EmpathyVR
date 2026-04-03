using UnityEngine;

namespace EmpathyVR.Environment
{
    /// <summary>
    /// Smoothly rotates the global skybox material over time.
    /// This adds a dynamic feel to 360-photo or panoramic backgrounds in VR.
    /// </summary>
    public class SkyboxRotator : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [Tooltip("How fast the skybox rotates (degrees per second). Suggested: 0.5 to 1.5.")]
        [SerializeField] private float rotationSpeed = 1.0f;

        private float _currentRotation = 0f;
        private static readonly int RotationProp = Shader.PropertyToID("_Rotation");

        private void Update()
        {
            // Calculate new rotation based on time and speed
            _currentRotation += rotationSpeed * Time.deltaTime;

            // Loop rotation back to 0 once it exceeds 360 to prevent large numbers
            if (_currentRotation >= 360f) _currentRotation -= 360f;
            if (_currentRotation < 0f) _currentRotation += 360f;

            // Apply rotation to the active skybox material
            if (RenderSettings.skybox != null)
            {
                RenderSettings.skybox.SetFloat(RotationProp, _currentRotation);
            }
        }

        /// <summary>
        /// Public API to set rotation speed at runtime (e.g., from an event).
        /// </summary>
        public void SetRotationSpeed(float speed)
        {
            rotationSpeed = speed;
        }

        private void OnDisable()
        {
            // Reset rotation when the object is disabled (optional)
            // if (RenderSettings.skybox != null) RenderSettings.skybox.SetFloat(RotationProp, 0f);
        }
    }
}
