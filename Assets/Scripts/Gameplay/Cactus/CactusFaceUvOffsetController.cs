using Core.Events;
using Gameplay.Config;
using UnityEngine;

namespace Gameplay.Cactus
{
    public sealed class CactusFaceUvOffsetController : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private Renderer cactusRenderer;

        private bool _autoFailedThisDawn;
        private float _temperatureC;
        private bool _isCrying;

        private MaterialPropertyBlock _mpb;

        private static readonly int BaseMapStId = Shader.PropertyToID("_BaseMap_ST");

        private void Awake()
        {
            if (config == null || cactusRenderer == null)
            {
                enabled = false;
                return;
            }

            _mpb = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            GameEvents.WindowStateChanged += OnWindowStateChanged;
            GameEvents.TemperatureChanged += OnTemperatureChanged;
        }

        private void OnDisable()
        {
            GameEvents.WindowStateChanged -= OnWindowStateChanged;
            GameEvents.TemperatureChanged -= OnTemperatureChanged;
        }

        private void OnWindowStateChanged(WindowState state, bool autoFailedThisDawn)
        {
            _autoFailedThisDawn = autoFailedThisDawn && state == WindowState.Closed;
            Evaluate();
        }

        private void OnTemperatureChanged(float tempC)
        {
            _temperatureC = tempC;
            Evaluate();
        }

        private void Evaluate()
        {
            bool inSafeRange =
                _temperatureC >= config.safeMinTempC &&
                _temperatureC <= config.safeMaxTempC;

            bool shouldCry = _autoFailedThisDawn && inSafeRange;

            if (shouldCry == _isCrying)
                return;

            if (shouldCry) Cry();
            else Smile();
        }

        private void Cry()
        {
            _isCrying = true;
            SetFaceOffset(0.0f);
        }

        private void Smile()
        {
            _isCrying = false;
            SetFaceOffset(0.5f);
        }

        private void SetFaceOffset(float offsetX)
        {
            cactusRenderer.GetPropertyBlock(_mpb);
            _mpb.SetVector(BaseMapStId, new Vector4(0.5f, 1f, offsetX, 0f));
            cactusRenderer.SetPropertyBlock(_mpb);
        }
    }
}
