namespace Gameplay.GameSpeed
{
    public class SpeedController
    {
        public static float Speed { get; private set; } = 2.5f;

        public SpeedController(float speed)
        {
            Speed = speed;
        }
    }
}
