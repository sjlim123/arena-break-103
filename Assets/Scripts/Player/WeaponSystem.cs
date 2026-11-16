using System;
using ArenaBreak.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ArenaBreak.Player
{
    public class WeaponSystem : MonoBehaviour
    {
        private const string PlayerMapName = "Player";
        private const string AttackActionName = "Attack";
        private const string ReloadActionName = "Reload";
        private const string ReloadBinding = "<Keyboard>/r";
        private const string MarkerTemplateName = "HitMarkerTemplate";

        private static readonly Vector3 ViewportCenter = new Vector3(0.5f, 0.5f, 0f);

        [Header("Input")]
        [SerializeField] private InputActionAsset _inputActions;

        [Header("Camera")]
        [SerializeField] private Camera _camera;

        [Header("Fire")]
        [SerializeField] private float _fireInterval = 0.15f;
        [SerializeField] private float _range = 50f;
        [Min(0)] [SerializeField] private int _damage = 10;

        [Header("Ammo")]
        [SerializeField] private int _maxAmmo = 12;
        [SerializeField] private float _reloadDuration = 1.5f;

        [Header("Hit Marker (debug)")]
        [SerializeField] private Color _markerColor = Color.yellow;
        [SerializeField] private float _markerScale = 0.15f;
        [SerializeField] private float _markerDuration = 0.5f;

        private InputAction _attackAction;
        private InputAction _reloadAction;
        private GameObject _markerTemplate;

        private int _currentAmmo;
        private float _nextFireTime;
        private bool _isReloading;
        private float _reloadEndTime;

        public event Action<int, int> OnAmmoChanged;

        private void Awake()
        {
            _currentAmmo = _maxAmmo;

            // 참조가 빠진 채로 Update가 돌면 매 프레임 NullReferenceException이 쌓이므로 여기서 끈다
            if (!TryCacheReferences())
            {
                enabled = false;
                return;
            }

            // 에셋에 Reload 액션이 없어서 코드로 만든다
            _reloadAction = new InputAction(ReloadActionName, InputActionType.Button, ReloadBinding);
            _markerTemplate = CreateMarkerTemplate();
        }

        private void Start()
        {
            // HUD가 구독한 뒤에 첫 값을 받을 수 있도록 시작할 때 한 번 알린다
            OnAmmoChanged?.Invoke(_currentAmmo, _maxAmmo);
        }

        private void OnEnable()
        {
            if (_attackAction == null)
            {
                return;
            }

            // 에셋을 PlayerController와 공유하므로 맵 전체가 아니라 이 액션들만 켜고 끈다
            _attackAction.Enable();
            _reloadAction.Enable();
        }

        private void OnDisable()
        {
            if (_attackAction == null)
            {
                return;
            }

            _attackAction.Disable();
            _reloadAction.Disable();

            // 재장전 도중에 꺼졌다가 켜지면 타이머가 이미 지나 있어서 즉시 완료돼 버린다
            _isReloading = false;
        }

        private void OnDestroy()
        {
            _reloadAction?.Dispose();

            if (_markerTemplate != null)
            {
                Destroy(_markerTemplate);
            }
        }

        private void Update()
        {
            if (_isReloading)
            {
                if (Time.time >= _reloadEndTime)
                {
                    FinishReload();
                }

                return;
            }

            if (_reloadAction.WasPressedThisFrame() && _currentAmmo < _maxAmmo)
            {
                StartReload();
                return;
            }

            if (_attackAction.IsPressed() && _currentAmmo > 0 && Time.time >= _nextFireTime)
            {
                Fire();
            }
        }

        private bool TryCacheReferences()
        {
            if (_inputActions == null)
            {
                Debug.LogError("WeaponSystem: InputActionAsset이 비어 있다. 인스펙터에서 지정할 것.", this);
                return false;
            }

            if (_camera == null)
            {
                Debug.LogError("WeaponSystem: Camera가 비어 있다. 플레이어의 자식 카메라를 지정할 것.", this);
                return false;
            }

            InputActionMap playerMap = _inputActions.FindActionMap(PlayerMapName);
            if (playerMap == null)
            {
                Debug.LogError($"WeaponSystem: '{_inputActions.name}'에 '{PlayerMapName}' 액션 맵이 없다.", this);
                return false;
            }

            _attackAction = playerMap.FindAction(AttackActionName);
            if (_attackAction == null)
            {
                Debug.LogError($"WeaponSystem: '{PlayerMapName}' 맵에 '{AttackActionName}' 액션이 없다.", this);
                return false;
            }

            return true;
        }

        private GameObject CreateMarkerTemplate()
        {
            GameObject template = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            template.name = MarkerTemplateName;
            template.transform.localScale = Vector3.one * _markerScale;

            // 구체에 콜라이더가 남아 있으면 다음 발사가 구체에 맞는다.
            // Destroy는 프레임 끝에 처리돼서 같은 프레임에 복제하면 콜라이더가 딸려오므로 즉시 제거한다
            DestroyImmediate(template.GetComponent<Collider>());

            template.GetComponent<Renderer>().material.color = _markerColor;
            template.SetActive(false);

            return template;
        }

        private void Fire()
        {
            _nextFireTime = Time.time + _fireInterval;
            _currentAmmo--;

            Ray ray = _camera.ViewportPointToRay(ViewportCenter);
            if (Physics.Raycast(ray, out RaycastHit hit, _range))
            {
                ShowHitMarker(hit.point);
                ApplyDamage(hit.collider);
            }

            OnAmmoChanged?.Invoke(_currentAmmo, _maxAmmo);
        }

        private void ApplyDamage(Collider target)
        {
            // 콜라이더가 자식에 붙은 대상도 맞도록 부모까지 찾는다. 벽과 바닥은 없어서 구체만 남기고 넘어간다
            IDamageable damageable = target.GetComponentInParent<IDamageable>();
            if (damageable == null)
            {
                return;
            }

            damageable.TakeDamage(_damage);
        }

        private void ShowHitMarker(Vector3 point)
        {
            // 풀링 대상: 연사하는 동안 구체가 계속 생성/파괴된다
            GameObject marker = Instantiate(_markerTemplate, point, Quaternion.identity);
            marker.SetActive(true);
            Destroy(marker, _markerDuration);
        }

        private void StartReload()
        {
            _isReloading = true;
            _reloadEndTime = Time.time + _reloadDuration;
        }

        private void FinishReload()
        {
            _isReloading = false;
            _currentAmmo = _maxAmmo;

            OnAmmoChanged?.Invoke(_currentAmmo, _maxAmmo);
        }
    }
}
