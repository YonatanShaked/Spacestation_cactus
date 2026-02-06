using Core.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Lighting
{
    public enum LightState
    {
        Eclipse,
        SunlitWindowOpen,
        SunlitWindowClosed
    }

    public sealed class OrbitLightStateController : MonoBehaviour
    {
        [Header("Orbit State Light Events")]
        [SerializeField] private UnityEvent onEclipse;
        [SerializeField] private UnityEvent onSunlitWindowOpen;
        [SerializeField] private UnityEvent onSunlitWindowClosed;

        private OrbitPhase _orbitPhase = OrbitPhase.Sunlit;
        private WindowState _windowState = WindowState.Closed;

        private LightState _currentState;

        private void OnEnable()
        {
            GameEvents.OrbitPhaseChanged += OnOrbitPhaseChanged;
            GameEvents.WindowStateChanged += OnWindowStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.OrbitPhaseChanged -= OnOrbitPhaseChanged;
            GameEvents.WindowStateChanged -= OnWindowStateChanged;
        }

        private void Start()
        {
            EvaluateAndApplyState(force: true);
        }

        private void OnOrbitPhaseChanged(OrbitPhase phase)
        {
            _orbitPhase = phase;
            EvaluateAndApplyState(force: false);
        }

        private void OnWindowStateChanged(WindowState state, bool _)
        {
            _windowState = state;
            EvaluateAndApplyState(force: false);
        }

        private void EvaluateAndApplyState(bool force)
        {
            LightState next = ComputeState(_orbitPhase, _windowState);

            if (!force && next == _currentState)
                return;

            _currentState = next;

            switch (_currentState)
            {
                case LightState.Eclipse:
                    onEclipse?.Invoke();
                    break;

                case LightState.SunlitWindowOpen:
                    onSunlitWindowOpen?.Invoke();
                    break;

                case LightState.SunlitWindowClosed:
                    onSunlitWindowClosed?.Invoke();
                    break;
            }
        }

        private static LightState ComputeState(OrbitPhase orbit, WindowState window)
        {
            if (orbit == OrbitPhase.Eclipse)
                return LightState.Eclipse;

            return window == WindowState.Open
                ? LightState.SunlitWindowOpen
                : LightState.SunlitWindowClosed;
        }
    }
}
