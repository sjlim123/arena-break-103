using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaBreak.Player
{
    public class PlayerDash : MonoBehaviour
    {
        private const string PlayerMapName = "Player";
        private const string SprintActionName = "Sprint";

        [Header("Input")]
        [SerializeField] private InputActionAsset _inputActions;

        [Header("Dash")]
        [Min(0f)] [SerializeField] private float _dashDistance = 6f;
        [Min(0.01f)] [SerializeField] private float _dashDuration = 0.2f;
        [Min(0f)] [SerializeField] private float _cooldown = 1.5f;

        private CharacterController _controller;
        private InputAction _sprintAction;

        private bool _isDashing;
        private Vector3 _dashDirection;
        private float _dashSpeed;
        private float _dashTimeLeft;

        private bool _isOnCooldown;
        private float _cooldownEndTime;

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
            if (_sprintAction == null)
            {
                return;
            }

            // 에셋을 PlayerController, WeaponSystem과 공유하므로 맵 전체를 끄면 그쪽 입력까지 꺼진다
            _sprintAction.Enable();
        }

        private void OnDisable()
        {
            if (_sprintAction == null)
            {
                return;
            }

            _sprintAction.Disable();

            // 돌진 도중에 꺼지면 남은 시간이 그대로 남아 다시 켜졌을 때 이어서 돌진해 버린다
            _isDashing = false;
        }

        private void Update()
        {
            if (_isDashing)
            {
                ApplyDash();
            }

            if (_isOnCooldown && Time.time >= _cooldownEndTime)
            {
                _isOnCooldown = false;
                Debug.Log("[PlayerDash] 쿨다운 종료 - 대시 가능");
            }

            if (!_isDashing && !_isOnCooldown && _sprintAction.WasPressedThisFrame())
            {
                StartDash();
            }
        }

        private bool TryCacheReferences()
        {
            if (_controller == null)
            {
                Debug.LogError("PlayerDash: CharacterController가 없다. 같은 오브젝트에 붙일 것.", this);
                return false;
            }

            if (_inputActions == null)
            {
                Debug.LogError("PlayerDash: InputActionAsset이 비어 있다. 인스펙터에서 지정할 것.", this);
                return false;
            }

            InputActionMap playerMap = _inputActions.FindActionMap(PlayerMapName);
            if (playerMap == null)
            {
                Debug.LogError($"PlayerDash: '{_inputActions.name}'에 '{PlayerMapName}' 액션 맵이 없다.", this);
                return false;
            }

            _sprintAction = playerMap.FindAction(SprintActionName);
            if (_sprintAction == null)
            {
                Debug.LogError($"PlayerDash: '{PlayerMapName}' 맵에 '{SprintActionName}' 액션이 없다.", this);
                return false;
            }

            return true;
        }

        private void StartDash()
        {
            // 위나 아래를 보고 있어도 수직으로 날아가지 않도록 수평으로 투영한다.
            // 대시 중 마우스를 돌려도 궤적이 휘지 않도록 시작 시점의 방향을 고정한다
            _dashDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            _dashSpeed = _dashDistance / _dashDuration;
            _dashTimeLeft = _dashDuration;
            _isDashing = true;

            _isOnCooldown = true;
            _cooldownEndTime = Time.time + _cooldown;

            Debug.LogFormat("[PlayerDash] 대시 시작 - 쿨다운 {0}초", _cooldown);
        }

        private void ApplyDash()
        {
            // 마지막 프레임에서는 남은 시간만큼만 이동해야 총 거리가 설정값을 넘지 않는다
            float step = Mathf.Min(Time.deltaTime, _dashTimeLeft);
            _controller.Move(_dashDirection * (_dashSpeed * step));

            _dashTimeLeft -= step;
            if (_dashTimeLeft <= 0f)
            {
                _isDashing = false;
            }
        }
    }
}
