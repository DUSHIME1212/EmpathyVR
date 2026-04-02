using UnityEngine;
using UnityEngine.UI;

public class GazeReticleUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void SetFillAmount(float normalizedAmount)
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = normalizedAmount;
        }
    }
}
