using Core.Events;
using Gameplay.Config;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Window
{
    public sealed class WindowController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameConfig config;

        private WindowState _state = WindowState.Closed;
        private OrbitPhase _orbitPhase = OrbitPhase.Sunlit;

        private int _autoWindowUpgradeLevel;
        private bool _autoFailedThisDawn;

        private void OnEnable()
        {
            GameEvents.OrbitPhaseChanged += OnOrbitPhaseChanged;
        }

        private void OnDisable()
        {
            GameEvents.OrbitPhaseChanged -= OnOrbitPhaseChanged;
        }

        private void Awake()
        {
            if (config == null)
            {
                Debug.LogError($"{nameof(WindowController)} missing config.", this);
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            Publish();
        }

        private void OnOrbitPhaseChanged(OrbitPhase phase)
        {
            _orbitPhase = phase;

            if (phase == OrbitPhase.Sunlit)
                TryAutoOpenAtDawn();
            else
                CloseWindowInternal(autoFailedThisDawn: false);
        }

        private void TryAutoOpenAtDawn()
        {
            _autoFailedThisDawn = false;

            float failChance = GetAutoOpenFailChance();
            bool failed = Random.value < failChance;

            if (failed)
            {
                _autoFailedThisDawn = true;
                Debug.Log($"[Window] Auto-open FAILED at dawn. FailChance={failChance:P0}");
                Publish();
                return;
            }

            OpenWindowInternal(autoFailedThisDawn: false);
            Debug.Log($"[Window] Auto-open SUCCESS at dawn. FailChance={failChance:P0}");
        }

        private float GetAutoOpenFailChance()
        {
            float reduced = config.baseAutoOpenFailChance - (_autoWindowUpgradeLevel * config.autoOpenFailChanceReductionPerUpgrade);
            return Mathf.Max(config.minAutoOpenFailChance, reduced);
        }

        public bool CanManuallyOpen()
            => (_orbitPhase == OrbitPhase.Sunlit) && (_state == WindowState.Closed) && _autoFailedThisDawn;

        public bool CanClose()
            => _state == WindowState.Open;

        public void ManualOpen()
        {
            if (!CanManuallyOpen())
            {
                Debug.Log("[Window] ManualOpen blocked (conditions not met).");
                return;
            }

            OpenWindowInternal(autoFailedThisDawn: _autoFailedThisDawn);
            Debug.Log("[Window] Manually opened.");
        }

        public void ManualClose()
        {
            if (!CanClose())
            {
                Debug.Log("[Window] ManualClose blocked (already closed).");
                return;
            }

            CloseWindowInternal(autoFailedThisDawn: false);
            Debug.Log("[Window] Manually closed.");
        }

        public void SetAutoWindowUpgradeLevel(int level)
        {
            _autoWindowUpgradeLevel = Mathf.Max(0, level);
            float chance = GetAutoOpenFailChance();
            GameEvents.RaiseAutoOpenFailChanceChanged(chance);
        }

        private void OpenWindowInternal(bool autoFailedThisDawn)
        {
            _state = WindowState.Open;
            GameEvents.RaiseWindowStateChanged(_state, autoFailedThisDawn);
        }

        private void CloseWindowInternal(bool autoFailedThisDawn)
        {
            _state = WindowState.Closed;
            GameEvents.RaiseWindowStateChanged(_state, autoFailedThisDawn);
        }

        private void Publish()
        {
            GameEvents.RaiseWindowStateChanged(_state, _autoFailedThisDawn);
            GameEvents.RaiseAutoOpenFailChanceChanged(GetAutoOpenFailChance());
        }
    }
}
