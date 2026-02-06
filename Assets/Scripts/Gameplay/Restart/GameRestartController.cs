using Core.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.Restart
{
    /// <summary>
    /// Handles full game restart by resetting all static game events
    /// and reloading the active scene to ensure a clean gameplay state.
    /// </summary>
    public sealed class GameRestartController : MonoBehaviour
    {
        public void Restart()
        {
            GameEvents.ResetAll();
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.buildIndex);
        }
    }
}
