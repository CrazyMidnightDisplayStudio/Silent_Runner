using Gameplay.GameSpeed;
using Gameplay.TileGeneration;
using Gameplay.Utils;
using UnityEngine;

namespace Gameplay.City
{
    public class CityMover : MonoBehaviour
    {
        [SerializeField] private TileGenerator _tileGenerator;

        private RotateObject _rotator = new RotateObject();
        public static bool IsRotating { get; private set; } = false;

        private void FixedUpdate()
        {
            foreach (var tile in _tileGenerator.Tiles)
            {
                tile.MoveInFixedUpdate(SpeedController.Speed);
            }
        }

        public async void Rotate()
        {
            if (IsRotating) return;

            IsRotating = true;
            _tileGenerator.onRotation();
            await _rotator.RotateToAngleTask(transform, CityRoot.RotateDirection, 1f);
            IsRotating = false;
        }

        private void OnDestroy()
        {
            _rotator.Dispose();
        }
    }
}
