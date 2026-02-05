using UnityEngine;

namespace Debugging
{
    [RequireComponent(typeof(Light))]
    public sealed class DiscoLight : MonoBehaviour
    {
        [SerializeField] private float colorSpeed = 1.5f;
        [SerializeField] private float intensityPulse = 0f;
        [SerializeField] private float pulseSpeed = 3f;

        private Light _discoLight;
        private float _hue;

        void Awake()
        {
            _discoLight = GetComponent<Light>();
        }

        void Update()
        {
            // Cycle _hue (0–1)
            _hue += Time.deltaTime * colorSpeed;
            if (_hue > 1f) _hue -= 1f;

            // Bright, saturated color
            _discoLight.color = Color.HSVToRGB(_hue, 1f, 1f);

            // Optional intensity pulsing
            if (intensityPulse > 0f)
            {
                _discoLight.intensity =
                    Mathf.Lerp(1f, intensityPulse, (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
            }
        }
    }
}
