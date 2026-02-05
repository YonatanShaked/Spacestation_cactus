using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.Restart
{
    public sealed class GameRestartController : MonoBehaviour
    {
        public void Restart()
        {
            var scene = SceneManager.GetActiveScene();
            Debug.Log("[Restart] Reloading scene: " + scene.name);
            SceneManager.LoadScene(scene.buildIndex);
        }
    }
}
