#if UNITY_EDITOR
using Gameplay.Restart;
using Gameplay.Upgrades;
using Gameplay.Window;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Debugging
{
    public sealed class DebugHotkeys : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UpgradeSystem upgrades;
        [SerializeField] private WindowController window;
        [SerializeField] private AcPulseController pulse;
        [SerializeField] private GameRestartController restart;

        private Keyboard _keyboard;

        private void Awake()
        {
            _keyboard = Keyboard.current;
        }

        private void Update()
        {
            if (_keyboard.digit1Key.wasPressedThisFrame)
                upgrades.BuyAutoWindowUpgrade();

            if (_keyboard.digit2Key.wasPressedThisFrame)
                upgrades.BuyAc();

            if (_keyboard.digit3Key.wasPressedThisFrame)
                upgrades.BuyAcPulseUpgrade();

            if (_keyboard.oKey.wasPressedThisFrame)
                window.ManualOpen();

            if (_keyboard.cKey.wasPressedThisFrame)
                window.ManualClose();

            if (_keyboard.pKey.wasPressedThisFrame)
                pulse.TryPulse();

            if (_keyboard.rKey.wasPressedThisFrame)
                restart.Restart();
        }
    }
}
#endif
