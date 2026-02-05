using Core.Events;
using Gameplay.Config;
using UnityEngine;

namespace Gameplay.Upgrades
{
    public sealed class UpgradeSystem : MonoBehaviour
    {
        [SerializeField] private GameConfig config;

        [Header("Runtime State (Read-only)")]
        [SerializeField] private int autoWindowLevel;
        [SerializeField] private bool acPurchased;
        [SerializeField] private bool acPulsePurchased;

        private int _maxAutoWindowLevel;

        private void Awake()
        {
            if (config == null)
            {
                Debug.LogError($"{nameof(UpgradeSystem)} missing config.", this);
                enabled = false;
                return;
            }

            _maxAutoWindowLevel = CalculateMaxAutoWindowLevel();
            autoWindowLevel = Mathf.Clamp(autoWindowLevel, 0, _maxAutoWindowLevel);
        }

        private void Start()
        {
            PublishAll();
        }

        public void BuyAutoWindowUpgrade()
        {
            if (autoWindowLevel >= _maxAutoWindowLevel)
            {
                Debug.Log($"[Upgrades] Auto Window already at max. Level={autoWindowLevel}, FailChance={ComputeAutoOpenFailChance():P0}");
                return;
            }

            autoWindowLevel++;
            float chance = ComputeAutoOpenFailChance();

            Debug.Log($"[Upgrades] Auto Window upgrade purchased. Level={autoWindowLevel}/{_maxAutoWindowLevel}, FailChance={chance:P0}");
            GameEvents.RaiseAutoOpenFailChanceChanged(chance);
        }

        public void BuyAc()
        {
            if (acPurchased)
            {
                Debug.Log("[Upgrades] AC already purchased.");
                return;
            }

            acPurchased = true;
            Debug.Log("[Upgrades] AC purchased -> max alive temp becomes 115C.");
            GameEvents.RaiseAcPurchasedChanged(true);
        }

        public void BuyAcPulseUpgrade()
        {
            if (!acPurchased)
            {
                Debug.Log("[Upgrades] Cannot buy AC Pulse without buying AC first.");
                return;
            }

            if (acPulsePurchased)
            {
                Debug.Log("[Upgrades] AC Pulse already purchased.");
                return;
            }

            acPulsePurchased = true;
            Debug.Log("[Upgrades] AC Pulse upgrade purchased.");
            GameEvents.RaiseAcPulsePurchasedChanged(true);

            // Optional: let AcPulseController decide readiness; harmless for now.
            GameEvents.RaiseAcPulseReadyChanged(true);
        }

        public float ComputeAutoOpenFailChance()
        {
            float reduced = config.baseAutoOpenFailChance - (autoWindowLevel * config.autoOpenFailChanceReductionPerUpgrade);
            return Mathf.Max(config.minAutoOpenFailChance, reduced);
        }

        public int AutoWindowLevel => autoWindowLevel;
        public int MaxAutoWindowLevel => _maxAutoWindowLevel;

        public bool AcPurchased => acPurchased;
        public bool AcPulsePurchased => acPulsePurchased;

        private int CalculateMaxAutoWindowLevel()
        {
            float baseFail = config.baseAutoOpenFailChance;
            float minFail = config.minAutoOpenFailChance;
            float step = config.autoOpenFailChanceReductionPerUpgrade;

            if (step <= 0f)
                return 0;

            float neededReduction = Mathf.Max(0f, baseFail - minFail);
            int levels = Mathf.CeilToInt(neededReduction / step);

            return Mathf.Max(0, levels);
        }

        private void PublishAll()
        {
            GameEvents.RaiseAutoOpenFailChanceChanged(ComputeAutoOpenFailChance());
            GameEvents.RaiseAcPurchasedChanged(acPurchased);
            GameEvents.RaiseAcPulsePurchasedChanged(acPulsePurchased);
        }
    }
}
