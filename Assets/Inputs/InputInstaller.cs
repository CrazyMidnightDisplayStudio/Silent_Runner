using Zenject;

namespace Assets.Inputs
{
    public class InputInstaller: MonoInstaller
    {
        private PlayerInputActions _playerInput;
        public override void InstallBindings()
        {
            _playerInput = new PlayerInputActions();
            _playerInput.Enable();
            Container.Bind<PlayerInputActions>().FromInstance(_playerInput).AsSingle();
        }
        private void OnDestroy()
        {
            _playerInput.Disable();
        }
    }
}
