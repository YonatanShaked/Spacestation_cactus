using Core.Events;
using Gameplay.Config;
using Gameplay.Scoring;
using Gameplay.Cactus;
using UnityEngine;

namespace Gameplay.Upgrades
{
    public sealed class AcPulseController : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private ScoreSystem score;
        [SerializeField] private CactusThermodynamics cactus;

        private OrbitPhase _orbitPhase = OrbitPhase.Sunlit;

        private bool _acPulsePurchased;
        private bool _pulseReady;

        private void Awake()
        {
            if (config == null || score == null || cactus == null)
            {
                Debug.LogError($"{nameof(AcPulseController)} missing references.", this);
                enabled = false;
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
            // If purchased at runtime, Start will get state via event; until then, keep false.
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
                Debug.Log("[AC Pulse] Pulse blocked (not purchased / not ready / cactus dead).");
                return;
            }

            // Costs 20 points (spec).
            if (!score.TrySpend(config.acPulseCostPoints))
            {
                Debug.Log($"[AC Pulse] Not enough points. Need {config.acPulseCostPoints}, have {score.Score}.");
                return;
            }

            cactus.ApplyInstantCooling(config.acPulseTempDropC);

            _pulseReady = false;
            Debug.Log("[AC Pulse] USED -> now Used until next Sunlit.");
            Publish();

            GameEvents.RaiseAcPulseTriggered();
        }

        private void OnOrbitPhaseChanged(OrbitPhase phase)
        {
            _orbitPhase = phase;

            // Resets at start of next Sunlit (spec).
            if (phase == OrbitPhase.Sunlit && _acPulsePurchased)
            {
                _pulseReady = true;
                Debug.Log("[AC Pulse] Reset -> Ready (new Sunlit).");
                Publish();
            }
        }

        private void OnAcPulsePurchasedChanged(bool purchased)
        {
            _acPulsePurchased = purchased;
            if (_acPulsePurchased)
            {
                // Make it available immediately; it will also reset each Sunlit.
                _pulseReady = true;
            }
            Publish();
        }

        private void OnCactusDied()
        {
            // Keep state; just can't use while dead.
            Publish();
        }

        private void Publish()
        {
            GameEvents.RaiseAcPulseReadyChanged(_pulseReady);
        }
    }
}
