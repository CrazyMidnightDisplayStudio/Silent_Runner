using System.Collections.Generic;
using System.Linq;
using Gameplay.City;
using UnityEngine;
using Zenject;

namespace Gameplay.TileGeneration
{
    public class TileGenerator : MonoBehaviour
    {
        [SerializeField] private CityTile _prefab;
        private int _lastTileId;
        private readonly List<CityTile> _tiles = new();

        private TileTracker _tileTracker;

        public IReadOnlyList<CityTile> Tiles => _tiles;

        private void Start()
        {
            // Генерируем первые 4 тайла при старте
            Generate(CityRoot.RootPosition);
            Generate(_tiles.Last().transform.position + -CityRoot.MoveDirection * 15f);

            _tileTracker.SubscribeToProgress(0.3f,
                () => { Generate(_tiles.Last().transform.position - CityRoot.MoveDirection * 15f); }, false, false);
        }

        [Inject]
        private void Init(TileTracker tileTracker) => _tileTracker = tileTracker;

        public void onRotation()
        {
            if (_tiles.Count < 4)
            {
                Debug.LogError("Tile count is less than 4");
                return;
            }

            (_tiles[0], _tiles[1], _tiles[2], _tiles[3]) =
                (_tiles[1], _tiles[3], _tiles[0], _tiles[2]);
        }

        private void Generate(Vector3 position)
        {
            CityTile outerTile = Instantiate(_prefab, position + Vector3.left * 15f, Quaternion.identity, transform);
            outerTile.Id = _lastTileId++;
            _tiles.Add(outerTile);

            CityTile frontTile = Instantiate(_prefab, position, Quaternion.identity, transform);
            frontTile.Id = _lastTileId++;
            _tiles.Add(frontTile);

            while (_tiles.Count > 4)
            {
                DestroyLastTile();
            }
        }

        private void DestroyLastTile()
        {
            CityTile tile = _tiles.First();
            _tiles.Remove(tile);
            Destroy(tile.gameObject);
        }
    }
}
