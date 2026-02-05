using Core.Events;
using UnityEngine;

namespace UI
{
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
