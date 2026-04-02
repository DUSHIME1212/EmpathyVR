using UnityEngine;

/// <summary>
/// Casts a ray from the camera center. On dwell completion, calls the
/// IInteractable interface on the hit object.
/// Works in both VR (eye ray) and desktop (mouse center).
/// </summary>
public class GazeInteractor : MonoBehaviour
{
    [SerializeField] private float maxGazeDistance = 10f;
    [SerializeField] private float dwellDuration = 2f;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private GazeReticleUI gazeReticle;

    private IInteractable _currentTarget;
    private float _dwellTimer;

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxGazeDistance, interactableMask))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != _currentTarget)
            {
                _currentTarget?.OnGazeExit();
                _currentTarget = interactable;
                _dwellTimer = 0f;
                _currentTarget?.OnGazeEnter();
            }

            _dwellTimer += Time.deltaTime;
            gazeReticle.SetFillAmount(_dwellTimer / dwellDuration);

            if (_dwellTimer >= dwellDuration)
            {
                _currentTarget?.OnDwellComplete();
                _currentTarget = null;
                _dwellTimer = 0f;
                gazeReticle.SetFillAmount(0f);
            }
        }
        else
        {
            if (_currentTarget != null)
            {
                _currentTarget.OnGazeExit();
                _currentTarget = null;
                _dwellTimer = 0f;
                gazeReticle.SetFillAmount(0f);
            }
        }
    }
}

// ── Interface ─────────────────────────────────────────────────────
public interface IInteractable
{
    void OnGazeEnter();
    void OnGazeExit();
    void OnDwellComplete();
}