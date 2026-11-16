using ArenaBreak.Core;
using UnityEngine;
using UnityEngine.AI;

namespace ArenaBreak.Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        private enum State
        {
            Idle,
            Chase,
            Attack,
            Dead
        }

        [Header("Target")]
        [SerializeField] private Transform _player;

        [Header("Range")]
        [Min(0f)] [SerializeField] private float _chaseStartDistance = 60f;
        [Min(0f)] [SerializeField] private float _attackDistance = 2f;

        [Header("Attack")]
        [Min(0f)] [SerializeField] private float _attackInterval = 2f;
        [Min(0)] [SerializeField] private int _attackDamage = 10;

        [Header("Death")]
        [Min(0f)] [SerializeField] private float _destroyDelay = 1f;

        private NavMeshAgent _agent;
        private Health _health;
        private IDamageable _playerDamageable;

        private State _state = State.Idle;
        private float _nextAttackTime;

        // 프리팹은 씬 오브젝트를 참조할 수 없어서, 스폰한 쪽이 Instantiate 직후에 플레이어를 넣어준다
        public void SetPlayer(Transform player)
        {
            _player = player;
            CachePlayerDamageable();
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();

            // 참조가 빠진 채로 Update가 돌면 매 프레임 NullReferenceException이 쌓이므로 여기서 끈다
            if (_agent == null || _health == null)
            {
                Debug.LogError("EnemyAI: NavMeshAgent와 Health가 같은 오브젝트에 있어야 한다.", this);
                enabled = false;
                return;
            }

            // 씬에 직접 둔 적은 인스펙터 참조로 이미 플레이어가 들어 있다
            CachePlayerDamageable();
        }

        private void OnEnable()
        {
            if (_health == null)
            {
                return;
            }

            _health.OnDied += HandleDied;
        }

        private void OnDisable()
        {
            if (_health == null)
            {
                return;
            }

            _health.OnDied -= HandleDied;
        }

        private void Start()
        {
            // Awake 시점에는 스폰한 쪽이 아직 SetPlayer를 부르지 못했으므로 Start에서 확인한다
            if (_player == null)
            {
                Debug.LogWarning("EnemyAI: 플레이어 참조가 비어 있다. 스폰한 쪽이 SetPlayer를 호출해야 한다.", this);
            }
        }

        private void Update()
        {
            switch (_state)
            {
                case State.Idle:
                    UpdateIdle();
                    break;
                case State.Chase:
                    UpdateChase();
                    break;
                case State.Attack:
                    UpdateAttack();
                    break;
                case State.Dead:
                    break;
            }
        }

        private void UpdateIdle()
        {
            if (_player == null)
            {
                return;
            }

            if (GetDistanceToPlayer() <= _chaseStartDistance)
            {
                ChangeState(State.Chase);
            }
        }

        private void UpdateChase()
        {
            if (_player == null)
            {
                ChangeState(State.Idle);
                return;
            }

            if (GetDistanceToPlayer() <= _attackDistance)
            {
                ChangeState(State.Attack);
                return;
            }

            _agent.SetDestination(_player.position);
        }

        private void UpdateAttack()
        {
            if (_player == null)
            {
                ChangeState(State.Idle);
                return;
            }

            if (GetDistanceToPlayer() > _attackDistance)
            {
                ChangeState(State.Chase);
                return;
            }

            if (Time.time >= _nextAttackTime)
            {
                Attack();
            }
        }

        private void Attack()
        {
            _nextAttackTime = Time.time + _attackInterval;

            // 플레이어에게 Health가 없는 동안은 이 로그로만 공격이 일어나는지 확인한다
            Debug.LogFormat("[EnemyAI] {0} 공격 - 데미지 {1}", name, _attackDamage);

            if (_playerDamageable != null)
            {
                _playerDamageable.TakeDamage(_attackDamage);
            }
        }

        private void HandleDied()
        {
            ChangeState(State.Dead);
            Destroy(gameObject, _destroyDelay);
        }

        private void ChangeState(State next)
        {
            _state = next;

            // 이동 여부는 상태에 들어갈 때 한 번만 정한다
            _agent.isStopped = next != State.Chase;
        }

        private float GetDistanceToPlayer()
        {
            Vector3 toPlayer = _player.position - transform.position;

            // 플레이어 피벗이 캡슐 중앙(높이 1)이라 높이 차가 공격 거리에 섞이지 않게 한다
            toPlayer.y = 0f;

            return toPlayer.magnitude;
        }

        private void CachePlayerDamageable()
        {
            _playerDamageable = _player != null ? _player.GetComponentInParent<IDamageable>() : null;
        }
    }
}
