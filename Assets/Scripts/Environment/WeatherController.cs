// ─────────────────────────────────────────────────────────────────────────────
// FILE: WeatherController.cs
// Place in: Assets/_Project/Scripts/Environment/
// Purpose: Controls time-of-day lighting and atmospheric effects.
//          GameManager calls SetWeatherState() as chapters progress.
//          Drought worsens → sky gets hazier, sun harsher, ambient cooler.
// ─────────────────────────────────────────────────────────────────────────────
using UnityEngine;
using DG.Tweening;
 
public class WeatherController : MonoBehaviour
{
    public enum WeatherState
    {
        Dawn,           // Hopeful golden hour — start of experience
        MidMorning,     // Bright, dry — crop walk
        HarshMidday,    // Intense, bleaching light — tension peaks
        Overcast        // Muted, uncertain — reflection
    }
 
    [Header("Sun")]
    [SerializeField] private Light sunLight;
    [SerializeField] private Gradient sunColorOverTime;
 
    [Header("Ambient")]
    [SerializeField] private Color dawnAmbient     = new Color(0.6f, 0.5f, 0.35f);
    [SerializeField] private Color midMornAmbient  = new Color(0.55f, 0.55f, 0.45f);
    [SerializeField] private Color harshAmbient    = new Color(0.65f, 0.58f, 0.4f);
    [SerializeField] private Color overcastAmbient = new Color(0.35f, 0.38f, 0.35f);
 
    [Header("Sun Angles (X rotation)")]
    [SerializeField] private float dawnAngle      = 10f;
    [SerializeField] private float midMornAngle   = 35f;
    [SerializeField] private float harshAngle     = 70f;
    [SerializeField] private float overcastAngle  = 45f;
 
    [Header("Transition")]
    [SerializeField] private float transitionDuration = 3f;
 
    private WeatherState _currentState = WeatherState.Dawn;
 
    private void Start()
    {
        ApplyState(WeatherState.Dawn, instant: true);
    }
 
    public void SetWeatherState(WeatherState newState, bool instant = false)
    {
        if (_currentState == newState) return;
        _currentState = newState;
        ApplyState(newState, instant);
    }
 
    private void ApplyState(WeatherState state, bool instant)
    {
        float targetAngle = state switch
        {
            WeatherState.Dawn       => dawnAngle,
            WeatherState.MidMorning => midMornAngle,
            WeatherState.HarshMidday => harshAngle,
            WeatherState.Overcast   => overcastAngle,
            _                       => dawnAngle
        };
 
        Color targetAmbient = state switch
        {
            WeatherState.Dawn       => dawnAmbient,
            WeatherState.MidMorning => midMornAmbient,
            WeatherState.HarshMidday => harshAmbient,
            WeatherState.Overcast   => overcastAmbient,
            _                       => dawnAmbient
        };
 
        if (instant)
        {
            sunLight.transform.rotation = Quaternion.Euler(targetAngle, 170f, 0f);
            RenderSettings.ambientLight = targetAmbient;
        }
        else
        {
            float startAngle = sunLight.transform.eulerAngles.x;
            DOTween.To(
                () => startAngle,
                x => sunLight.transform.rotation = Quaternion.Euler(x, 170f, 0f),
                targetAngle,
                transitionDuration
            );
 
            Color startAmbient = RenderSettings.ambientLight;
            DOTween.To(
                () => startAmbient,
                c => RenderSettings.ambientLight = c,
                targetAmbient,
                transitionDuration
            );
        }
    }
}