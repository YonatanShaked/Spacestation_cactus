using Core.Events;
using Gameplay.Config;
using UnityEngine;

namespace Gameplay.Scoring
{
    /// <summary>
    /// Tracks player score, multiplier, and safe-temperature streak progression.
    /// Awards points only while Sunlit, window open, and cactus alive, and increases multiplier by doubling after sustained safe temperature intervals.
    /// Publishes score, multiplier, and streak updates via events and supports spending score for upgrades.
    /// </summary>
    public sealed class ScoreSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameConfig config;

        private int _score;
        private float _scoreRemainder;
        private int _multiplier = 1;
        private int _lastPublishedStreakWholeSeconds = -1;

        private OrbitPhase _orbitPhase = OrbitPhase.Sunlit;
        private WindowState _windowState = WindowState.Closed;
        private bool _isAlive = true;

        private float _temperatureC;
        private float _safeStreakSeconds;

        public int Score => _score;


        private void Awake()
        {
            if (config == null)
            {
                Debug.LogError($"{nameof(ScoreSystem)} missing config.", this);
                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            GameEvents.OrbitPhaseChanged += OnOrbitPhaseChanged;
            GameEvents.WindowStateChanged += OnWindowStateChanged;
            GameEvents.TemperatureChanged += OnTemperatureChanged;
            GameEvents.CactusDied += OnCactusDied;
        }

        private void OnDisable()
        {
            GameEvents.OrbitPhaseChanged -= OnOrbitPhaseChanged;
            GameEvents.WindowStateChanged -= OnWindowStateChanged;
            GameEvents.TemperatureChanged -= OnTemperatureChanged;
            GameEvents.CactusDied -= OnCactusDied;
        }

        private void Start()
        {
            PublishAll();
        }

        private void Update()
        {
            UpdateMultiplierStreak(Time.deltaTime);

            if (ShouldAccumulateScore())
            {
                float pointsThisFrame = config.pointsPerSecond * _multiplier * Time.deltaTime;
                AddScore(pointsThisFrame);
            }
        }

        public bool TrySpend(int cost)
        {
            if (cost <= 0)
                return true;

            if (_score < cost)
                return false;

            _score -= cost;
            _scoreRemainder = 0f;
            GameEvents.RaiseScoreChanged(_score);
            return true;
        }

        private bool ShouldAccumulateScore()
        {
            bool sunlit = _orbitPhase == OrbitPhase.Sunlit;
            bool windowOpen = _windowState == WindowState.Open;
            return sunlit && windowOpen && _isAlive;
        }

        private void UpdateMultiplierStreak(float dt)
        {
            if (!_isAlive)
                return;

            bool inSafeRange = (_temperatureC >= config.safeMinTempC) && (_temperatureC <= config.safeMaxTempC);

            if (!inSafeRange)
            {
                if (_safeStreakSeconds > 0f || _multiplier != 1)
                    Debug.Log("[Score] Safe streak broken -> reset multiplier to x1.");

                _safeStreakSeconds = 0f;
                _lastPublishedStreakWholeSeconds = -1; // force publish
                PublishStreakIfChanged();

                SetMultiplier(1);
                return;
            }

            _safeStreakSeconds += dt;
            PublishStreakIfChanged();

            while (_safeStreakSeconds >= config.secondsPerMultiplierStep && _multiplier < config.maxMultiplier)
            {
                _safeStreakSeconds -= config.secondsPerMultiplierStep;
                int newMultiplier = Mathf.Min(config.maxMultiplier, _multiplier * 2);
                SetMultiplier(newMultiplier);

                Debug.Log($"[Score] Multiplier increased -> x{_multiplier}");
            }

            PublishStreakIfChanged();
        }

        private void AddScore(float points)
        {
            float total = _scoreRemainder + points;
            int whole = Mathf.FloorToInt(total);

            if (whole > 0)
            {
                _score += whole;
                GameEvents.RaiseScoreChanged(_score);
            }

            _scoreRemainder = total - whole;
        }

        private void SetMultiplier(int newMultiplier)
        {
            newMultiplier = Mathf.Clamp(newMultiplier, 1, config.maxMultiplier);
            if (newMultiplier == _multiplier)
                return;

            _multiplier = newMultiplier;
            GameEvents.RaiseMultiplierChanged(_multiplier);
        }

        private void PublishStreakIfChanged()
        {
            int wholeSeconds = Mathf.FloorToInt(_safeStreakSeconds);
            if (wholeSeconds == _lastPublishedStreakWholeSeconds)
                return;

            _lastPublishedStreakWholeSeconds = wholeSeconds;
            GameEvents.RaiseTempSafeStreakChanged(_safeStreakSeconds);
        }

        private void OnCactusDied()
        {
            _isAlive = false;
            _safeStreakSeconds = 0f;
            _lastPublishedStreakWholeSeconds = -1;
            PublishStreakIfChanged();

            SetMultiplier(1);
        }

        private void PublishAll()
        {
            GameEvents.RaiseScoreChanged(_score);
            GameEvents.RaiseMultiplierChanged(_multiplier);

            _lastPublishedStreakWholeSeconds = -1;
            PublishStreakIfChanged();
        }

        private void OnOrbitPhaseChanged(OrbitPhase phase) => _orbitPhase = phase;

        private void OnWindowStateChanged(WindowState state, bool _) => _windowState = state;

        private void OnTemperatureChanged(float tempC) => _temperatureC = tempC;
    }
}
