#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Core.Events;
using UnityEngine;

namespace Debugging
{
    public sealed class DebugConsoleLogger : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEvents.OrbitPhaseChanged += phase => Debug.Log($"[Event] OrbitPhaseChanged: {phase}");
            GameEvents.WindowStateChanged += (state, failed) => Debug.Log($"[Event] WindowStateChanged: {state}, AutoFailedThisDawn={failed}");
            GameEvents.CactusDied += () => Debug.Log("[Event] CactusDied");
            GameEvents.ScoreChanged += s => Debug.Log($"[Event] ScoreChanged: {s}");
            GameEvents.MultiplierChanged += m => Debug.Log($"[Event] MultiplierChanged: x{m}");
        }
    }
}
#endif
