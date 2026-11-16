using System;
using ArenaBreak.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ArenaBreak.Core
{
    public class GameManager : MonoBehaviour
    {
        public enum State
        {
            Ready,
            Playing,
            GameOver,
            Cleared
        }

        private const string RestartActionName = "Restart";
        private const string RestartBinding = "<Keyboard>/r";
        private const string QuitActionName = "Quit";
        private const string QuitBinding = "<Keyboard>/escape";
        private const float PausedTimeScale = 0f;
        private const float NormalTimeScale = 1f;

        [Header("Ready")]
        [Min(0f)] [SerializeField] private float _readyDuration = 2f;

        [Header("Player")]
        [SerializeField] private Health _playerHealth;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private WeaponSystem _weaponSystem;
        [SerializeField] private PlayerDash _playerDash;

        [Header("Waves")]
        [SerializeField] private WaveSpawner _waveSpawner;

        private InputAction _restartAction;
        private InputAction _quitAction;

        private State _state = State.Ready;
        private float _readyEndTime;

        public State CurrentState => _state;

        public event Action<State> OnStateChanged;

        private void Awake()
        {
            // 참조가 빠진 채로 상태가 바뀌면 그 시점에 NullReferenceException이 나므로 여기서 끈다
            if (!TryValidateReferences())
            {
                enabled = false;
                return;
            }

            // 에셋에 R/Esc 액션이 없어서 코드로 만든다
            _restartAction = new InputAction(RestartActionName, InputActionType.Button, RestartBinding);
            _quitAction = new InputAction(QuitActionName, InputActionType.Button, QuitBinding);
        }

        private void OnEnable()
        {
            if (_restartAction == null)
            {
                return;
            }

            _restartAction.Enable();
            _quitAction.Enable();

            _playerHealth.OnDied += HandlePlayerDied;
            _waveSpawner.OnAllWavesCleared += HandleAllWavesCleared;
        }

        private void OnDisable()
        {
            if (_restartAction == null)
            {
                return;
            }

            _restartAction.Disable();
            _quitAction.Disable();

            if (_playerHealth != null)
            {
                _playerHealth.OnDied -= HandlePlayerDied;
            }

            if (_waveSpawner != null)
            {
                _waveSpawner.OnAllWavesCleared -= HandleAllWavesCleared;
            }
        }

        private void OnDestroy()
        {
            _restartAction?.Dispose();
            _quitAction?.Dispose();

            // 멈춘 시간이 이 오브젝트가 사라진 뒤에도 남지 않게 한다
            Time.timeScale = NormalTimeScale;
        }

        private void Start()
        {
            _readyEndTime = Time.time + _readyDuration;
            ChangeState(State.Ready);
        }

        private void Update()
        {
            // 어느 상태에서든 종료할 수 있어야 한다
            if (_quitAction.WasPressedThisFrame())
            {
                Quit();
                return;
            }

            switch (_state)
            {
                case State.Ready:
                    UpdateReady();
                    break;
                case State.Playing:
                    break;
                case State.GameOver:
                case State.Cleared:
                    UpdateEnded();
                    break;
            }
        }

        private bool TryValidateReferences()
        {
            if (_playerHealth == null)
            {
                Debug.LogError("GameManager: 플레이어 Health가 비어 있다. 인스펙터에서 지정할 것.", this);
                return false;
            }

            if (_playerController == null)
            {
                Debug.LogError("GameManager: PlayerController가 비어 있다. 인스펙터에서 지정할 것.", this);
                return false;
            }

            if (_weaponSystem == null)
            {
                Debug.LogError("GameManager: WeaponSystem이 비어 있다. 인스펙터에서 지정할 것.", this);
                return false;
            }

            if (_waveSpawner == null)
            {
                Debug.LogError("GameManager: WaveSpawner가 비어 있다. 인스펙터에서 지정할 것.", this);
                return false;
            }

            if (_playerDash == null)
            {
                Debug.LogWarning("GameManager: PlayerDash가 비어 있다. 게임이 끝난 뒤에도 대시가 가능하다.", this);
            }

            return true;
        }

        private void UpdateReady()
        {
            if (Time.time >= _readyEndTime)
            {
                ChangeState(State.Playing);
            }
        }

        private void UpdateEnded()
        {
            if (_restartAction.WasPressedThisFrame())
            {
                Restart();
            }
        }

        private void HandlePlayerDied()
        {
            if (IsEnded())
            {
                return;
            }

            ChangeState(State.GameOver);
        }

        private void HandleAllWavesCleared()
        {
            if (IsEnded())
            {
                return;
            }

            ChangeState(State.Cleared);
        }

        private bool IsEnded()
        {
            return _state == State.GameOver || _state == State.Cleared;
        }

        private void ChangeState(State next)
        {
            _state = next;

            switch (next)
            {
                case State.Ready:
                case State.Playing:
                    break;
                case State.GameOver:
                case State.Cleared:
                    EnterEndState();
                    break;
            }

            // 3장에서 HUD로 옮길 임시 출력
            Debug.LogFormat("[GameManager] 상태 변경 - {0}", next);
            OnStateChanged?.Invoke(next);
        }

        private void EnterEndState()
        {
            // timeScale 0 은 Update를 멈추지 않아서, 입력 컴포넌트를 끄지 않으면 끝난 뒤에도 총이 나간다
            SetPlayerControlEnabled(false);

            // PlayerController가 꺼지면서 커서를 이전 상태로 돌리므로, 그 뒤에 확실히 보이게 한다
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = PausedTimeScale;
        }

        private void Restart()
        {
            // 멈춘 채로 씬을 불러오면 다시 시작한 씬도 멈춰 있다
            Time.timeScale = NormalTimeScale;

            // 끌 때와 짝을 맞춰 되돌린다. 씬 재로드를 쓰지 않는 재시작(부활 등)에서도 조작이 살아 있어야 한다
            SetPlayerControlEnabled(true);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void SetPlayerControlEnabled(bool isEnabled)
        {
            _playerController.enabled = isEnabled;
            _weaponSystem.enabled = isEnabled;

            if (_playerDash != null)
            {
                _playerDash.enabled = isEnabled;
            }
        }

        private void Quit()
        {
#if UNITY_EDITOR
            // 에디터에서는 Application.Quit이 아무 일도 하지 않아서 플레이 모드를 끈다
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
