# 2D 아이소메트릭 슈터 - 개발자 가이드

## 프로젝트 개요

이 문서는 Tadadosi 및 단국대학교 학생들이 개발한 2D 아이소메트릭 슈터 프로젝트를 이해, 수정 또는 기여하고자 하는 개발자를 위한 포괄적인 가이드입니다. 이 게임은 Unity(버전 2019.4.0f1)로 개발되었으며 C#을 사용하여 스크립팅되었습니다.

## 프로젝트 구조

이 프로젝트는 명확한 관심사 분리를 가진 모듈형 컴포넌트 기반 아키텍처를 따릅니다. 다음은 프로젝트 구조의 개요입니다:

```
Assets/
├── Animations/         # 애니메이션 클립 및 컨트롤러
├── Audio/              # 효과음 및 음악 트랙
├── Materials/          # 쉐이더 재질을 포함한 재질 에셋
├── Prefabs/            # 재사용 가능한 게임 오브젝트
├── Scenes/             # Unity 씬
├── Scripts/            # C# 스크립트 파일 (아래 상세 설명)
├── Shaders/            # 커스텀 쉐이더 파일
└── Sprites/            # 2D 스프라이트 에셋
```

### 스크립트 구조

Scripts 폴더는 게임을 구동하는 C# 코드를 포함하고 있습니다. 다음과 같이 구성되어 있습니다:

```
Scripts/
├── Animations/             # 애니메이션 처리 스크립트
├── Audio/                  # 오디오 시스템 스크립트
├── Camera/                 # 카메라 동작 및 효과
├── Crosshair/              # 조준 시스템
├── Debug/                  # 디버깅 유틸리티
├── Environment/            # 환경 오브젝트 및 상호작용
├── FX/                     # 애프터이미지를 포함한 시각 효과
│   └── AfterImage/         # 애프터이미지 효과 시스템
├── Global Managers/        # 게임 전체 관리 시스템
├── Inputs/                 # 커스텀 입력 처리
├── Physics/                # 물리 관련 컴포넌트
├── Player/                 # 플레이어 동작 및 제어
├── Utilities/              # 헬퍼 클래스 및 함수
│   ├── Arrays/             # 배열 조작 유틸리티
│   └── Rotation/           # 회전 처리 유틸리티
├── Weapons/                # 무기 시스템
│   ├── Animations/         # 무기 애니메이션 스크립트
│   └── Weapon Derived Classes/ # 특정 무기 구현
└── _NOTES/                 # 개발 노트
```

## 아키텍처 개요

게임은 단일 책임 원칙 접근 방식의 컴포넌트 기반 아키텍처를 따릅니다. 주요 시스템은 다음과 같습니다:

1. **입력 시스템**: 키보드/마우스 및 컨트롤러 입력을 모두 지원하는 커스텀 입력 핸들러(`TadaInput`)
2. **플레이어 시스템**: 플레이어 엔티티의 다양한 측면을 처리하는 모듈식 플레이어 컴포넌트
3. **무기 시스템**: 다양한 무기 유형에 대한 파생 구현이 있는 기본 무기 클래스
4. **오디오 시스템**: 로컬 및 글로벌 사운드 재생을 위한 매니저 및 이미터
5. **시각 효과 시스템**: 애프터이미지 및 기타 시각적 강화를 처리

## 핵심 시스템 상세 설명

### 1. 입력 시스템 (TadaInput)

`TadaInput` 클래스는 Unity의 내장 입력 시스템을 확장하여 커스텀 입력 처리를 제공합니다. 이 시스템은 키보드/마우스와 게임패드 제어를 모두 원활하게 지원합니다.

주요 특징:
- 커스텀 입력 매핑 및 열거형
- 키 상태 추적(누름/떼기/유지)
- 부드러운 입력 축 및 원시 입력 축 제공
- 마우스와 조이스틱 교차 호환성

```csharp
// 입력 사용 예시
if (TadaInput.GetKeyDown(TadaInput.ThisKey.Dash) && _PlayerPhysics.Velocity.sqrMagnitude > 0)
    _PlayerSkills.Dash();
```

### 2. 플레이어 시스템

플레이어 시스템은 다양한 기능으로 분리된 여러 클래스로 구성됩니다:

- **PlayerController**: 입력을 받아 다른 컴포넌트에 명령을 전달하는 중앙 컨트롤러
- **PlayerPhysics**: 이동 및 물리적 상호작용 처리
- **PlayerAnimations**: 플레이어 애니메이션 상태 관리
- **PlayerBodyPartsHandler**: 플레이어 신체 부위 시각적 방향 및 레이어 순서 관리
- **PlayerSkills**: 대시와 같은 특수 능력 구현
- **PlayerShoulderMain/Secondary**: 무기 조준 및 회전 처리

각 컴포넌트는 단일 책임을 가지며, 코드를 모듈화하고 유지 관리를 용이하게 합니다.

### 3. 무기 시스템

무기 시스템은 상속과 다형성의 좋은 예입니다:

- **Weapon**: 모든 무기의 기본 클래스로, 일반적인 동작과 메서드를 정의
- 파생 무기 클래스: 
  - **Weapon_ShootProjectileCanCharge**: 충전 가능한 레이저 샷을 발사하는 무기
  - **Weapon_ChargeContinuousShooting**: 지속적인 레이저 빔을 발사하는 무기

**WeaponHandler**는 무기 전환과 무기 액션 발동을 관리하며 플레이어 컨트롤러와 실제 무기 클래스 사이의 중개자 역할을 합니다.

```csharp
// 무기 전환 예시
public void SwitchWeapon(WeaponSwitchMode mode, int index = 0)
{
    CheckWeaponsAvailability();
    DisableAllWeapons();
    
    switch (mode)
    {
        case WeaponSwitchMode.Next:
            currentWeaponIndex = ArraysHandler.GetNextIndex(currentWeaponIndex, weapons.Length);
            break;
        // 다른 케이스...
    }
    
    currentWeapon = weapons[currentWeaponIndex];
    currentWeapon.gameObject.SetActive(true);
}
```

### 4. 오디오 시스템

오디오 시스템은 두 가지 주요 접근 방식으로 설계되었습니다:

1. **전역 사운드(SoundManager)**: 싱글톤 패턴을 사용하여 모든 곳에서 접근할 수 있는 전역 오디오 소스 제공
2. **로컬 사운드(SoundHandlerLocal)**: 개별 게임 오브젝트에 부착되는 로컬 오디오 소스 관리

이 이중 접근 방식은 UI 사운드와 같은 전역 사운드와 무기 발사와 같은 공간적 사운드를 모두 효과적으로 처리합니다.

### 5. 시각 효과 시스템

애프터이미지 시스템은 플레이어가 대시할 때 시각적 잔상을 생성하는 포괄적인 예시입니다:

- **AfterImageGenerator**: 초기 설정 및 애프터이미지 생성
- **AfterImage**: 개별 애프터이미지의 동작 처리
- **AfterImageGroup**: 애프터이미지 그룹 관리
- **AfterImageHandler**: 모든 애프터이미지 그룹 관리 및 활성화

이 접근 방식은 유지 관리가 용이하고 확장 가능한 방식으로 복잡한 시각 효과를 구현합니다.

## 확장 가이드

### 새 무기 추가하기

새 무기를 추가하려면:

1. `Weapon` 클래스를 상속받는 새 스크립트 생성
2. 필요에 따라 `PrimaryAction()` 및 `SecondaryAction()` 메서드 재정의
3. 무기에 필요한 애니메이션 및 시각적 요소 구현
4. `WeaponHandler`의 무기 배열에 새 무기 추가

```csharp
public class Weapon_NewType : Weapon
{
    // 특정 무기에 필요한 변수

    protected override void Awake()
    {
        base.Awake();
        useRateValues = new float[] { 0.2f, 0.1f }; // 다양한 발사 속도 설정
    }

    public override void PrimaryAction(bool value)
    {
        base.PrimaryAction(value);
        // 사용자 정의 주 액션 구현
    }

    public override void SecondaryAction(bool value)
    {
        base.SecondaryAction(value);
        // 사용자 정의 보조 액션 구현
    }
}
```

### 새로운 플레이어 능력 추가하기

새 플레이어 능력을 추가하려면:

1. `PlayerSkills` 클래스에 새 메서드 추가
2. 필요한 컴포넌트 참조 설정
3. `PlayerController`에서 적절한 입력 바인딩 추가

```csharp
// PlayerSkills.cs에 새 능력 추가
public void TeleportAbility()
{
    if (canTeleport)
    {
        StartCoroutine(CO_Teleport());
    }
}

private IEnumerator CO_Teleport()
{
    canTeleport = false;
    
    // 텔레포트 효과 구현
    Vector3 targetPosition = transform.position + TadaInput.AimDirection * teleportDistance;
    transform.position = targetPosition;
    
    yield return new WaitForSeconds(teleportCooldown);
    canTeleport = true;
}

// PlayerController.cs에 바인딩 추가
if (TadaInput.GetKeyDown(TadaInput.ThisKey.Teleport))
    _PlayerSkills.TeleportAbility();
```

### 입력 시스템 확장하기

새 입력을 추가하려면:

1. `TadaInput.ThisKey` 열거형에 새 키 유형 추가
2. `Update()` 메서드에 입력 감지 코드 추가
3. 관련 메서드에서 새 입력 사용

```csharp
// TadaInput.cs에 새 키 추가
public enum ThisKey
{ 
    // 기존 키...
    Teleport,
    // 새 키...
    Count
}

// Update()에 감지 코드 추가
if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.Joystick1Button3))
    StoreCurrentKeyDown(ThisKey.Teleport);
```

## 성능 최적화 팁

1. **오브젝트 풀링 구현**: 프로젝타일과 효과에 대해 Instantiate/Destroy 대신 오브젝트 풀링 사용
2. **가비지 컬렉션 최소화**: 업데이트 루프에서 새 할당 피하기
3. **DrawCall 최적화**: 스프라이트 아틀라스 사용

## 프로젝트 빌드 및 배포

### 빌드 프로세스

1. Unity 에디터에서 `File > Build Settings` 열기
2. 대상 플랫폼 선택 (Windows, macOS, Linux)
3. 씬 추가 (메인 씬이 포함되어 있는지 확인)
4. 플레이어 설정에서 제품 이름, 회사 이름, 아이콘 등의 세부 정보 구성
5. "Build" 또는 "Build and Run" 클릭

### 배포 체크리스트

- README 및 라이센스 파일이 최신 상태인지 확인
- 모든 에셋이 올바르게 포함되어 있는지 확인
- GitHub Releases를 통해 빌드 배포

## 디버깅 팁

1. **GUIHandler 및 GizmosHandler 사용**: 디버그 정보를 시각화하기 위한 내장 유틸리티 활용
2. **디버그 플래그 활성화**: 각 클래스에 있는 디버그 플래그를 활성화하여 특정 시스템에 대한 로그 확인
3. **VS Code 또는 Visual Studio**: 중단점 및 변수 검사를 위한 통합 디버거 사용

## 알려진 이슈 및 향후 개선사항

1. **오브젝트 풀링**: 프로젝타일 및 효과에 대한 오브젝트 풀링 시스템 구현
2. **입력 시스템**: 런타임에 키 리매핑 지원 추가
3. **에너미 AI**: 에너미 시스템 및 AI 구현
4. **레벨 생성 시스템**: 수동 레벨 구성 대신 레벨 생성 시스템 개발

## 데이터 흐름 다이어그램

```
┌──────────────┐     ┌────────────────┐     ┌────────────────┐
│   TadaInput  │────▶│PlayerController│────▶│  PlayerPhysics │
└──────────────┘     └────────┬───────┘     └────────────────┘
                              │
                              ▼
┌──────────────┐     ┌────────────────┐     ┌────────────────┐
│ WeaponHandler│◀────│PlayerController│────▶│   PlayerSkills │
└───────┬──────┘     └────────┬───────┘     └────────────────┘
        │                     │
        ▼                     ▼
┌──────────────┐     ┌────────────────┐     ┌────────────────┐
│    Weapon    │     │PlayerAnimations│     │AfterImageHandler│
└──────────────┘     └────────────────┘     └────────────────┘
```

## 추가 리소스 및 참고자료

- [Unity 공식 문서](https://docs.unity3d.com/)
- [C# 프로그래밍 가이드](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/)
- [게임 디자인 패턴](https://gameprogrammingpatterns.com/)
- [GitHub 저장소](https://github.com/tadadosii/2DTopDownIsometricShooterStudy)

## 기여 가이드라인

1. GitHub에서 저장소 포크
2. 새 브랜치 생성 (`git checkout -b feature/amazing-feature`)
3. 변경사항 커밋 (`git commit -m 'Add some amazing feature'`)
4. 브랜치 푸시 (`git push origin feature/amazing-feature`)
5. Pull Request 오픈

코드 스타일, 테스트 요구사항 및 PR 프로세스에 대한 추가 정보는 저장소의 CONTRIBUTING.md 파일을 참조하세요.

## 커밋 규칙

### 커밋 메시지 기본 형식
```
[타입]: 제목 (50자 이내)
본문 (선택 사항, 72자마다 줄바꿈)
각주 (선택 사항)
```
### 타입 종류
- `feat`: 새로운 기능 추가
- `fix`: 버그 수정
- `docs`: 문서 수정
- `style`: 코드 포맷팅, 세미콜론 누락, 코드 변경이 없는 경우
- `refactor`: 코드 리팩토링
- `test`: 테스트 코드 추가
- `chore`: 빌드 업무 수정, 패키지 매니저 설정 등
### 좋은 커밋 메시지 예시
```
[feat]: 플레이어 점프 기능 추가
- Space 키로 점프 가능
- 점프 높이를 조절할 수 있는 변수 추가
- 점프 중에는 이동 속도 감소
```
### 커밋 규칙
1. 너무 큰 단위로 커밋하지 않기 (너무 많은 기능)
2. **작업 단위별로 자주 커밋하기**
3. **테스트 완료 후 커밋하기**
4. 설명 자세할수록 좋음

## 풀 리퀘스트(PR)

### PR 제목 형식
```
[타입] 간결한 변경 내용 설명 (YYYY-MM-DD, 요일)
```
### PR 본문 템플릿
```
## 작성 날짜
YYYY-MM-DD
## 변경 사항
여기에 변경 사항을 자세히 적어주세요.
```
### PR 규칙
1. PR을 보내기 테스트하기
2. 너무 큰 단위로 PR하지 않기
3. 코드에 주석 잘 달기

## 라이센스

이 프로젝트는 MIT 라이센스 하에 제공됩니다. 이는 코드와 프로젝트 설정에만 적용됩니다. 오디오 파일과 스프라이트에는 별도의 라이센스 조건이 적용됩니다.