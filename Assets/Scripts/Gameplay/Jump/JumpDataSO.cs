using UnityEngine;

namespace Gameplay.Jump
{
    [CreateAssetMenu(menuName = "Jump Settings Data")]
    public class JumpDataSO : ScriptableObject
    {
        [Header("Jump")]
        public float jumpHeight; //Высота прыжка игрока (максимальная)
        public float jumpTimeToApex; //Время в секундах между применением силы прыжка и достижением желаемой высоты прыжка
        [Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Льготный период после нажатия кнопки перейти, в течение которого переход будет выполнен автоматически после выполнения требований (например, при заземлении).
        [Range(0.01f, 0.5f)] public float coyoteTime; //Льготный период после падения с платформы, когда вы все еще можете спрыгнуть
        [HideInInspector] public float jumpForce; //Фактическая сила, приложенная (вверх) к игроку при прыжке.

        [Space(5)]
        [Header("Gravity")]
        [HideInInspector] public float gravityStrength; //Направленное вниз усилие (сила тяжести), необходимое для достижения желаемой высоты и времени прыжка.
        [Range(-1, 1)]
        public float fallGravityScale; //Изменяет скорость падения игрока после достижения пика прыжка на процент (+ ускоряет, - замедляет)
        [Range(-1, 1)]
        public float jumpCutGravityScale; //Изменяет скорость падения игрока, если игрок отпускает кнопку прыжка, продолжая прыгать (1 - 100% прыжка без прерывания, -1 - -100% от текущей скорости прыжка)
        public float maxFallSpeed; //Максимальная скорость падения (предельная скорость) игрока при падении.
        [Range(0f, 0.9f)]
        public float jumpHangGravityMult; //Уменьшает силу тяжести, находясь близко к вершине (желаемой максимальной высоте) прыжка, позволяет немного зависнуть в воздухе


        //Обратный вызов Unity, вызываемый при обновлении инспектора
        private void OnValidate()
        {
            // Рассчёт силы гравитации от желаемой высоты прыжка и времени достижения этой высоты (gravity = 2 * jumpHeight / timeToJumpApex^2)  обратная формула g = 2h / t²
            gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
            Debug.Log($"Гравитация равна: {gravityStrength}");
            //Нужная сила прыжка для данной гравитации и времени(jumpForce = gravity * timeToJumpApex)
            jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
            Debug.Log($"Сила прыжка равна: {jumpForce}");

        }
    }
}
