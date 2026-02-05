using Core.Events;
using Gameplay.Config;
using UnityEngine;

namespace Gameplay.Orbit
{
    public sealed class OrbitController : MonoBehaviour
    {
        [SerializeField] private GameConfig config;

        private float _timer;
        private OrbitPhase _phase;

        private void Awake()
        {
            if (config == null)
            {
                Debug.LogError($"{nameof(OrbitController)} missing config.", this);
                enabled = false;
            }
        }

        private void Start()
        {
            // Start in Sunlit by default (you can choose either)
            SetPhase(OrbitPhase.Sunlit);
            _timer = 0f;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            float phaseDuration = _phase == OrbitPhase.Sunlit ? config.sunlitSeconds : config.eclipseSeconds;
            if (_timer >= phaseDuration)
            {
                _timer = 0f;
                SetPhase(_phase == OrbitPhase.Sunlit ? OrbitPhase.Eclipse : OrbitPhase.Sunlit);
            }
        }

        private void SetPhase(OrbitPhase newPhase)
        {
            _phase = newPhase;
            GameEvents.RaiseOrbitPhaseChanged(_phase);
            Debug.Log($"[Orbit] Phase -> {_phase}");
        }
    }
}
