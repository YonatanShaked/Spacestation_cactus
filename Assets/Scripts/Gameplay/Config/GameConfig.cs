using UnityEngine;

namespace Gameplay.Config
{
    /// <summary>
    /// Central configuration asset for gameplay tuning and balancing.
    /// Holds all designer-editable values for orbit timing, window logic, temperature, scoring, upgrades, and visual thresholds.
    /// Shared read-only across systems to avoid hardcoded values and enable rapid iteration without code changes.
    /// </summary>
    [CreateAssetMenu(menuName = "Config/GameConfig", fileName = "GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Orbit")]
        [Min(1f)] public float fullCycleSeconds = 90f;
        [Min(1f)] public float sunlitSeconds = 60f;
        [Min(1f)] public float eclipseSeconds = 30f;

        [Header("Window Auto-Open")]
        [Range(0f, 1f)] public float baseAutoOpenFailChance = 0.25f;
        [Range(0f, 1f)] public float minAutoOpenFailChance = 0.10f;
        [Range(0f, 1f)] public float autoOpenFailChanceReductionPerUpgrade = 0.05f;

        [Header("Temperature")]
        public float startingTempC = 22f;
        public float tempIncreasePerSecondWhenOpen = 1f;
        public float tempDecreasePerSecondWhenClosed = 2f;

        public float baseMinAliveTempC = 0f;
        public float baseMaxAliveTempC = 80f;
        public float acMaxAliveTempC = 115f;
        [Min(0.1f)] public float deathGraceSeconds = 3f;

        [Header("Scoring")]
        [Min(0f)] public float pointsPerSecond = 1f;
        public float safeMinTempC = 10f;
        public float safeMaxTempC = 60f;
        [Min(1f)] public float secondsPerMultiplierStep = 30f;
        [Range(1, 4)] public int maxMultiplier = 4;

        [Header("AC Pulse")]
        public int acPulseCostPoints = 20;
        public float acPulseTempDropC = 10f;
        [Min(0.5f)] public float acPulseEffectDurationSeconds = 1.5f;

        [Header("Cactus Visual States")]
        public int flowerScoreThreshold = 100;
        public float thornsTempThreshold = 80f;
    }
}
