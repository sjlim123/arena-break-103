using System;
using System.Collections;
using System.Collections.Generic;
using ArenaBreak.Enemy;
using UnityEngine;

namespace ArenaBreak.Core
{
    public class WaveSpawner : MonoBehaviour
    {
        [Header("Waves")]
        [SerializeField] private List<WaveData> _waves;
        [Min(0f)] [SerializeField] private float _nextWaveDelay = 3f;

        [Header("Spawn")]
        [SerializeField] private Transform _spawnPoints;
        [SerializeField] private Transform _player;

        private readonly List<Transform> _spawnPointList = new List<Transform>();
        private readonly List<Health> _waveEnemies = new List<Health>();
        private readonly Dictionary<EnemyAI, List<EnemyAI>> _pools = new Dictionary<EnemyAI, List<EnemyAI>>();
        private readonly Dictionary<EnemyAI, Health> _healthByEnemy = new Dictionary<EnemyAI, Health>();
        private int _aliveCount;

        public event Action<int, int> OnWaveStarted;
        public event Action<int, int> OnWaveCleared;
        public event Action OnAllWavesCleared;

        private void Awake()
        {
            // 참조가 빠진 채로 웨이브를 돌리면 스폰 도중에 NullReferenceException이 나므로 여기서 끈다
            if (!TryCacheReferences())
            {
                enabled = false;
            }
        }

        private void Start()
        {
            StartCoroutine(RunWaves());
        }

        private void OnDisable()
        {
            UnsubscribeAll();
        }

        private bool TryCacheReferences()
        {
            if (_waves == null || _waves.Count == 0)
            {
                Debug.LogError("WaveSpawner: 웨이브 리스트가 비어 있다. WaveData를 순서대로 넣을 것.", this);
                return false;
            }

            for (int i = 0; i < _waves.Count; i++)
            {
                if (_waves[i] == null || _waves[i].EnemyPrefab == null)
                {
                    Debug.LogError($"WaveSpawner: 웨이브 {i + 1}번의 WaveData나 적 프리팹이 비어 있다.", this);
                    return false;
                }
            }

            if (_spawnPoints == null)
            {
                Debug.LogError("WaveSpawner: 스폰 위치 오브젝트가 비어 있다. SpawnPoints를 지정할 것.", this);
                return false;
            }

            if (_player == null)
            {
                Debug.LogError("WaveSpawner: 플레이어가 비어 있다. 인스펙터에서 지정할 것.", this);
                return false;
            }

            foreach (Transform child in _spawnPoints)
            {
                _spawnPointList.Add(child);
            }

            if (_spawnPointList.Count == 0)
            {
                Debug.LogError("WaveSpawner: SpawnPoints에 자식이 없다.", this);
                return false;
            }

            return true;
        }

        private IEnumerator RunWaves()
        {
            for (int i = 0; i < _waves.Count; i++)
            {
                int waveNumber = i + 1;

                // 3장에서 HUD로 옮길 임시 출력. 문자열 연결을 피하려고 LogFormat을 쓴다
                Debug.LogFormat("[WaveSpawner] 웨이브 {0}/{1} 시작 - 적 {2}마리", waveNumber, _waves.Count, _waves[i].SpawnCount);
                OnWaveStarted?.Invoke(waveNumber, _waves.Count);

                yield return SpawnWave(_waves[i]);

                // 스폰이 끝난 뒤에만 기다린다. 스폰 도중에 먼저 나온 적을 다 잡아도 웨이브가 끝난 게 아니다
                yield return new WaitUntil(() => _aliveCount <= 0);

                UnsubscribeAll();
                Debug.LogFormat("[WaveSpawner] 웨이브 {0}/{1} 클리어", waveNumber, _waves.Count);
                OnWaveCleared?.Invoke(waveNumber, _waves.Count);

                if (waveNumber < _waves.Count)
                {
                    yield return new WaitForSeconds(_nextWaveDelay);
                }
            }

            Debug.Log("[WaveSpawner] 전체 클리어");
            OnAllWavesCleared?.Invoke();
        }

        private IEnumerator SpawnWave(WaveData wave)
        {
            // 간격마다 새로 만들면 스폰할 때마다 가비지가 생기므로 웨이브당 하나만 만든다
            WaitForSeconds interval = new WaitForSeconds(wave.SpawnInterval);

            for (int i = 0; i < wave.SpawnCount; i++)
            {
                SpawnEnemy(wave.EnemyPrefab);

                if (i < wave.SpawnCount - 1)
                {
                    yield return interval;
                }
            }
        }

        private void SpawnEnemy(EnemyAI prefab)
        {
            Transform point = _spawnPointList[UnityEngine.Random.Range(0, _spawnPointList.Count)];

            EnemyAI enemy = GetEnemy(prefab, point.position, point.rotation);
            if (enemy == null)
            {
                return;
            }

            enemy.SetPlayer(_player);

            // 같은 웨이브 안에서 죽은 적이 비활성화된 뒤 다시 꺼내질 수 있다.
            // 구독이 겹치면 한 번 죽을 때 두 번 세므로 먼저 해제하고 다시 구독한다
            Health health = _healthByEnemy[enemy];
            health.OnDied -= HandleEnemyDied;
            health.OnDied += HandleEnemyDied;

            if (!_waveEnemies.Contains(health))
            {
                _waveEnemies.Add(health);
            }

            _aliveCount++;
        }

        private EnemyAI GetEnemy(EnemyAI prefab, Vector3 position, Quaternion rotation)
        {
            if (!_pools.TryGetValue(prefab, out List<EnemyAI> pool))
            {
                pool = new List<EnemyAI>();
                _pools.Add(prefab, pool);
            }

            // 죽은 적은 EnemyAI가 스스로 비활성화하므로 꺼져 있는 것이 풀에 돌아온 적이다
            for (int i = 0; i < pool.Count; i++)
            {
                EnemyAI pooled = pool[i];
                if (pooled != null && !pooled.gameObject.activeSelf)
                {
                    // 꺼진 상태에서 옮겨야 NavMeshAgent가 켜질 때 새 위치에 놓인다
                    pooled.transform.SetPositionAndRotation(position, rotation);
                    pooled.gameObject.SetActive(true);
                    return pooled;
                }
            }

            EnemyAI created = Instantiate(prefab, position, rotation);

            // 사망 이벤트를 받을 수 없으면 웨이브가 영영 끝나지 않으므로 풀에 넣지 않고 되돌린다
            if (!created.TryGetComponent(out Health health))
            {
                Debug.LogError($"WaveSpawner: '{prefab.name}'에 Health가 없다.", this);
                Destroy(created.gameObject);
                return null;
            }

            _healthByEnemy.Add(created, health);
            pool.Add(created);
            return created;
        }

        private void HandleEnemyDied()
        {
            _aliveCount--;
        }

        private void UnsubscribeAll()
        {
            for (int i = 0; i < _waveEnemies.Count; i++)
            {
                if (_waveEnemies[i] != null)
                {
                    _waveEnemies[i].OnDied -= HandleEnemyDied;
                }
            }

            _waveEnemies.Clear();
        }
    }
}
