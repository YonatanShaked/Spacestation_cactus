using Core.Events;
using UnityEngine;

namespace Gameplay.Window
{
    /// <summary>
    /// Drives window opening and closing animations based on window state events.
    /// Listens to window state changes and plays the corresponding animator states while avoiding redundant animation triggers.
    /// Isolated from gameplay logic and depends only on animation state names.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public sealed class WindowAnimator : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private string openStateName = "Windows_Open";
        [SerializeField] private string closeStateName = "Windows_Close";

        private Animator _animator;
        private bool _windowsOpen;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            GameEvents.WindowStateChanged += OnWindowStateChanged;
        }

        private void OnDisable()
        {
            GameEvents.WindowStateChanged -= OnWindowStateChanged;
        }

        private void OnWindowStateChanged(WindowState state, bool _)
        {
            bool shouldBeOpen = (state == WindowState.Open);

            if (_windowsOpen == shouldBeOpen)
                return;

            if (shouldBeOpen)
                _animator.Play(openStateName);
            else
                _animator.Play(closeStateName);

            _windowsOpen = shouldBeOpen;
        }
    }
}
