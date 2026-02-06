using Core.Events;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Updates on-screen HUD text elements in response to gameplay events.
    /// Listens to core game, scoring, and upgrade events and reflects the current game state using TextMeshPro UI elements.
    /// Keeps UI presentation fully decoupled from gameplay logic through event-driven updates.
    /// </summary>
    public sealed class InfoTextEventHandler : MonoBehaviour
    {
        [Header("Core HUD")]
        [SerializeField] private TextMeshProUGUI temperatureText;
        [SerializeField] private TextMeshProUGUI orbitText;
        [SerializeField] private TextMeshProUGUI windowText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI multiplierText;
        [SerializeField] private TextMeshProUGUI tempStreakText;

        [Header("Upgrades")]
        [SerializeField] private TextMeshProUGUI autoOpenFailRateText;
        [SerializeField] private TextMeshProUGUI boughtACText;
        [SerializeField] private TextMeshProUGUI acPulseText;

        private bool _acPulsePurchased;
        private bool _acPulseReady;

        private void OnEnable()
        {
            GameEvents.TemperatureChanged += OnTemperatureChanged;
            GameEvents.OrbitPhaseChanged += OnOrbitPhaseChanged;
            GameEvents.WindowStateChanged += OnWindowStateChanged;
            GameEvents.ScoreChanged += OnScoreChanged;
            GameEvents.MultiplierChanged += OnMultiplierChanged;
            GameEvents.TempSafeStreakChanged += OnTempSafeStreakChanged;

            GameEvents.AutoOpenFailChanceChanged += OnAutoOpenFailChanceChanged;
            GameEvents.AcPurchasedChanged += OnAcPurchasedChanged;
            GameEvents.AcPulsePurchasedChanged += OnAcPulsePurchasedChanged;
            GameEvents.AcPulseReadyChanged += OnAcPulseReadyChanged;
        }

        private void OnDisable()
        {
            GameEvents.TemperatureChanged -= OnTemperatureChanged;
            GameEvents.OrbitPhaseChanged -= OnOrbitPhaseChanged;
            GameEvents.WindowStateChanged -= OnWindowStateChanged;
            GameEvents.ScoreChanged -= OnScoreChanged;
            GameEvents.MultiplierChanged -= OnMultiplierChanged;
            GameEvents.TempSafeStreakChanged -= OnTempSafeStreakChanged;

            GameEvents.AutoOpenFailChanceChanged -= OnAutoOpenFailChanceChanged;
            GameEvents.AcPurchasedChanged -= OnAcPurchasedChanged;
            GameEvents.AcPulsePurchasedChanged -= OnAcPulsePurchasedChanged;
            GameEvents.AcPulseReadyChanged -= OnAcPulseReadyChanged;
        }

        private void OnTemperatureChanged(float temp) =>
            temperatureText.text = $"Temperature: {temp:0.0}°C";

        private void OnOrbitPhaseChanged(OrbitPhase phase) =>
            orbitText.text = $"Orbit: {phase}";

        private void OnWindowStateChanged(WindowState state, bool failedAutoOpen) =>
            windowText.text = $"Window: {state} | Failed Open-Auto: {(failedAutoOpen ? "Yes" : "No")}";

        private void OnScoreChanged(int score) =>
            scoreText.text = $"Score: {score}";

        private void OnMultiplierChanged(int multi) =>
            multiplierText.text = $"Multiplier: x{multi}";

        private void OnTempSafeStreakChanged(float seconds) =>
            tempStreakText.text = $"Streak Temp: {FormatAsMinutesSeconds(seconds)}";

        private void OnAutoOpenFailChanceChanged(float chance) =>
            autoOpenFailRateText.text = $"Auto-Open Fail Chance: {chance:P0}";

        private void OnAcPurchasedChanged(bool purchased) =>
            boughtACText.text = $"AC: {(purchased ? "Bought" : "Not Bought")}";

        private void OnAcPulsePurchasedChanged(bool purchased)
        {
            _acPulsePurchased = purchased;
            RefreshAcPulseText();
        }

        private void OnAcPulseReadyChanged(bool ready)
        {
            _acPulseReady = ready;
            RefreshAcPulseText();
        }

        private void RefreshAcPulseText()
        {
            if (!_acPulsePurchased)
            {
                acPulseText.text = "AC Pulse: Not Bought";
                return;
            }

            acPulseText.text = $"AC Pulse: {(_acPulseReady ? "Ready" : "Used")}";
        }

        private static string FormatAsMinutesSeconds(float seconds)
        {
            if (seconds < 0f) seconds = 0f;
            int total = Mathf.FloorToInt(seconds);
            int mm = total / 60;
            int ss = total % 60;
            return $"{mm:00}:{ss:00}";
        }
    }
}
