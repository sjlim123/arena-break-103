using ArenaBreak.Core;
using ArenaBreak.Player;
using UnityEngine;
using UnityEngine.UI;

namespace ArenaBreak.UI
{
    public class HUDController : MonoBehaviour
    {
        private const string GameOverTitle = "GAME OVER";
        private const string ClearedTitle = "CLEARED";

        [Header("Event Sources")]
        [SerializeField] private Health _playerHealth;
        [SerializeField] private WeaponSystem _weaponSystem;
        [SerializeField] private WaveSpawner _waveSpawner;
        [SerializeField] private GameManager _gameManager;

        // 앵커 폭으로 줄이는 빨간 바. 부모를 가득 채우도록(앵커 0,0 ~ 1,1, 오프셋 0) 만들어 둘 것
        [Header("Health Bar")]
        [SerializeField] private RectTransform _healthFill;

        [Header("Texts")]
        [SerializeField] private Text _ammoText;
        [SerializeField] private Text _reloadText;
        [SerializeField] private Text _waveText;
        [SerializeField] private Text _killText;

        [Header("Result")]
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private Text _resultTitleText;

        private void Awake()
        {
            // 참조가 빠진 채로 이벤트가 오면 그 시점에 NullReferenceException이 나므로 여기서 끈다
            if (!TryValidateReferences())
            {
                enabled = false;
            }
        }

        private void OnEnable()
        {
            _playerHealth.OnHealthChanged += HandleHealthChanged;
            _weaponSystem.OnAmmoChanged += HandleAmmoChanged;
            _weaponSystem.OnReloadingChanged += HandleReloadingChanged;
            _waveSpawner.OnWaveStarted += HandleWaveStarted;
            _waveSpawner.OnKillCountChanged += HandleKillCountChanged;
            _gameManager.OnStateChanged += HandleStateChanged;

            // 아직 이벤트가 오지 않은 값의 초기 표시. 체력은 가득 찬 채로 시작하고, 탄약과 웨이브는 시작할 때 이벤트가 온다
            SetHealthRatio(1f);
            HandleKillCountChanged(0);
            _reloadText.enabled = false;
            _resultPanel.SetActive(false);
        }

        private void OnDisable()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged -= HandleHealthChanged;
            }

            if (_weaponSystem != null)
            {
                _weaponSystem.OnAmmoChanged -= HandleAmmoChanged;
                _weaponSystem.OnReloadingChanged -= HandleReloadingChanged;
            }

            if (_waveSpawner != null)
            {
                _waveSpawner.OnWaveStarted -= HandleWaveStarted;
                _waveSpawner.OnKillCountChanged -= HandleKillCountChanged;
            }

            if (_gameManager != null)
            {
                _gameManager.OnStateChanged -= HandleStateChanged;
            }
        }

        private bool TryValidateReferences()
        {
            bool isValid = true;

            isValid &= !IsMissing(_playerHealth, "플레이어 Health");
            isValid &= !IsMissing(_weaponSystem, "WeaponSystem");
            isValid &= !IsMissing(_waveSpawner, "WaveSpawner");
            isValid &= !IsMissing(_gameManager, "GameManager");
            isValid &= !IsMissing(_healthFill, "체력바 Fill");
            isValid &= !IsMissing(_ammoText, "탄약 텍스트");
            isValid &= !IsMissing(_reloadText, "재장전 텍스트");
            isValid &= !IsMissing(_waveText, "웨이브 텍스트");
            isValid &= !IsMissing(_killText, "킬 텍스트");
            isValid &= !IsMissing(_resultPanel, "결과 패널");
            isValid &= !IsMissing(_resultTitleText, "결과 제목 텍스트");

            return isValid;
        }

        private bool IsMissing(Object reference, string label)
        {
            if (reference != null)
            {
                return false;
            }

            Debug.LogError($"HUDController: {label}이(가) 비어 있다. 인스펙터에서 지정할 것.", this);
            return true;
        }

        private void HandleHealthChanged(int current, int max)
        {
            float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
            SetHealthRatio(ratio);
        }

        private void SetHealthRatio(float ratio)
        {
            // Image.fillAmount는 스프라이트가 없는 Image에서는 적용되지 않아서 앵커 폭으로 줄인다
            Vector2 anchorMax = _healthFill.anchorMax;
            anchorMax.x = ratio;
            _healthFill.anchorMax = anchorMax;
        }

        private void HandleAmmoChanged(int current, int max)
        {
            _ammoText.text = $"{current} / {max}";
        }

        private void HandleReloadingChanged(bool isReloading)
        {
            // 씬이 정리되는 중에 호출되면 텍스트가 이미 사라졌을 수 있다
            if (_reloadText == null)
            {
                return;
            }

            _reloadText.enabled = isReloading;
        }

        private void HandleWaveStarted(int waveNumber, int totalWaves)
        {
            _waveText.text = $"WAVE {waveNumber} / {totalWaves}";
        }

        private void HandleKillCountChanged(int kills)
        {
            _killText.text = $"KILLS {kills}";
        }

        private void HandleStateChanged(GameManager.State state)
        {
            switch (state)
            {
                case GameManager.State.GameOver:
                    ShowResult(GameOverTitle);
                    break;
                case GameManager.State.Cleared:
                    ShowResult(ClearedTitle);
                    break;
                default:
                    _resultPanel.SetActive(false);
                    break;
            }
        }

        private void ShowResult(string title)
        {
            _resultTitleText.text = title;
            _resultPanel.SetActive(true);
        }
    }
}
