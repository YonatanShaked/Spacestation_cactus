#if UNITY_EDITOR || DEVELOPMENT_BUILD
using Core.Events;
using UnityEngine;

namespace Debugging
{
    /// <summary>
    /// Debug-only event logger for observing gameplay events in the Unity console.
    /// Subscribes to core game events and prints their payloads to aid development and debugging.
    /// Compiled only in editor and development builds to avoid impacting production performance.
    /// </summary>
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
