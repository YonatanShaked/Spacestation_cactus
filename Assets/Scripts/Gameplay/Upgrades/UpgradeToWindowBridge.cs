using Gameplay.Window;
using UnityEngine;

namespace Gameplay.Upgrades
{
    public sealed class UpgradeToWindowBridge : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UpgradeSystem upgrades;
        [SerializeField] private WindowController window;

        private void Awake()
        {
            if (upgrades == null || window == null)
            {
                Debug.LogError($"{nameof(UpgradeToWindowBridge)} missing references.", this);
                enabled = false;
                return;
            }
        }

        private void Start()
        {
            window.SetAutoWindowUpgradeLevel(upgrades.AutoWindowLevel);
        }

        public void SyncAutoWindowLevel()
        {
            window.SetAutoWindowUpgradeLevel(upgrades.AutoWindowLevel);
        }
    }
}
