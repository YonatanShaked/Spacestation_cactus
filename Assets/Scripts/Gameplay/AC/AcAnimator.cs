using System.Collections;
using Core.Events;
using Gameplay.Config;
using UnityEngine;

namespace Gameplay.AC
{
    public sealed class AcAnimator : MonoBehaviour
    {
        [SerializeField] private GameConfig config;
        [SerializeField] private Animation fanAnimation;
        [SerializeField] private GameObject pulseEffectObject;

        private Coroutine _pulseRoutine;
        private bool _pulsePlaying;

        private void Awake()
        {
            if (config == null)
            {
                Debug.LogError($"{nameof(AcAnimator)} missing GameConfig reference.", this);
                enabled = false;
                return;
            }

            if (fanAnimation == null)
            {
                Debug.LogError($"{nameof(AcAnimator)} missing fanAnimation reference.", this);
                enabled = false;
                return;
            }

            if (pulseEffectObject == null)
            {
                Debug.LogError($"{nameof(AcAnimator)} missing pulseEffectObject reference.", this);
                enabled = false;
                return;
            }

            // Start disabled until a pulse happens
            pulseEffectObject.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.AcPurchasedChanged += OnAcPurchasedChanged;
            GameEvents.AcPulseTriggered += OnAcPulseTriggered;
        }

        private void OnDisable()
        {
            GameEvents.AcPurchasedChanged -= OnAcPurchasedChanged;
            GameEvents.AcPulseTriggered -= OnAcPulseTriggered;

            if (_pulseRoutine != null)
            {
                StopCoroutine(_pulseRoutine);
                _pulseRoutine = null;
            }

            _pulsePlaying = false;

            if (pulseEffectObject != null)
                pulseEffectObject.SetActive(false);
        }

        private void OnAcPurchasedChanged(bool purchased)
        {
            if (purchased)
                fanAnimation.Play();
        }

        private void OnAcPulseTriggered()
        {
            // Extra guard: even if someone fires the event twice, don’t overlap pulses
            if (_pulsePlaying)
                return;

            if (_pulseRoutine != null)
            {
                StopCoroutine(_pulseRoutine);
                _pulseRoutine = null;
            }

            _pulseRoutine = StartCoroutine(PulseRoutine());
        }

        private IEnumerator PulseRoutine()
        {
            _pulsePlaying = true;

            pulseEffectObject.SetActive(true);

            float duration = Mathf.Max(0.05f, config.acPulseEffectDurationSeconds);
            yield return new WaitForSeconds(duration);

            pulseEffectObject.SetActive(false);

            _pulsePlaying = false;
            _pulseRoutine = null;
        }
    }
}
