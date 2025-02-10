using UnityEngine;
using Zenject;

namespace Gameplay.TileGeneration
{
    public class TileTrackerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<TileTracker>().FromComponentsInHierarchy().AsSingle();
        }
    }
}
