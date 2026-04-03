using UnityEngine;

namespace EmpathyVR.UI
{
    /// <summary>
    /// Ensures the World Space Canvas is positioned at a comfortable distance 
    /// and orientation in front of the camera when the scene starts.
    /// Helps fix issues where the UI is positioned too far away in the Editor.
    /// </summary>
    public class WorldCanvasPositioner : MonoBehaviour
    {
        [Header("Position Settings")]
        [SerializeField] private float distanceFromCamera = 2.0f;
        [SerializeField] private float heightOffset = 0.0f;
        [SerializeField] private bool repositionOnStart = true;
        [SerializeField] private bool faceCamera = true;

        private void Start()
        {
            if (repositionOnStart)
            {
                RepositionCanvas();
            }
        }

        public void RepositionCanvas()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                Debug.LogWarning("[WorldCanvasPositioner] No Main Camera found! Cannot reposition UI.");
                return;
            }

            // Position: Fixed distance in front of camera
            Vector3 forward = mainCam.transform.forward;
            forward.y = 0; // Keep it level horizontally
            forward.Normalize();

            Vector3 targetPosition = mainCam.transform.position + (forward * distanceFromCamera);
            targetPosition.y += heightOffset;

            transform.position = targetPosition;

            // Rotation: Face the camera
            if (faceCamera)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
            }

            Debug.Log($"[WorldCanvasPositioner] UI repositioned to {transform.position}.");
        }
    }
}
