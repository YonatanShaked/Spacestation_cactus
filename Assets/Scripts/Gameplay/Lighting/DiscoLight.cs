using UnityEngine;

namespace Gameplay.Lighting
{
    /// <summary>
    /// Simple decorative light controller that cycles light color over time.
    /// Optionally applies a pulsing intensity effect to create a disco-style lighting effect.
    /// Intended for non-critical visual flair and isolated from core gameplay logic.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public sealed class DiscoLight : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float colorSpeed = 1.5f;
        [SerializeField] private float intensityPulse = 0f;
        [SerializeField] private float pulseSpeed = 3f;

        private Light _discoLight;
        private float _hue;

        private void Awake()
        {
            _discoLight = GetComponent<Light>();
        }

        private void Update()
        {
            _hue += Time.deltaTime * colorSpeed;

            if (_hue > 1f)
                _hue -= 1f;

            _discoLight.color = Color.HSVToRGB(_hue, 1f, 1f);

            if (intensityPulse > 0f)
                _discoLight.intensity = Mathf.Lerp(1f, intensityPulse, (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
        }
    }
}
