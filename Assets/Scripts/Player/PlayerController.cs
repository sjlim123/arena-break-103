using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaBreak.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private const string PlayerMapName = "Player";
        private const string MoveActionName = "Move";
        private const string LookActionName = "Look";
        private const string JumpActionName = "Jump";

        [Header("Input")]
        [SerializeField] private InputActionAsset _inputActions;

        [Header("Camera")]
        [SerializeField] private Transform _cameraTransform;

        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpHeight = 1.2f;
        [SerializeField] private float _gravity = -20f;
        // 0으로 두면 CharacterController.isGrounded가 프레임마다 깜빡여서 점프 판정이 흔들린다
        [SerializeField] private float _groundedStickVelocity = -2f;

        [Header("Look")]
        [SerializeField] private float _lookSensitivity = 0.1f;
        [SerializeField] private float _maxPitchAngle = 80f;
        // 커서가 잠기는 첫 프레임에 마우스 delta가 크게 튀는 것을 막는다
        [SerializeField] private float _maxLookDeltaPerFrame = 50f;

        [Header("Cursor")]
        [SerializeField] private bool _lockCursor = true;

        private CharacterController _controller;
        private InputActionMap _playerMap;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _jumpAction;

        private float _pitch;
        private float _verticalVelocity;

        private bool _cursorOverridden;
        private CursorLockMode _previousLockState;
        private bool _previousCursorVisible;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            // 참조가 빠진 채로 Update가 돌면 매 프레임 NullReferenceException이 쌓이므로 여기서 끈다
            if (!TryCacheReferences())
            {
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (_playerMap == null)
            {
                return;
            }

            _playerMap.Enable();
            LockCursor();
        }

        private void OnDisable()
        {
            if (_playerMap == null)
            {
                return;
            }

            _playerMap.Disable();
            RestoreCursor();
        }

        private void Update()
        {
            ApplyLook();
            ApplyMovement();
        }

        private bool TryCacheReferences()
        {
            if (_inputActions == null)
            {
                Debug.LogError("PlayerController: InputActionAsset이 비어 있다. 인스펙터에서 지정할 것.", this);
                return false;
            }

            if (_cameraTransform == null)
            {
                Debug.LogError("PlayerController: 카메라 Transform이 비어 있다. 자식 카메라를 지정할 것.", this);
                return false;
            }

            _playerMap = _inputActions.FindActionMap(PlayerMapName);
            if (_playerMap == null)
            {
                Debug.LogError($"PlayerController: '{_inputActions.name}'에 '{PlayerMapName}' 액션 맵이 없다.", this);
                return false;
            }

            _moveAction = _playerMap.FindAction(MoveActionName);
            _lookAction = _playerMap.FindAction(LookActionName);
            _jumpAction = _playerMap.FindAction(JumpActionName);

            if (_moveAction == null || _lookAction == null || _jumpAction == null)
            {
                Debug.LogError(
                    $"PlayerController: '{PlayerMapName}' 맵에 {MoveActionName} / {LookActionName} / {JumpActionName} 액션이 모두 있어야 한다.",
                    this);
                _playerMap = null;
                return false;
            }

            return true;
        }

        private void ApplyLook()
        {
            Vector2 look = Vector2.ClampMagnitude(_lookAction.ReadValue<Vector2>(), _maxLookDeltaPerFrame);

            // 마우스 delta는 이미 프레임당 이동량이라 deltaTime을 곱하면 프레임레이트에 따라 감도가 달라진다
            transform.Rotate(Vector3.up, look.x * _lookSensitivity);

            _pitch = Mathf.Clamp(_pitch - look.y * _lookSensitivity, -_maxPitchAngle, _maxPitchAngle);
            _cameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void ApplyMovement()
        {
            bool isGrounded = _controller.isGrounded;

            if (isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = _groundedStickVelocity;
            }

            if (isGrounded && _jumpAction.WasPressedThisFrame())
            {
                // v = sqrt(2gh): 인스펙터에서 높이로 조절하려고 속도로 환산한다
                _verticalVelocity = Mathf.Sqrt(Mathf.Max(0f, _jumpHeight * -2f * _gravity));
            }

            _verticalVelocity += _gravity * Time.deltaTime;

            // 대각선 입력이 1을 넘어 빨라지지 않게 한다
            Vector2 move = Vector2.ClampMagnitude(_moveAction.ReadValue<Vector2>(), 1f);
            Vector3 horizontal = (transform.right * move.x + transform.forward * move.y) * _moveSpeed;

            _controller.Move((horizontal + Vector3.up * _verticalVelocity) * Time.deltaTime);

            // 천장에 머리를 박았는데 상승 속도가 남아 있으면 천장에 매달려 있다가 뒤늦게 떨어진다
            if ((_controller.collisionFlags & CollisionFlags.Above) != 0 && _verticalVelocity > 0f)
            {
                _verticalVelocity = 0f;
            }
        }

        private void LockCursor()
        {
            if (!_lockCursor)
            {
                return;
            }

            _previousLockState = Cursor.lockState;
            _previousCursorVisible = Cursor.visible;
            _cursorOverridden = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void RestoreCursor()
        {
            if (!_cursorOverridden)
            {
                return;
            }

            Cursor.lockState = _previousLockState;
            Cursor.visible = _previousCursorVisible;
            _cursorOverridden = false;
        }
    }
}
