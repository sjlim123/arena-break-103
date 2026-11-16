using System;
using UnityEngine;

namespace ArenaBreak.Core
{
    public class Health : MonoBehaviour, IDamageable
    {
        [Min(1)] [SerializeField] private int _maxHealth = 100;

        // 플레이 중 인스펙터에서 값을 눈으로 확인하려고 직렬화한다. 쓰기는 이 클래스만 한다
        [SerializeField] private int _currentHealth;

        private bool _isDead;

        public int CurrentHealth => _currentHealth;

        public event Action<int, int> OnHealthChanged;
        public event Action OnDied;

        private void OnEnable()
        {
            // 풀에서 다시 꺼낼 때도 처음 상태로 돌아가야 하므로 Awake가 아니라 OnEnable에서 초기화한다
            _currentHealth = _maxHealth;
            _isDead = false;
        }

        public void TakeDamage(int amount)
        {
            // 음수 데미지가 회복으로 동작하는 것을 막는다
            if (_isDead || amount <= 0)
            {
                return;
            }

            _currentHealth = Mathf.Max(_currentHealth - amount, 0);

            // 이벤트 안에서 다시 TakeDamage가 호출돼도 사망이 두 번 발생하지 않도록 먼저 표시한다
            _isDead = _currentHealth <= 0;

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_isDead)
            {
                // 파괴나 게임오버 같은 사망 반응은 플레이어와 적이 달라서 구독하는 쪽이 정한다
                OnDied?.Invoke();
            }
        }
    }
}
