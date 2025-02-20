using Gameplay.City;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Gameplay
{
    public class CoreGameplayScript : MonoBehaviour
    {
        [SerializeField] private CityMover _cityMover;
        private PlayerInputActions _playerInputActions;
        private TileTracker _tileTracker;

        [Inject]
        private void Init(TileTracker tileTracker, PlayerInputActions playerInputActions)
        {
            _tileTracker = tileTracker;
            _playerInputActions = playerInputActions;
        }

        private void OnEnable()
        {
            _playerInputActions.Gameplay.Enable();
            _playerInputActions.Gameplay.Turn.performed += OnRotate;
        }

        private void OnDisable()
        {
            _playerInputActions.Gameplay.Turn.performed -= OnRotate;
            _playerInputActions.Gameplay.Disable();
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
