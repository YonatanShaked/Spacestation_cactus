using Core.Events;
using Gameplay.Config;
using UnityEngine;

namespace Gameplay.Upgrades
{
    /// <summary>
    /// Manages all player upgrade logic and progression.
    /// Handles purchase validation, upgrade limits, and derived gameplay effects such as auto-window fail chance and AC availability.
    /// Publishes upgrade state changes via events to keep gameplay systems decoupled.
    /// </summary>
    public sealed class UpgradeSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameConfig config;

        private int _autoWindowLevel;
        private bool _acPurchased;
        private bool _acPulsePurchased;
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
            _autoWindowLevel = Mathf.Clamp(_autoWindowLevel, 0, _maxAutoWindowLevel);
        }

        private void Start()
        {
            PublishAll();
        }

        public void BuyAutoWindowUpgrade()
        {
            if (_autoWindowLevel >= _maxAutoWindowLevel)
            {
                Debug.Log($"[Upgrades] Auto Window already at max. Level={_autoWindowLevel}, FailChance={ComputeAutoOpenFailChance():P0}");
                return;
            }

            _autoWindowLevel++;
            Debug.Log($"[Upgrades] Auto Window upgrade purchased. Level={_autoWindowLevel}/{_maxAutoWindowLevel}, FailChance={ComputeAutoOpenFailChance():P0}");

            PublishAutoOpenFailChance();
        }

        public void BuyAc()
        {
            if (_acPurchased)
            {
                Debug.Log("[Upgrades] AC already purchased.");
                return;
            }

            _acPurchased = true;
            Debug.Log("[Upgrades] AC purchased -> max alive temp becomes 115C.");
            GameEvents.RaiseAcPurchasedChanged(true);
        }

        public void BuyAcPulseUpgrade()
        {
            if (!_acPurchased)
            {
                Debug.Log("[Upgrades] Cannot buy AC Pulse without buying AC first.");
                return;
            }

            if (_acPulsePurchased)
            {
                Debug.Log("[Upgrades] AC Pulse already purchased.");
                return;
            }

            _acPulsePurchased = true;
            Debug.Log("[Upgrades] AC Pulse upgrade purchased.");

            GameEvents.RaiseAcPulsePurchasedChanged(true);
            GameEvents.RaiseAcPulseReadyChanged(true);
        }

        public float ComputeAutoOpenFailChance()
        {
            float reduced = config.baseAutoOpenFailChance - (_autoWindowLevel * config.autoOpenFailChanceReductionPerUpgrade);
            return Mathf.Max(config.minAutoOpenFailChance, reduced);
        }

        public int AutoWindowLevel => _autoWindowLevel;
        public int MaxAutoWindowLevel => _maxAutoWindowLevel;
        public bool AcPurchased => _acPurchased;
        public bool AcPulsePurchased => _acPulsePurchased;

        private int CalculateMaxAutoWindowLevel()
        {
            float baseFail = config.baseAutoOpenFailChance;
            float minFail = config.minAutoOpenFailChance;
            float step = config.autoOpenFailChanceReductionPerUpgrade;

            if (step <= 0f)
                return 0;

            float neededReduction = Mathf.Max(0f, baseFail - minFail);
            return Mathf.Max(0, Mathf.CeilToInt(neededReduction / step));
        }

        private void PublishAutoOpenFailChance()
        {
            GameEvents.RaiseAutoOpenFailChanceChanged(ComputeAutoOpenFailChance());
        }

        private void PublishAll()
        {
            PublishAutoOpenFailChance();
            GameEvents.RaiseAcPurchasedChanged(_acPurchased);
            GameEvents.RaiseAcPulsePurchasedChanged(_acPulsePurchased);
        }
    }
}
