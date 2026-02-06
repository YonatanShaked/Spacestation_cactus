using Core.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.Restart
{
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
