using Gameplay.City;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Gameplay
{
    public class CoreGameplayScript : MonoBehaviour
    {
        [SerializeField] private CityMover _cityMover;
        private PlayerInputActions _controls;
        private TileTracker _tileTracker;

        [Inject]
        private void Init(TileTracker tileTracker)
        {
            _tileTracker = tileTracker;
        }

        private void Awake()
        {
            _controls = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _controls.Gameplay.Enable();
            _controls.Gameplay.Turn.performed += OnRotate;
        }

        private void OnDisable()
        {
            _controls.Gameplay.Turn.performed -= OnRotate;
            _controls.Gameplay.Disable();
        }
        private void OnRotate(InputAction.CallbackContext context)
        {
            if (_tileTracker.ProgressInTile < 0.7f)
            {
                Debug.Log("Early turn blocked");
                return;
            }
            _tileTracker.SubscribeToProgress(0.9f, _cityMover.Rotate, true, true);
            Debug.Log("Rotate callback registered");
        }
    }
}
