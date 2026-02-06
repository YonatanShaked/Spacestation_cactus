using Core.Events;
using Gameplay.Config;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay.Window
{
    /// <summary>
    /// Controls window gameplay behavior and state transitions.
    /// Handles automatic opening at sunrise with failure chance, manual player interaction, and forced closing during eclipse.
    /// Reacts to orbit and upgrade events and publishes window state changes to the rest of the game.
    /// </summary>
    public sealed class WindowController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameConfig config;

        private WindowState _state = WindowState.Closed;
        private OrbitPhase _orbitPhase = OrbitPhase.Sunlit;

        private bool _autoFailedThisDawn;

        private float _autoOpenFailChance;

        private void Awake()
        {
            if (config == null)
            {
                Debug.LogError($"{nameof(WindowController)} missing config.", this);
                enabled = false;
                return;
            }

            // Fallback (in case UpgradeSystem hasn't published yet)
            _autoOpenFailChance = config.baseAutoOpenFailChance;
        }

        private void OnEnable()
        {
            GameEvents.OrbitPhaseChanged += OnOrbitPhaseChanged;
            GameEvents.AutoOpenFailChanceChanged += OnAutoOpenFailChanceChanged;
        }

        private void OnDisable()
        {
            GameEvents.OrbitPhaseChanged -= OnOrbitPhaseChanged;
            GameEvents.AutoOpenFailChanceChanged -= OnAutoOpenFailChanceChanged;
        }

        private void Start()
        {
            Publish();
        }

        private void OnAutoOpenFailChanceChanged(float chance)
        {
            _autoOpenFailChance = Mathf.Clamp01(chance);
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

        private float GetAutoOpenFailChance() => _autoOpenFailChance;

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
        }
    }
}
