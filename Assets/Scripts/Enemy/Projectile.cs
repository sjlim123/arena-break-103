using ArenaBreak.Core;
using UnityEngine;

namespace ArenaBreak.Enemy
{
    public class Projectile : MonoBehaviour
    {
        // 한 프레임에 한 번의 스윕이 겹칠 수 있는 충돌 수의 상한
        private const int MaxHitsPerSweep = 16;

        [Min(0f)] [SerializeField] private float _speed = 12f;
        [Min(0f)] [SerializeField] private float _lifetime = 5f;
        [Min(0.01f)] [SerializeField] private float _radius = 0.15f;

        private readonly RaycastHit[] _hits = new RaycastHit[MaxHitsPerSweep];

        private Vector3 _direction;
        private Transform _target;
        private int _damage;
        private float _despawnTime;
        private bool _isLaunched;

        public void Launch(Vector3 direction, Transform target, int damage)
        {
            _direction = direction.normalized;
            _target = target;
            _damage = damage;
            _despawnTime = Time.time + _lifetime;
            _isLaunched = true;
        }

        private void Update()
        {
            if (!_isLaunched)
            {
                return;
            }

            float distance = _speed * Time.deltaTime;

            // 속도가 빠르면 한 프레임에 벽을 통과하므로 이동할 거리만큼 쓸어서 충돌을 검사한다
            int count = Physics.SphereCastNonAlloc(transform.position, _radius, _direction, _hits, distance);

            if (TryFindBlockingHit(count, out RaycastHit hit, out bool isTarget))
            {
                if (isTarget)
                {
                    ApplyHit(hit.collider);
                }

                Despawn();
                return;
            }

            transform.position += _direction * distance;

            if (Time.time >= _despawnTime)
            {
                Despawn();
            }
        }

        private bool TryFindBlockingHit(int count, out RaycastHit nearest, out bool isTarget)
        {
            nearest = default;
            isTarget = false;

            float nearestDistance = float.MaxValue;
            bool found = false;

            for (int i = 0; i < count; i++)
            {
                RaycastHit hit = _hits[i];

                // 이미 더 가까운 충돌이 있으면 이 충돌은 볼 필요가 없다 (불필요한 컴포넌트 조회를 줄인다)
                if (hit.distance >= nearestDistance)
                {
                    continue;
                }

                bool hitTarget = _target != null && hit.collider.transform.IsChildOf(_target);

                // 대상이 아닌 IDamageable(다른 적, 발사한 적 자신)은 통과한다. 적끼리 맞으면 안 된다
                if (!hitTarget && hit.collider.GetComponentInParent<IDamageable>() != null)
                {
                    continue;
                }

                nearest = hit;
                nearestDistance = hit.distance;
                isTarget = hitTarget;
                found = true;
            }

            return found;
        }

        private void ApplyHit(Collider targetCollider)
        {
            IDamageable damageable = targetCollider.GetComponentInParent<IDamageable>();

            // 플레이어에게 Health 가 없는 동안은 이 로그로만 명중을 확인한다
            Debug.LogFormat("[Projectile] 명중 - 데미지 {0}, 대상에 IDamageable {1}", _damage, damageable != null);

            if (damageable != null)
            {
                damageable.TakeDamage(_damage);
            }
        }

        private void Despawn()
        {
            // 풀링 대상: 발사할 때마다 생성되고 사라지는 오브젝트다
            Destroy(gameObject);
        }
    }
}
