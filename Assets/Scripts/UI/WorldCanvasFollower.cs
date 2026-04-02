using UnityEngine;

/// <summary>
/// Keeps a World Space canvas positioned relative to the camera.
/// Used in VR to keep the HUD/Panels in the user's field of view.
/// </summary>
public class WorldCanvasFollower : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float distance = 2.0f;
    [SerializeField] private float heightOffset = 0.0f;
    [SerializeField] private float smoothSpeed = 5.0f;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Position the canvas in front of the camera
        Vector3 targetPosition = cameraTransform.position + (cameraTransform.forward * distance);
        targetPosition.y += heightOffset;

        // Smoothly move towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        // Make the canvas face the camera
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
}
