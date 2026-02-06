using Core.Events;
using Gameplay.Config;
using System;
using UnityEngine;

namespace Gameplay.Cactus
{
    public sealed class CactusFaceUvOffsetController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameConfig config;
        [SerializeField] private Renderer cactusRenderer;

        private const float AtlasTilingX = 0.5f;
        private const float CryOffsetX = 0.0f;
        private const float SmileOffsetX = 0.5f;

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
            bool shouldCry =
                _autoFailedThisDawn &&
                _temperatureC >= config.safeMinTempC &&
                _temperatureC <= config.safeMaxTempC;

            if (shouldCry == _isCrying)
                return;

            (shouldCry ? (Action)Cry : Smile)();
        }

        private void Cry()
        {
            _isCrying = true;
            SetFaceOffset(CryOffsetX);
        }

        private void Smile()
        {
            _isCrying = false;
            SetFaceOffset(SmileOffsetX);
        }

        private void SetFaceOffset(float offsetX)
        {
            cactusRenderer.GetPropertyBlock(_mpb);
            _mpb.SetVector(BaseMapStId, new Vector4(AtlasTilingX, 1f, offsetX, 0f));
            cactusRenderer.SetPropertyBlock(_mpb);
        }
    }
}
