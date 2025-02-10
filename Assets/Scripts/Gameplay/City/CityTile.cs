using UnityEngine;

namespace Gameplay.City
{
    public class CityTile : MonoBehaviour
    {
        public static float TileSize = 15f;

        private int _id;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                name = $"CityTile{_id}";
            }
        }

        public Vector3 GetPosition() => transform.position;

        public bool isTilePassedByPlayer()
        {
            return GetPosition().z < CityRoot.RootPosition.z;
        }

        public void MoveInFixedUpdate(float speed)
        {
            transform.Translate(CityRoot.MoveDirection * speed * Time.fixedDeltaTime, Space.World);
        }
    }
}
