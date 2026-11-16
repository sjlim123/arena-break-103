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

            // 풀링 대상: 웨이브마다 적이 계속 생성/파괴된다
            EnemyAI enemy = Instantiate(prefab, point.position, point.rotation);
            enemy.SetPlayer(_player);

            // 사망 이벤트를 받을 수 없으면 웨이브가 영영 끝나지 않으므로 세지 않고 되돌린다
            if (!enemy.TryGetComponent(out Health health))
            {
                Debug.LogError($"WaveSpawner: '{prefab.name}'에 Health가 없다.", this);
                Destroy(enemy.gameObject);
                return;
            }

            health.OnDied += HandleEnemyDied;
            _waveEnemies.Add(health);
            _aliveCount++;
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
