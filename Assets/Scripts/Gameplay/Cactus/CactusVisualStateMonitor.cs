using Core.Events;
using Gameplay.Config;
using UnityEngine;

namespace Gameplay.Cactus
{
    /// <summary>
    /// Monitors cactus visual states based on gameplay conditions.
    /// Activates flower and thorn visuals according to current score and temperature thresholds defined in the game configuration.
    /// Reacts to score and temperature events without direct coupling to gameplay logic.
    /// </summary>
    public sealed class CactusVisualStateMonitor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameConfig config;
        [SerializeField] private GameObject flower;
        [SerializeField] private GameObject thorns;

        private int _score;
        private float _temperatureC;

        private void Awake()
        {
            if (config == null || flower == null || thorns == null)
            {
                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            GameEvents.ScoreChanged += OnScoreChanged;
            GameEvents.TemperatureChanged += OnTemperatureChanged;
        }

        private void OnDisable()
        {
            GameEvents.ScoreChanged -= OnScoreChanged;
            GameEvents.TemperatureChanged -= OnTemperatureChanged;
        }

        private void OnScoreChanged(int score)
        {
            _score = score;
            EvaluateFlower();
        }

        private void OnTemperatureChanged(float tempC)
        {
            _temperatureC = tempC;
            EvaluateThorns();
        }

        private void EvaluateFlower()
        {
            bool shouldFlower = _score > config.flowerScoreThreshold;
            flower.SetActive(shouldFlower);
        }

        private void EvaluateThorns()
        {
            bool shouldHaveThorns = _temperatureC > config.thornsTempThreshold;
            thorns.SetActive(shouldHaveThorns);
        }
    }
}
