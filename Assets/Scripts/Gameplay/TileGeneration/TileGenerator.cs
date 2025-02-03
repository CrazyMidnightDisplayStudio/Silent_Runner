using System.Collections.Generic;
using System.Linq;
using Gameplay.City;
using UnityEngine;

namespace Gameplay.TileGeneration
{
    public class TileGenerator : MonoBehaviour
    {
        [SerializeField] private CityTile _prefab;

        public IReadOnlyList<CityTile> Tiles => _tiles;

        private List<CityTile> _tiles = new List<CityTile>();
        private int _lastTileId = 0;

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
            var outerTile = Instantiate(_prefab, position + Vector3.left * 15f, Quaternion.identity, transform);
            outerTile.Id = _lastTileId++;
            _tiles.Add(outerTile);

            var frontTile = Instantiate(_prefab, position, Quaternion.identity, transform);
            frontTile.Id = _lastTileId++;
            _tiles.Add(frontTile);

            while (_tiles.Count > 4)
            {
                Debug.Log($"tiles count befor destroy = {_tiles.Count}");
                DestroyLastTile();
            }
        }

        private void DestroyLastTile()
        {
            var tile = _tiles.First();
            _tiles.Remove(tile);
            Destroy(tile.gameObject);
        }

        private void Start()
        {
            Generate(CityRoot.RootPosition);
        }

        private void Update()
        {
            if (CityMover.IsRotating) return;

            if (_tiles.Last().isTilePassedByPlayer())
            {
                Generate(_tiles.Last().transform.position + (-CityRoot.MoveDirection * 15f));
            }
        }
    }
}
