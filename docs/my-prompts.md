# 내 프롬프트 로그

> **제출물입니다.** 
> 수업과 과제에서 실제로 사용한 프롬프트를 **전문 그대로** 기록하세요.
> **실패한 시도를 지우지 마세요.** 실패 기록이 회고 점수의 근거가 됩니다.

- 이름: 2026109042
- 학번: 임성진

---

## 기록 형식

각 작업마다 아래 형식으로 남깁니다.

````markdown
## <장> — <무엇을 만들었나>

### 시도 1
```
(프롬프트 전문을 여기에 붙여넣기)
```
**결과**:
**문제점**:
**원인 분석**:

### 시도 2
```
(수정한 프롬프트 전문)
```
**무엇을 바꿨나**:
**결과**:
**배운 점**:
````

---

## 1장

### 1-1. CLAUDE.md 작성

````markdown
## <1장> — <클로드 md 다듬기>

### 시도 1
```
적 체력을 담을 간단한 클래스를 만들어줘.
현재 체력을 public 필드로 노출해서 다른 스크립트가 바로 접근할 수 있게 해줘.
파일은 만들지 말고 코드만 보여줘.
```
**결과**:public 필드는 CLAUDE.md 규약(“public 필드 금지, 필요하면 프로퍼티”)에 어긋나서 그대로 만들지 않았다. 
대신 다른 스크립트가 CurrentHealth로 바로 읽을 수 있게 프로퍼티로 노출함. 
읽는 쪽에서는 사용법이 같음. 파일은 만들지 않았고 코드만 보여줌.
**문제점**: 없음. 문제없이 규직 작동


### 시도 2(추가)
```
예외 무시 금지 try-catch 블록을 비워두거나 단순히 에러만 삼키지 않고, 반드시 적절한 에러 처리나 사용자 피드백 로직을 연결합니다.
```
**무엇을 바꿨나**: 위 문구를 CLAUDE.md 에 나만의 규칙으로 추가함. 그다음 규칙 커밋함.
**결과**: main 브랜치에 커밋했어요: ac94e61 docs: CLAUDE.md에 개인 규약 추가. 변경 파일은 CLAUDE.md 하나 라는 정상적인 결과 +작업 완료됨.
````



### 1-2. 플레이어 이동
````markdown
## <1-2> — <플레이어이동>

### 시도 1
```
1인칭 플레이어 컨트롤러를 만들어줘.

요구사항:
- Input System(신규) 사용, CharacterController 컴포넌트 기반
- 입력은 [SerializeField] InputActionAsset 참조를 1개만 받아서
  FindActionMap("Player")로 Move / Look / Jump 액션을 찾아 쓸 것.
  PlayerInput 컴포넌트나 Keyboard.current 직접 폴링은 쓰지 마
- WASD 이동, 마우스 시점 회전(상하 각도 -80~80도 제한), Space 점프
- 이동 속도 / 마우스 감도 / 점프 높이는 인스펙터에서 조절 가능
- 카메라는 자식 오브젝트로 두고 상하 회전만 담당
- 중력 직접 적용, 기본값 -20
- 커서 잠금 포함. 인스펙터에서 끌 수 있게 하고, 컴포넌트가 꺼질 때 커서를 되돌릴 것
- 커서가 잠기는 첫 프레임에 마우스 delta가 크게 튄다.
  프레임당 delta에 상한을 둘 것 (상한값은 인스펙터에서 조절)

파일: Assets/Scripts/Player/PlayerController.cs 하나만 생성.
씬은 건드리지 말고, 내가 직접 붙일 테니 필요한 세팅 절차만 알려줘.
```
**결과**:점프 속도를 어떻게 계산했는가?, 점프 속도를 어떻게 계산했는가? 와 같은 확인할 것 모두 확인했을때 문제 없이 완벽한 결과값이 나옴.
+Unity 콘솔에 에러와 경고가 없어요. 0건이에요. 답변까지 받음으로써 문제가 전혀 없음
### 시도 2
```
방금 만든 PlayerController를 씬에 붙여줘. MCP 도구로 직접 해줘.

- Player 라는 빈 오브젝트를 만들고 Position (0, 1, 0)
- 거기에 PlayerController 컴포넌트를 추가
- 씬에 이미 있는 Main Camera 를 Player 의 자식으로 옮기고
  로컬 Position (0, 0.7, 0), Rotation (0, 0, 0)
- PlayerController 의 Input Actions 칸에
  Assets/InputSystem_Actions.inputactions 를 연결
- Camera Pivot 칸에 그 Main Camera 를 연결

끝나면 컴포넌트를 다시 읽어서 두 칸에 실제로 값이 들어갔는지 보여줘.
```
**결과**:컴포넌트를 제대로 찾았고 성공했음. 오류도 없었음. + 마지막 사람이 해야하는 검증 까지 모두 문제가 전혀 없었음. 오류도 0.커밋도 정상적으로 완료.

````
### 1-3. 사격 시스템
````markdown
## <3> — <사격시스템>

### 시도 1
```
사격 연습용 표적을 Arena 씬에 배치해줘.

- Targets 라는 빈 오브젝트를 Position (0, 0, 0) 에 만들고 그 아래에 자식으로 넣을 것
- Target_1 / Target_2 / Target_3 — 전부 Cube
- Scale (1, 2, 1)
- Position: y=1, z=8, x는 각각 -3 / 0 / 3
- Assets/Materials 에 빨간색 URP Lit 머티리얼을 만들어 표적 3개에 적용할 것
```
**결과**: 모두 성공 + 오류 0 작동문제도 없었음.
허나, 7. 큰 기능은 설계안 2~3개 먼저	⚠️ PlayerController는 설계안 없이 바로 만들었어요. 요구사항이 아주 구체적이라 생략했는데, 규칙대로면 물었어야 했을 수 있어요 < 7번 규칙에 대해서는 스스로 부정확함을 인지함.

**배운 점**: 과정은 거의 완벽하나 사용자의 규칙에 대해서는 아직 덜 완벽함. 따라서 사람이 규칙을 보고 어기거나 변질된 부분을 확인 및 검증을 거쳐줘야함.
### 시도 2
```
Raycast 기반 사격 시스템을 추가해줘.

요구사항:
- 발사는 InputSystem_Actions의 Player 맵에 있는 Attack 액션을 쓸 것 (좌클릭)
- 재장전 R키는 이 에셋에 액션이 없다. 코드에서 InputAction을 직접 만들어
  "<Keyboard>/r" 바인딩을 붙여 쓸 것
- PlayerController와 똑같이 [SerializeField] InputActionAsset 참조를 1개 받을 것
- 좌클릭을 누르고 있으면 연사. 발사 간격 0.15초, 사거리 50m
- 카메라 정중앙에서 Ray를 쏠 것
- 명중 지점에 디버그용으로 작은 구체를 0.5초간 표시.
  색은 인스펙터에서 바꿀 수 있게 하고 기본값은 노란색 — 벽·표적과 같은 색이면 안 보인다.
  그 구체의 Collider는 반드시 제거할 것 — 안 그러면 다음 발사가 구체에 맞는다
- 탄약 12발, R키 재장전(1.5초, 재장전 중 발사 불가)
- 발사와 재장전 시 남은 탄약을 콘솔에 출력 (3장에서 HUD로 옮길 것이다)
- PlayerController는 수정하지 말고 별도 컴포넌트로

파일: Assets/Scripts/Player/WeaponSystem.cs 하나만 생성.
씬은 건드리지 말고 세팅 절차만 알려줘.
```
**결과**: 모두 성공 + 오류 0 작동문제도 없었음. 코드 읽기과정 씬 세팅 문제 없었음 정상완료! + 오류도 0, input action,camera,playercontroller등등 전부 문제 없이 완료!

**배운 점**:
````
### 1-4. 과제 — 대시 또는 조준
````markdown
## <4> — <대시>

### 시도 1
```
대시(Dash)를 추가해줘.

요구사항:
- 입력은 InputSystem_Actions의 Player 맵에 있는 Sprint 액션을 쓸 것 (Shift).
  새 InputAction을 코드로 만들지 말 것
- PlayerController, WeaponSystem과 똑같이 [SerializeField] InputActionAsset 참조를 1개 받을 것
- 에셋을 다른 컴포넌트와 공유하니까 맵 전체를 Enable/Disable하지 말고
  Sprint 액션만 OnEnable/OnDisable에서 켜고 끌 것
- Shift를 누르면 바라보는 방향으로 짧게 돌진
  · 방향은 transform.forward의 수평(XZ) 성분. 이동 입력(WASD)과 무관하게 항상 정면으로
  · 위나 아래를 보고 있어도 수직으로는 날아가지 않는다
- 돌진 거리 / 돌진 시간 / 쿨다운은 인스펙터에서 조절 가능
  기본값: 거리 6m, 시간 0.2초, 쿨다운 1.5초
- 이동은 CharacterController.Move로 할 것 (벽을 뚫으면 안 된다)
  · Awake에서 CharacterController를 캐싱
- 돌진 중에는 재입력을 무시한다. 쿨다운은 돌진을 시작한 시점부터 센다
- 공중에서도 대시 가능. 수직 속도는 건드리지 않는다 (중력은 PlayerController가 그대로 처리)
- 대시 시작과 쿨다운 종료 시 콘솔에 출력 (검증용. 나중에 HUD로 옮길 것이다)
  · Update 안에서 문자열 연결(+) 금지 → Debug.LogFormat 사용
- 참조(에셋 / Player 맵 / Sprint 액션 / CharacterController)가 비어 있으면
  LogError를 남기고 컴포넌트를 끌 것
- PlayerController와 WeaponSystem은 수정하지 말고 별도 컴포넌트로

파일: Assets/Scripts/Player/PlayerDash.cs 하나만 생성.

PlayerDash 컴포넌트를 Player 에 붙이고,
Input Actions 칸에 Assets/InputSystem_Actions.inputactions 를 연결해줘.

끝나면 각 칸에 실제로 값이 들어갔는지 다시 읽어서 보여줘.

□ Shift를 누르면 바라보는 방향으로 짧게 돌진한다
□ 이동 키를 누르지 않고 Shift만 눌러도 정면으로 나간다
□ 위나 아래를 보고 대시해도 공중으로 뜨거나 땅으로 꺼지지 않는다
□ 거리가 대략 6m다 (표적 z=8 앞에서 시험)
□ 벽에 붙어서 대시해도 벽을 뚫지 않는다
□ 대시 중에 Shift를 다시 눌러도 연속 발동되지 않는다
□ 쿨다운 1.5초 전에 Shift를 눌러도 대시하지 않는다
□ 쿨다운이 끝나면 다시 대시된다 (콘솔 로그로 확인)
□ 공중(점프 중)에서도 대시된다
□ 대시 직후에도 이동, 시점, 점프, 사격이 정상이다
□ 콘솔에 에러가 없다
```
**결과**:대시가 문제 없이 만들어짐, 사람의 테스트 통과, 에러0, 요구사항 및 체크사항 모두 통과
**문제점**:Collapse켜져서 검증을 못했었음
**원인 분석**: 끄기 작업
**배운 점**: 프롬프트를 상세히 하나하나 적어줄수록 결과물과 오류가 현저히 감소
````
---

## 2장

### 2-1. 체력/데미지 설계 논의
```
체력/데미지 시스템을 만들 건데, 먼저 설계부터 논의하자. 코드는 아직 쓰지 마.

플레이어와 적이 같은 방식으로 데미지를 받게 하고 싶어.
IDamageable 인터페이스와 Health 컴포넌트로 나누는 게 좋을까?

사망 처리, 무적 시간, 체력 변화 이벤트를 각각 어디에 두는 게 좋을지
2~3가지 안을 장단점과 함께 제시해줘. 결정은 내가 한다. 
```
사망처리 안A + 고른 이유(본인의 이유) --> Health가 "체력 관리"만 맡음. 플레이어와 적(이펙트, 점수, 풀 반환)이 서로 다른 반응을 너무 어렵지 않게 정리!
무적시간 안B + 고른 이유(본인의 이유) --> 한 곳에서 끝남. 인스펙터에서 시간을 정하고, 적은 0으로 두면 무적 없음이기때문.

체력변화 안A + 고른 이유(본인의 이유) --> HUD 갱신에 충분하고 가장 단순해서 개발 용이.

### 2-2. 체력과 데미지 구현

<!-- WeaponSystem 수정분 git diff 에서 요청하지 않은 변경을 찾았다면 함께 기록 -->

방금 논의한 설계대로 체력 시스템을 만들어줘.
```
- IDamageable: TakeDamage(int amount) 하나만 가진 인터페이스
- Health: IDamageable을 구현하는 MonoBehaviour
  - 최대 체력은 인스펙터에서 조절, 기본 100
  - 현재 체력은 밖에서 프로퍼티로 읽기만 가능하되,
    플레이 중 인스펙터에서 값이 보이게 할 것 — 눈으로 검증해야 한다
  - 체력이 변하면 이벤트로 (현재, 최대) 를 알릴 것
  - 0 이하가 되면 사망 이벤트를 한 번만 발생시킬 것 (여러 번 맞아도 한 번)
  - 죽은 뒤에는 더 이상 데미지를 받지 않을 것
- 사망 시 오브젝트를 파괴하는 것은 Health가 하지 않는다. 이벤트를 받는 쪽이 정한다

파일 2개만: Assets/Scripts/Core/IDamageable.cs, Assets/Scripts/Core/Health.cs
씬은 건드리지 마.

WeaponSystem이 맞힌 대상에게 데미지를 주도록 고쳐줘.

- Raycast에 맞은 오브젝트에서 IDamageable을 찾아서 TakeDamage 호출
- 데미지 값은 인스펙터에서 조절, 기본 10
- IDamageable이 없는 대상(벽, 바닥)은 지금처럼 구체만 표시하고 넘어갈 것
- 기존 기능(연사 간격, 탄약, 재장전, 명중 구체)은 그대로 유지

WeaponSystem.cs만 수정하고 다른 파일은 건드리지 마.
```
**결과**: 컴파일 오류 0, 작동이 에러 0,눈으로 검증했을때 문제 없음 확인완료++.
**문제점**: 사람이 직접 확인해야하는 작업 이외 없음.
**배운 점**: 여러 단계를 거쳐가면서 만들어나감으로써 요청사항의 정확도 향상이 눈에 보임.
````

### 2-3. 적 AI

```
NavMeshAgent 기반 근접 적을 만들어줘.

요구사항:
- 상태는 Idle / Chase / Attack / Dead — enum + switch로 단순하게.
  상태 머신 프레임워크나 별도 클래스를 만들지 마
- 플레이어를 추적. 추적 시작 거리와 공격 거리는 인스펙터에서 조절
  (기본: 추적 60m, 공격 2m)
- 공격은 2초 간격, 데미지 10. 앞서 만든 IDamageable로 줄 것
- 공격할 때마다 콘솔에 로그를 남길 것.
  지금은 플레이어에게 Health가 없어서 공격이 일어나는지 로그로만 확인할 수 있다
- 체력은 앞서 만든 Health 컴포넌트를 쓴다. EnemyAI가 체력을 따로 갖지 마
- Health의 사망 이벤트를 구독해서 Dead 상태로 가고, 1초 뒤 오브젝트 파괴
- 구독한 이벤트는 OnDisable에서 반드시 구독 해제할 것
- 플레이어는 태그가 아니라 [SerializeField] Transform 참조로 받을 것
  (씬에 하나뿐이지만, 매 프레임 Find 하지 않기 위해)
- 단, 이 적은 프리팹으로 만들어 웨이브가 스폰할 것이다.
  프리팹은 씬 오브젝트를 참조할 수 없으니, 스폰한 쪽이 플레이어를 넣어줄 수 있게
  공개 메서드를 하나 둘 것

파일: Assets/Scripts/Enemy/EnemyAI.cs 하나만 생성.
씬과 프리팹은 건드리지 말고 세팅 절차만 알려줘.
```

적 프리팹을 만들어줘. MCP 도구로 직접 해줘.

- Capsule 로 만들고 이름은 Enemy
- Position (0, 1, 0), Scale 은 기본값
- NavMeshAgent 추가 — Speed 3.5, Stopping Distance 1.8
- ArenaBreak.Core.Health 추가 — 최대 체력 30
- ArenaBreak.Enemy.EnemyAI 추가
- Assets/Materials 에 주황색 URP Lit 머티리얼을 만들어 적용
  (표적은 빨강이므로 구분되어야 한다)
- Assets/Prefabs/Enemy.prefab 으로 저장

끝나면 프리팹의 컴포넌트를 다시 읽어서 각 값이 실제로 들어갔는지 보여줘.
```

**결과**: 구현 성공 + 거리 계산,플레이어의 IDamageable,참조가 비면,첫 공격 문제 없음. 요청한데로 프리팹 절차 설명까지 문제 없었음.
**문제점**: 사람이 직접 확인해야하는 작업 이외 없음.
**배운 점**: 여러 단계를 거쳐가면서 만들어나감으로써 요청사항의 정확도 향상이 눈에 보임.
```


````
### 2-4. 웨이브 시스템
```
ScriptableObject 기반 웨이브 시스템을 만들어줘.

- WaveData (ScriptableObject): 적 프리팹, 스폰 수, 스폰 간격
  CreateAssetMenu 를 붙여서 에디터에서 만들 수 있게 할 것
- WaveSpawner (MonoBehaviour):
  - WaveData 리스트를 순서대로 진행
  - 현재 웨이브의 적이 전부 죽으면 3초 후 다음 웨이브
  - 스폰 위치는 SpawnPoints 오브젝트 하나를 [SerializeField]로 받아서
    그 자식 Transform들을 Awake에서 모아 쓰고, 그중 랜덤으로 고를 것.
    Transform 배열을 인스펙터에 노출하지 마
  - 플레이어 Transform을 [SerializeField]로 받아서,
    스폰한 적에게 2-3에서 만든 공개 메서드로 넘겨줄 것
  - 웨이브 시작 / 웨이브 클리어 / 전체 클리어를 C# 이벤트로 노출
  - 살아 있는 적 수는 스폰할 때 세고, 각 적의 사망 이벤트를 구독해서 줄일 것.
    매 프레임 FindObjectsOfType 으로 세지 마

파일 2개만: Assets/Scripts/Core/WaveData.cs, Assets/Scripts/Core/WaveSpawner.cs
오브젝트 풀링은 아직 넣지 마. 다음 단계에서 할 거야.
씬은 건드리지 말고 세팅 절차만 알려줘.
```
**결과**: 구현 성공 요청한데로 절차 설명까지 문제 없었음. 웨이브 및 적들 생성 정상.
**문제점**: 사람이 직접 확인해야하는 작업 이외 없음.
**배운 점**: 여러 단계를 거쳐가면서 직접 검증하는 부분이 있어 매우 확실하게 진행됨.
```
````
### 2-5. 오브젝트 풀링 리팩터링

<!-- "밖에서 보면 아무것도 안 바뀌었는가" 에 대한 본인 판정 -->
방금 만든 WaveSpawner에 오브젝트 풀링을 적용해줘.

- Instantiate/Destroy 를 풀 재사용으로 바꿀 것
- WaveSpawner 의 공개 API와 이벤트 시그니처는 그대로 유지
- 적이 죽으면 파괴하지 말고 비활성화해서 풀로 돌려보낼 것
- 풀에서 꺼낸 적은 체력과 상태가 초기화되어 있어야 한다
- 변경 전후 차이를 요약해서 설명해줘
```
**결과**: 정상적으로 완료 + 규약을 지켜가며 검증 과정 또한 확인시켜줌.
**문제점**: 사람이 직접 확인해야하는 작업 이외 없음.
**배운 점**: 규약을 지켜가며 클로드 스스로 직접 검증하는 부분이 있었고 어떤식으로 진행했는지 또한 설명하며 진행됨.
```
### 2-6. 과제 — 원거리 적 또는 돌진 적

---
원거리 적(RangedEnemy)을 추가해줘. 아래 순서대로, 단계마다 컴파일 에러를 확인하고 넘어가. 막히면 멈추고 알려줘.
풀링도 그대로 동작해야 하니, RangedEnemyAI 는 EnemyAI 를 상속한다. 근접 적 동작은 달라지면 안 된다.
프리팹 (MCP 도구로 직접)
- Projectile: Sphere, Scale 0.3, Collider 제거(플레이어의 Ray 가 막히면 안 된다), Projectile 컴포넌트,
  형광 초록 URP Lit 머티리얼 Projectile_Green → Assets/Prefabs/Projectile.prefab
- RangedEnemy: Enemy.prefab 을 복제(원본 유지) → Assets/Prefabs/RangedEnemy.prefab
  EnemyAI 제거 후 RangedEnemyAI 추가, NavMeshAgent Speed 3 / Stopping Distance 0, Health 20,
  보라색 URP Lit 머티리얼 RangedEnemy_Purple, 투사체 칸에 Projectile.prefab, 플레이어 칸은 비움
- 씬에 임시로 만든 오브젝트는 정리하고, 남은 게 있으면 알려줘

마지막 임시 테스트
- 씬의 기존 임시 Enemy 를 지우고, RangedEnemy 인스턴스를 (0,1,15) 에 놓고 플레이어 칸에 Player 연결
**결과**: 정상적으로 완료
**문제점**: 사람이 직접 확인해야하는 작업 이외 없음.
**배운 점**: 어떤식으로 진행했는지 또한 설명하며 진행됨.

## 3장

### 3-1. 게임 상태 머신

```
게임 상태 머신을 만들어줘.

- 상태: Ready / Playing / GameOver / Cleared — enum + switch로 단순하게
- Ready에서 시작해 잠깐 뒤 Playing으로 (대기 시간은 인스펙터에서 조절)
- 플레이어 Health의 사망 이벤트를 구독해서 GameOver로
- WaveSpawner의 전체 클리어 이벤트를 구독해서 Cleared로
- GameOver / Cleared 에서: 커서 표시, Time.timeScale = 0
- 그 상태에서 R키를 누르면 현재 씬을 다시 로드
  (Time.timeScale을 1로 되돌리는 것을 잊지 말 것)
- 상태가 바뀌면 C# 이벤트로 알릴 것 — HUD가 구독할 것이다
- 구독한 이벤트는 OnDisable에서 반드시 구독 해제할 것
- GameOver / Cleared 로 갈 때 플레이어 조작을 멈출 것.
  timeScale = 0 은 Update를 멈추지 않아서, 안 끄면 죽은 뒤에도 총이 나간다
  PlayerController 와 WeaponSystem 을 각각 [SerializeField] 로 받아 enabled 를 끌 것.
  배열이나 리스트 하나로 묶지 마 — 타입이 넓어지면 인스펙터에서 엉뚱한 것도 들어간다
- Esc로 게임을 종료할 것 (Application.Quit). 어느 상태에서든 눌리게
- R키와 Esc는 InputSystem_Actions 에셋에 액션이 없다.
  WeaponSystem의 재장전처럼 코드에서 InputAction을 만들어 쓸 것

파일: Assets/Scripts/Core/GameManager.cs 하나만 생성.
WaveSpawner와 PlayerController는 수정하지 마.
씬은 건드리지 말고 세팅 절차만 알려줘.
```
GameManager 컴포넌트를 씬의 GameManager 오브젝트에 붙여줘.
(2장에 WaveSpawner를 붙인 그 오브젝트다)

- 플레이어 Health 참조 칸에 씬의 Player 를 연결
- WaveSpawner 참조 칸에 같은 오브젝트의 WaveSpawner 를 연결
- PlayerController 칸과 WeaponSystem 칸에도 씬의 Player 를 연결

끝나면 컴포넌트를 다시 읽어서 네 칸에 실제로 값이 들어갔는지 보여줘.
None 인 칸이 하나라도 있으면 알려줘.

Assets/Data 에 WaveData 에셋을 2개 더 만들어줘.

- Wave_4 : 적 12마리, 스폰 간격 0.5초
- Wave_5 : 적 16마리, 스폰 간격 0.4초
- 둘 다 적 프리팹은 Assets/Prefabs/Enemy.prefab

만든 뒤 GameManager 오브젝트의 WaveSpawner 웨이브 리스트 끝에 순서대로 추가해줘.
끝나면 리스트를 다시 읽어서 5개가 들어갔는지 보여줘.
```
**결과**: 정상적으로 완료 씬세팅 작업 정상완료
**문제점**: 사람이 직접 확인해야하는 작업 이외 없음.
**배운 점**: 0으로 만드는 위치가 EnterEndState의 맨 끝인 이유
1로 되돌리는 코드가 LoadScene보다 앞이어야 하는 이유 를 둘다 설명해줌.
````
### 3-2. HUD
````
HUD가 구독할 수 있도록 두 곳에 이벤트를 추가해줘.

- WeaponSystem: 탄약이 변할 때 (현재, 최대) 를 알리는 이벤트.
  기존 Debug.Log는 지울 것 — 이제 화면에 나온다
- WaveSpawner: 적이 죽을 때마다 누적 킬 수를 알리는 이벤트.
  이미 각 적의 사망 이벤트를 구독하고 있으니 거기서 세면 된다

기존 기능과 공개 이벤트 시그니처는 그대로 유지할 것.
파일 2개만 수정하고 다른 파일은 건드리지 마.
````

```
git diff Assets/Scripts/Player/WeaponSystem.cs Assets/Scripts/Core/WaveSpawner.cs
HUD를 만들어줘. UI Toolkit 말고 uGUI로.
텍스트는 TextMeshPro 말고 레거시 UnityEngine.UI.Text 를 쓸 것.
(TMP를 처음 쓰면 임포트 창이 떠서 그동안 MCP가 멈춘다)

표시할 것:
- 좌하단: 체력바. 어두운 배경 Image 안에 빨간 Image를 넣고,
  그 빨간 Image의 RectTransform 앵커 폭으로 줄일 것.
  Image.fillAmount는 쓰지 마 — 이유는 아래에 적었다.
  Health의 HealthChanged 이벤트 구독
- 우하단: 탄약 "현재 / 최대". WeaponSystem의 탄약 이벤트 구독
- 상단 중앙: 웨이브 번호. WaveSpawner의 WaveStarted 이벤트 구독
- 상단 우측: 킬 수. WaveSpawner의 킬 이벤트 구독
- 화면 중앙: GameOver / Cleared 안내 문구와 "R키로 재시작".
  평소에는 숨기고 GameManager의 상태 이벤트로 켤 것

- Update()에서 폴링하지 말 것. 전부 이벤트 구독으로
- 구독은 OnEnable, 구독 해제는 OnDisable
- 참조는 전부 [SerializeField] 로 받을 것. Find 계열 금지

파일: Assets/Scripts/UI/HUDController.cs 하나만 생성.
UI 오브젝트는 아직 만들지 말고, 어떤 오브젝트가 필요한지 목록만 알려줘.

```
```
**결과**: 정상적으로 작업완료
**문제점**: 정상완료
```
### 3-3. 게임필

<!-- AI가 1번만 만들었는가, 3개를 다 만들었는가 -->

### 3-4. AI 코드 리뷰

<!-- 아래 표를 채울 것. 틀린 지적이 최소 1개는 있어야 한다 -->

| # | AI의 지적 | 내 판정 | 근거 |
| - | --------- | ------- | ---- |
| 1 |           |         |      |
| 2 |           |         |      |

### 3-5. 빌드

---

## 자체 점검

제출 전에 확인하세요.

```
□ 프롬프트를 요약하지 않고 전문 그대로 붙여넣었는가
□ 실패한 시도가 최소 3개 이상 기록되어 있는가
□ 각 실패마다 "왜 실패했는지" 분석이 있는가
□ 프롬프트를 어떻게 고쳤는지 비교할 수 있는가
```

> 실습 동안 한 번도 실패하지 않는 것은 불가능합니다.
> 실패 기록도 중요합니다.
