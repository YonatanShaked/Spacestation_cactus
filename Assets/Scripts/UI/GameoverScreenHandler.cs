using Core.Events;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Controls the game-over UI screen.
    /// Listens for the cactus death event and activates the game-over screen when the game ends.
    /// Keeps UI logic isolated from gameplay systems.
    /// </summary>
    public sealed class GameoverScreenHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject gameoverScreen;

        private void Awake()
        {
            gameoverScreen.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.CactusDied += OnCactusDied;
        }

        private void OnDisable()
        {
            GameEvents.CactusDied -= OnCactusDied;
        }

        private void OnCactusDied()
        {
            gameoverScreen.SetActive(true);
        }
    }
}
