using Core.Events;
using Gameplay.Config;
using Gameplay.Scoring;
using Gameplay.Cactus;
using UnityEngine;

namespace Gameplay.Upgrades
{
    public sealed class AcPulseController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameConfig config;
        [SerializeField] private ScoreSystem score;
        [SerializeField] private CactusThermodynamics cactus;

        private bool _acPulsePurchased;
        private bool _pulseReady;

        private void Awake()
        {
            if (config == null || score == null || cactus == null)
            {
                Debug.LogError($"{nameof(AcPulseController)} missing references.", this);
                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            GameEvents.OrbitPhaseChanged += OnOrbitPhaseChanged;
            GameEvents.AcPulsePurchasedChanged += OnAcPulsePurchasedChanged;
            GameEvents.CactusDied += OnCactusDied;
        }

        private void OnDisable()
        {
            GameEvents.OrbitPhaseChanged -= OnOrbitPhaseChanged;
            GameEvents.AcPulsePurchasedChanged -= OnAcPulsePurchasedChanged;
            GameEvents.CactusDied -= OnCactusDied;
        }

        private void Start()
        {
            Publish();
        }

        public bool CanPulse()
        {
            return _acPulsePurchased && _pulseReady && cactus.IsAlive;
        }

        public void TryPulse()
        {
            if (!CanPulse())
            {
                Debug.Log("[AC Pulse] Pulse blocked.");
                return;
            }

            if (!score.TrySpend(config.acPulseCostPoints))
            {
                Debug.Log($"[AC Pulse] Not enough points. Need {config.acPulseCostPoints}, have {score.Score}.");
                return;
            }

            cactus.ApplyInstantCooling(config.acPulseTempDropC);

            _pulseReady = false;
            Debug.Log("[AC Pulse] Now Used until next Sunlit.");
            Publish();

            GameEvents.RaiseAcPulseTriggered();
        }

        private void OnOrbitPhaseChanged(OrbitPhase phase)
        {
            if (phase == OrbitPhase.Sunlit && _acPulsePurchased)
            {
                _pulseReady = true;
                Debug.Log("[AC Pulse] Ready (new Sunlit).");
                Publish();
            }
        }

        private void OnAcPulsePurchasedChanged(bool purchased)
        {
            _acPulsePurchased = purchased;
            if (_acPulsePurchased)
                _pulseReady = true;

            Publish();
        }

        private void OnCactusDied()
        {
            Publish();
        }

        private void Publish()
        {
            GameEvents.RaiseAcPulseReadyChanged(_pulseReady);
        }
    }
}
