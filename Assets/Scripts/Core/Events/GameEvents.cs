using System;

namespace Core.Events
{
    public enum OrbitPhase { Sunlit, Eclipse }
    public enum WindowState { Open, Closed }

    public static class GameEvents
    {
        // Orbit
        public static event Action<OrbitPhase> OrbitPhaseChanged;

        // Window
        public static event Action<WindowState, bool /*autoFailedThisDawn*/> WindowStateChanged;

        // Cactus temperature / life
        public static event Action<float> TemperatureChanged;
        public static event Action CactusDied;

        // Score & multiplier
        public static event Action<int> ScoreChanged;
        public static event Action<int> MultiplierChanged;

        // Temperature streak
        public static event Action<float> TempSafeStreakChanged; // seconds

        // Upgrades
        public static event Action<float> AutoOpenFailChanceChanged;
        public static event Action<bool> AcPurchasedChanged;
        public static event Action<bool> AcPulsePurchasedChanged;
        public static event Action<bool> AcPulseReadyChanged;
        public static event Action AcPulseTriggered;

        public static void RaiseOrbitPhaseChanged(OrbitPhase phase) => OrbitPhaseChanged?.Invoke(phase);
        public static void RaiseWindowStateChanged(WindowState state, bool autoFailedThisDawn) => WindowStateChanged?.Invoke(state, autoFailedThisDawn);
        public static void RaiseTemperatureChanged(float tempC) => TemperatureChanged?.Invoke(tempC);
        public static void RaiseCactusDied() => CactusDied?.Invoke();

        public static void RaiseScoreChanged(int score) => ScoreChanged?.Invoke(score);
        public static void RaiseMultiplierChanged(int multiplier) => MultiplierChanged?.Invoke(multiplier);
        public static void RaiseTempSafeStreakChanged(float seconds) => TempSafeStreakChanged?.Invoke(seconds);
        public static void RaiseAutoOpenFailChanceChanged(float chance) => AutoOpenFailChanceChanged?.Invoke(chance);
        public static void RaiseAcPurchasedChanged(bool purchased) => AcPurchasedChanged?.Invoke(purchased);
        public static void RaiseAcPulsePurchasedChanged(bool purchased) => AcPulsePurchasedChanged?.Invoke(purchased);
        public static void RaiseAcPulseReadyChanged(bool ready) => AcPulseReadyChanged?.Invoke(ready);
        public static void RaiseAcPulseTriggered() => AcPulseTriggered?.Invoke();
    }
}
