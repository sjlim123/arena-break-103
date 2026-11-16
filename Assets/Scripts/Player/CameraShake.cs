using ArenaBreak.Core;
using UnityEngine;

namespace ArenaBreak.Player
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private Health _playerHealth;
        [Min(0f)] [SerializeField] private float _duration = 0.1f;
        // 카메라가 기준 위치에서 최대로 벗어나는 거리(미터). 시간이 지날수록 줄어든다
        [Min(0f)] [SerializeField] private float _intensity = 0.05f;

        private Vector3 _basePosition;
        private bool _isShaking;
        private float _shakeStartTime;

        private void Awake()
        {
            // 참조가 빠진 채로 이벤트가 오면 그 시점에 NullReferenceException이 나므로 여기서 끈다
            if (_playerHealth == null)
            {
                Debug.LogError("CameraShake: 플레이어 Health가 비어 있다. 인스펙터에서 지정할 것.", this);
                enabled = false;
                return;
            }

            _basePosition = transform.localPosition;
        }

        private void OnEnable()
        {
            if (_playerHealth == null)
            {
                return;
            }

            _playerHealth.OnHealthChanged += HandleHealthChanged;
        }

        private void OnDisable()
        {
            if (_playerHealth == null)
            {
                return;
            }

            _playerHealth.OnHealthChanged -= HandleHealthChanged;

            // 흔들리는 도중에 꺼져도 카메라가 어긋난 위치에 남지 않게 한다
            StopShake();
        }

        // PlayerController가 Update에서 회전을 매 프레임 덮어쓰므로, 회전은 건드리지 않고 위치만 흔든다
        private void LateUpdate()
        {
            if (!_isShaking)
            {
                return;
            }

            // 게임이 멈춰도(timeScale 0) 흔들림이 어긋난 채로 굳지 않도록 멈추지 않는 시간으로 센다
            float elapsed = Time.unscaledTime - _shakeStartTime;
            if (elapsed >= _duration)
            {
                StopShake();
                return;
            }

            // 끝으로 갈수록 약해지게 해서 멈출 때 갑자기 튀지 않게 한다
            float falloff = 1f - elapsed / _duration;
            transform.localPosition = _basePosition + Random.insideUnitSphere * (_intensity * falloff);
        }

        private void HandleHealthChanged(int current, int max)
        {
            // 흔들리는 도중에 또 맞으면 겹쳐서 커지지 않고 처음부터 다시 센다
            _shakeStartTime = Time.unscaledTime;
            _isShaking = true;
        }

        private void StopShake()
        {
            _isShaking = false;
            transform.localPosition = _basePosition;
        }
    }
}
