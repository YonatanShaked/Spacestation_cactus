using Core.Events;
using UnityEngine;

namespace Gameplay.Window
{
    [RequireComponent(typeof(Animator))]
    public sealed class WindowAnimator : MonoBehaviour
    {
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
