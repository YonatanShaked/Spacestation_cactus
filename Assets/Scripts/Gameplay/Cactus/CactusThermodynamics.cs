using Core.Events;
using Gameplay.Config;
using UnityEngine;

namespace Gameplay.Cactus
{
    /// <summary>
    /// Simulates cactus temperature behavior and life state.
    /// Updates temperature based on window state and AC upgrades, enforces alive temperature limits with a grace period,
    /// and publishes temperature changes and death events to the rest of the game.
    /// </summary>
    public sealed class CactusThermodynamics : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameConfig config;

        private float _tempC;
        private bool _isAlive = true;

        private WindowState _windowState = WindowState.Closed;
        private bool _acPurchased;

        private float _outOfAliveRangeTimer;

        public float TemperatureC => _tempC;
        public bool IsAlive => _isAlive;

        private void Awake()
        {
            if (config == null)
            {
                Debug.LogError($"{nameof(CactusThermodynamics)} missing config.", this);
                enabled = false;
                return;
            }

            _tempC = config.startingTempC;
        }

        private void OnEnable()
        {
            GameEvents.WindowStateChanged += OnWindowStateChanged;
            GameEvents.AcPurchasedChanged += OnAcPurchasedChanged;
        }

        private void OnDisable()
        {
            GameEvents.WindowStateChanged -= OnWindowStateChanged;
            GameEvents.AcPurchasedChanged -= OnAcPurchasedChanged;
        }

        private void Start()
        {
            PublishTemperature();
        }

        private void Update()
        {
            if (!_isAlive)
                return;

            float delta = (_windowState == WindowState.Open)
                ? config.tempIncreasePerSecondWhenOpen
                : -config.tempDecreasePerSecondWhenClosed;

            _tempC += delta * Time.deltaTime;

            float minAlive = config.baseMinAliveTempC;
            float maxAlive = _acPurchased ? config.acMaxAliveTempC : config.baseMaxAliveTempC;

            bool inAliveRange = _tempC >= minAlive && _tempC <= maxAlive;
            if (inAliveRange)
            {
                _outOfAliveRangeTimer = 0f;
            }
            else
            {
                _outOfAliveRangeTimer += Time.deltaTime;

                if (_outOfAliveRangeTimer >= config.deathGraceSeconds)
                {
                    Die($"Temp out of alive range for {config.deathGraceSeconds:0.0}s. Temp={_tempC:0.0}C Range=[{minAlive:0.0},{maxAlive:0.0}]");
                    return;
                }
            }

            PublishTemperature();
        }

        public void ApplyInstantCooling(float degreesC)
        {
            if (!_isAlive)
                return;

            _tempC -= Mathf.Abs(degreesC);
            Debug.Log($"[Cactus] Instant cooling applied: -{Mathf.Abs(degreesC):0.0}C -> {_tempC:0.0}C");
            PublishTemperature();
        }

        private void Die(string reason)
        {
            _isAlive = false;
            Debug.LogError($"[Cactus] DIED. Reason: {reason}");
            GameEvents.RaiseCactusDied();
        }

        private void OnWindowStateChanged(WindowState state, bool _)
        {
            _windowState = state;
        }

        private void OnAcPurchasedChanged(bool purchased)
        {
            _acPurchased = purchased;
        }

        private void PublishTemperature()
        {
            GameEvents.RaiseTemperatureChanged(_tempC);
        }
    }
}
