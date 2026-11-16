using UnityEngine;
using UnityEngine.AI;

namespace ArenaBreak.Enemy
{
    public class RangedEnemyAI : EnemyAI
    {
        [Header("Distance")]
        [Min(0f)] [SerializeField] private float _preferredDistance = 10f;
        [Min(0f)] [SerializeField] private float _retreatDistance = 6f;
        // 경계에서 접근/후퇴가 매 프레임 번갈아 일어나지 않게 상태를 바꾸는 거리에 두는 여유
        [Min(0f)] [SerializeField] private float _distanceTolerance = 1.5f;

        [Header("Retreat")]
        [Min(0f)] [SerializeField] private float _retreatStep = 4f;
        [Min(0f)] [SerializeField] private float _retreatSampleRadius = 2f;
        [Min(0f)] [SerializeField] private float _minRetreatGain = 0.5f;

        [Header("Fire")]
        [SerializeField] private Projectile _projectilePrefab;
        [Min(0f)] [SerializeField] private float _fireInterval = 2f;
        [Min(0)] [SerializeField] private int _projectileDamage = 10;
        [Min(0f)] [SerializeField] private float _muzzleOffset = 0.8f;

        private float _nextFireTime;
        private bool _isRetreating;

        protected override void OnEnable()
        {
            base.OnEnable();

            // 풀에서 다시 꺼낼 때도 처음 상태로 시작해야 하므로 이 클래스의 타이머와 상태도 OnEnable에서 초기화한다
            _nextFireTime = 0f;
            _isRetreating = false;

            if (_projectilePrefab == null)
            {
                Debug.LogError("RangedEnemyAI: 투사체 프리팹이 비어 있다. 인스펙터에서 지정할 것.", this);
            }
        }

        protected override void UpdateChase()
        {
            if (Player == null)
            {
                ChangeState(State.Idle);
                return;
            }

            if (GetDistanceToPlayer() <= _preferredDistance)
            {
                ChangeState(State.Attack);
                return;
            }

            Agent.SetDestination(Player.position);
        }

        protected override void UpdateAttack()
        {
            if (Player == null)
            {
                _isRetreating = false;
                ChangeState(State.Idle);
                return;
            }

            float distance = GetDistanceToPlayer();

            if (distance > _preferredDistance + _distanceTolerance)
            {
                _isRetreating = false;
                ChangeState(State.Chase);
                return;
            }

            UpdateRetreat(distance);

            if (Time.time >= _nextFireTime)
            {
                Fire();
            }
        }

        private void UpdateRetreat(float distance)
        {
            // 후퇴를 시작하는 거리와 끝내는 거리를 다르게 둬서 경계에서 떨지 않게 한다
            if (distance < _retreatDistance)
            {
                _isRetreating = true;
            }
            else if (distance >= _retreatDistance + _distanceTolerance)
            {
                _isRetreating = false;
            }

            // 부모의 ChangeState가 Attack에 들어갈 때 이동을 멈추므로, 후퇴하는 동안만 멈춤을 푼다.
            // 갈 곳이 없으면 멈춘 채로 그 자리에서 계속 쏜다
            if (_isRetreating && TrySetRetreatDestination(distance))
            {
                Agent.isStopped = false;
                return;
            }

            Agent.isStopped = true;
        }

        private bool TrySetRetreatDestination(float currentDistance)
        {
            Vector3 away = transform.position - Player.position;
            away.y = 0f;

            if (away.sqrMagnitude <= 0f)
            {
                return false;
            }

            Vector3 candidate = transform.position + away.normalized * _retreatStep;

            if (!NavMesh.SamplePosition(candidate, out NavMeshHit navHit, _retreatSampleRadius, NavMesh.AllAreas))
            {
                return false;
            }

            // NavMesh 위로 보정된 지점이 지금보다 충분히 멀어지지 않으면 벽 모서리에 막힌 것이다
            Vector3 toPoint = navHit.position - Player.position;
            toPoint.y = 0f;

            if (toPoint.magnitude < currentDistance + _minRetreatGain)
            {
                return false;
            }

            Agent.SetDestination(navHit.position);
            return true;
        }

        private void Fire()
        {
            _nextFireTime = Time.time + _fireInterval;

            if (_projectilePrefab == null)
            {
                return;
            }

            Vector3 toPlayer = Player.position - transform.position;
            Vector3 flatDirection = new Vector3(toPlayer.x, 0f, toPlayer.z);

            if (flatDirection.sqrMagnitude <= 0f)
            {
                return;
            }

            // 자기 콜라이더 밖에서 시작해야 하고, 높이는 적 위치 그대로 둔다
            Vector3 spawnPosition = transform.position + flatDirection.normalized * _muzzleOffset;
            Vector3 direction = (Player.position - spawnPosition).normalized;

            // 풀링 대상: 발사할 때마다 투사체가 생성된다
            Projectile projectile = Instantiate(_projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
            projectile.Launch(direction, Player, _projectileDamage);

            Debug.LogFormat("[RangedEnemyAI] {0} 발사 - 데미지 {1}", name, _projectileDamage);
        }
    }
}
