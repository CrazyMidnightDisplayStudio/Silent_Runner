using System;
using UnityEngine;
using UnityEngine.Windows;
using Zenject;

namespace Gameplay.Jump
{
    public class JumpController : MonoBehaviour
    {
        [SerializeField] private JumpDataSO _jumpData;
        private Rigidbody rb;
        private bool _isJumping = false;
        private bool _isFalling = false;
        private bool _isJumpCut = false;
        private bool isPressButton = false;
        [Header("Checks")]
        [SerializeField] private Transform _groundCheckPoint;
        private bool isGround = false;
        [SerializeField] private float _radius = 3;
        [SerializeField] LayerMask _groundLayer;
        public float LastPressedJumpTime { get; private set; } // время касания земли, нужно для буфферизации прыжка ,если игрок нажал чуть раньше чем игрок каснулся тригером земли
        public float LastOnGroundTime { get; private set; } // время кайота
        private PlayerInputActions _playerInputActions;
        private PlayerInputActions _inputs;

        [Inject]
        public void Construct(PlayerInputActions inputs)
        {
            _inputs = inputs;
        }
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            IsGravity(false); // время касания земли нужно для буфферизации прыжка
            SubscibeInput();
        }

        void Update()
        {
            LastPressedJumpTime -= Time.deltaTime;

            if (CanJump() && LastPressedJumpTime > 0)
            {
                Jump();
            }
            CheckedGround();
        }

        private void StartJump()
        {
            OnJump();
            isPressButton = true;
        }
        private void CancelJump()
        {
            isPressButton = false;
            _isJumpCut = CanJumpCut();
        }
        private void CheckedGround()
        {
            if (_isJumping && rb.velocity.y < 0)
            {
                _isJumping = false;
                _isFalling = true;
                _isJumpCut = false;
            }
            if (!_isJumping && LastOnGroundTime > 0f)
            {
                _isJumpCut = false;
                _isFalling = false;
            }
            if (!_isJumping && Physics.OverlapSphere(_groundCheckPoint.position, _radius, _groundLayer).Length > 0)
            {
                LastOnGroundTime = _jumpData.coyoteTime;
            }
        }
        private void FixedUpdate()
        {
            ApplyGravity();
        }
        private void Jump()
        {
            _isJumping = true;
            LastPressedJumpTime = 0;
            LastOnGroundTime = 0;
            rb.AddForce(0, _jumpData.jumpForce, 0, ForceMode.Impulse);
        }
        private void OnJump()
        {
            LastPressedJumpTime = _jumpData.jumpInputBufferTime;
        }
        private bool CanJump()
        {
            return LastOnGroundTime > 0 && !_isJumping;
        }
        private bool CanJumpCut()
        {
            return _isJumping && rb.velocity.y > 0;
        }
        private void IsGravity(bool isGravity)
        {
            rb.useGravity = isGravity;
        }
        private void ApplyGravity()
        {
            float gravity = _jumpData.gravityStrength;
            if (_isJumpCut)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * _jumpData.jumpCutGravityScale, 0);
            }
            else if (Math.Abs(rb.velocity.y) < _jumpData.jumpHangGravityMult && isPressButton && (_isJumping || _isFalling))
            {
                gravity = _jumpData.gravityStrength - _jumpData.gravityStrength * _jumpData.jumpHangGravityMult;
            }
            else if (rb.velocity.y < 0)
            {
                gravity = _jumpData.gravityStrength + _jumpData.gravityStrength * _jumpData.fallGravityScale;
            }
            rb.velocity += new Vector3(0, gravity * Time.fixedDeltaTime, 0);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_groundCheckPoint.position, _radius);
        }
        private void SubscibeInput()
        {
            _inputs.Gameplay.Jump.started += _ => StartJump();
            _inputs.Gameplay.Jump.canceled += _ => CancelJump();
        }
        private void UnsubscibeInput()
        {
            _inputs.Gameplay.Jump.started -= _ => StartJump();
            _inputs.Gameplay.Jump.canceled -= _ => CancelJump();
        }
        private void OnDestroy()
        {
            UnsubscibeInput();
        }
    }
}
