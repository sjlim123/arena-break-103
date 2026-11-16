using ArenaBreak.Core;
using UnityEngine;

namespace ArenaBreak.Enemy
{
    public class HitFlash : MonoBehaviour
    {
        // URP Lit의 메인 색 프로퍼티. 머티리얼 에셋은 적끼리 공유하므로 이 렌더러만 덮어쓴다
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField] private Color _flashColor = Color.white;
        [Min(0f)] [SerializeField] private float _flashDuration = 0.05f;

        private Health _health;
        private Renderer _renderer;
        private MaterialPropertyBlock _block;

        private bool _isFlashing;
        private float _flashEndTime;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _renderer = GetComponentInChildren<Renderer>();

            // 참조가 빠진 채로 이벤트가 오면 그 시점에 NullReferenceException이 나므로 여기서 끈다
            if (_health == null || _renderer == null)
            {
                Debug.LogError("HitFlash: Health와 Renderer가 이 오브젝트(또는 자식)에 있어야 한다.", this);
                enabled = false;
                return;
            }

            _block = new MaterialPropertyBlock();

            if (!_renderer.sharedMaterial.HasProperty(BaseColorId))
            {
                Debug.LogWarning("HitFlash: 머티리얼에 _BaseColor가 없어 플래시가 보이지 않는다. URP Lit을 쓸 것.", this);
            }
        }

        private void OnEnable()
        {
            if (_health == null)
            {
                return;
            }

            _health.OnHealthChanged += HandleHealthChanged;

            // 풀에서 다시 꺼낼 때 이전 플래시가 남아 있지 않게 한다
            EndFlash();
        }

        private void OnDisable()
        {
            if (_health == null)
            {
                return;
            }

            _health.OnHealthChanged -= HandleHealthChanged;

            // 플래시 도중에 꺼져도 다음에 재사용될 때 흰색으로 남지 않게 되돌린다
            EndFlash();
        }

        private void Update()
        {
            // 게임이 멈춰도(timeScale 0) 플래시가 흰색으로 굳지 않도록 멈추지 않는 시간으로 센다
            if (_isFlashing && Time.unscaledTime >= _flashEndTime)
            {
                EndFlash();
            }
        }

        private void HandleHealthChanged(int current, int max)
        {
            StartFlash();
        }

        private void StartFlash()
        {
            _renderer.GetPropertyBlock(_block);
            _block.SetColor(BaseColorId, _flashColor);
            _renderer.SetPropertyBlock(_block);

            _flashEndTime = Time.unscaledTime + _flashDuration;
            _isFlashing = true;
        }

        private void EndFlash()
        {
            _isFlashing = false;
            _renderer.SetPropertyBlock(null);
        }
    }
}
