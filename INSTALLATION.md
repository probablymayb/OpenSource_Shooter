# 설치 및 환경 구성 가이드 - 2D 아이소메트릭 슈터

이 문서는 2D 아이소메트릭 슈터 프로젝트를 위한 개발 환경 설정에 대한 상세한 안내를 제공합니다.

## 필수 소프트웨어

### Unity 설치

이 프로젝트는 **Unity 2019.4.0f1** 버전에서 개발되었으며, 호환성을 위해 이 버전을 사용하는 것이 권장됩니다.

1. [Unity Hub 다운로드 및 설치](https://unity.com/download)
2. Unity Hub를 열고 "Installs" 탭으로 이동
3. "Add" 버튼을 클릭하고 "Archive" 탭으로 이동
4. Unity 2019.4.0f1 버전을 찾아 다운로드
5. 다음 모듈 포함:
   - Universal Windows Platform Build Support
   - Android Build Support (선택사항)
   - iOS Build Support (선택사항)
   - WebGL Build Support (선택사항)

### 코드 에디터

Unity의 내장 편집기를 사용할 수 있지만, 다음 에디터 중 하나를 사용하는 것을 권장합니다:

- **Visual Studio 2019 이상**: Unity와의 통합이 잘 되어 있으며, 디버깅 기능 제공
- **Visual Studio Code**: 가볍고 빠른 에디터, Unity 확장 프로그램 설치 필요
- **Rider**: 유료 IDE이지만 강력한 리팩터링 및 코드 분석 도구 제공

## 프로젝트 설정

### GitHub에서 프로젝트 복제

```bash
# HTTPS 사용
git clone https://github.com/tadadosii/2DTopDownIsometricShooterStudy.git

# SSH 사용 (설정된 경우)
git clone git@github.com:tadadosii/2DTopDownIsometricShooterStudy.git
```

### Unity에서 프로젝트 열기

1. Unity Hub 실행
2. "Projects" 탭에서 "Open" 버튼 클릭
3. 복제한 프로젝트 폴더 선택
4. Unity 2019.4.0f1 버전이 선택되었는지 확인하고 프로젝트 열기

### 초기 설정 확인

프로젝트가 열리면 다음 사항을 확인하세요:

1. **플랫폼 설정**: 
   - `File > Build Settings`에서 대상 플랫폼이 올바르게 설정되었는지 확인
   - PC, Mac & Linux Standalone이 기본 설정

2. **그래픽 설정**:
   - `Edit > Project Settings > Graphics`에서 "Scriptable Render Pipeline Settings"가 "UniversalRenderPipelineAsset"으로 설정되었는지 확인

3. **입력 설정**:
   - `Edit > Project Settings > Input`에서 커스텀 입력 매핑 확인
   - 필수 입력 키: HorizontalAim, VerticalAim, Xbox360RightTrigger, Xbox360LeftTrigger

## 프로젝트 구조 이해

### 주요 폴더 구조

```
Assets/
├── Animations/         # 애니메이션 클립 및 컨트롤러
├── Audio/              # 효과음 및 음악 트랙
├── Materials/          # 쉐이더 재질 포함
├── Prefabs/            # 재사용 가능한 게임 오브젝트
│   ├── Player/         # 플레이어 및 관련 프리팹
│   ├── Weapons/        # 무기 프리팹
│   └── Environment/    # 환경 오브젝트 프리팹
├── Scenes/             # Unity 씬 파일
├── Scripts/            # C# 스크립트 폴더 (자세한 구조는 개발자 가이드 참조)
├── Shaders/            # 커스텀 쉐이더 파일
└── Sprites/            # 2D 스프라이트 에셋
```

## 개발 워크플로우

### 메인 씬 열기

프로젝트의 메인 씬 파일(`Assets/Scenes/MainScene.unity`)을 열어 게임의 기본 설정을 확인합니다.

### 에디터에서 게임 테스트

1. Unity 에디터에서 "Play" 버튼을 눌러 게임 테스트
2. "Game" 뷰에서 해상도 설정 확인 (기본: 1920x1080)
3. 프레임률 확인: `Stats` 토글 버튼 클릭(게임 뷰 상단)

### 스크립트 편집

1. 스크립트 파일을 두 번 클릭하여 설정된 코드 에디터에서 열기
2. C# 구문 및 Unity API 참조를 위해 [Unity 문서](https://docs.unity3d.com/) 활용

## 테스트 및 디버깅

### 디버그 기능 활성화

많은 스크립트에는 디버그 모드가 내장되어 있습니다:

1. 각 스크립트의 `debug` 변수를 `true`로 설정하여 활성화
2. `GUIHandler` 및 `GizmosHandler` 컴포넌트를 활용하여 시각적 디버깅
3. Unity 콘솔(`Window > General > Console`)에서 로그 확인

### 성능 모니터링

1. Profiler(`Window > Analysis > Profiler`)를 사용하여 CPU/GPU 성능 모니터링
2. Frame Debugger(`Window > Analysis > Frame Debugger`)로 렌더링 파이프라인 검사

## 빌드 및 배포

### 게임 빌드하기

1. `File > Build Settings` 메뉴 열기
2. 대상 플랫폼 선택
3. "Player Settings"에서 게임 이름, 회사 이름, 아이콘 및 기타 메타데이터 설정
4. "Build" 또는 "Build and Run" 버튼 클릭
5. 빌드 파일을 저장할 위치 선택

### 빌드 최적화

1. **파일 크기 최적화**:
   - `Edit > Project Settings > Player > Other Settings`에서 "Scripting Backend"를 IL2CPP로 설정 (보안 강화 및 성능 향상)
   - "API Compatibility Level"을 .NET Standard 2.0으로 설정

2. **성능 최적화**:
   - 스프라이트 아틀라스 사용 (`Window > 2D > Sprite Atlas`)
   - 텍스처 압축 설정 확인 (각 텍스처의 Inspector에서)
   - 오디오 포맷 최적화 (각 오디오 클립의 Inspector에서)

## 자주 발생하는 문제 및 해결 방법

### 에셋 관련 오류

1. **분홍색/보라색 텍스처 오류**:
   - 원인: 텍스처 파일 누락 또는 손상
   - 해결: 프로젝트를 다시 복제하거나 해당 에셋 재임포트

2. **스크립트 컴파일 오류**:
   - 원인: C# 문법 오류 또는 참조 누락
   - 해결: 콘솔 창에서 오류 메시지를 확인하고 문제 부분 수정

3. **씬 로드 오류**:
   - 원인: 누락된 프리팹 또는 스크립트 참조
   - 해결: 백업 씬 사용 또는 필수 에셋 확인 후 다시 로드

### Unity 버전 관련 문제

1. **다른 Unity 버전으로 프로젝트 열기**:
   - 주의: 프로젝트를 최신 버전으로 업그레이드하면 이전 버전과 호환되지 않을 수 있음
   - 권장: 항상 프로젝트 백업 후 업그레이드 시도

2. **패키지 호환성 문제**:
   - 현상: URP(Universal Render Pipeline) 관련 오류 발생
   - 해결: Package Manager에서 URP 패키지 버전 확인 및 업데이트

## 확장 및 수정 가이드

### 새로운 레벨 추가

1. 새 씬 생성: `File > New Scene`
2. 기존 환경 요소 복사 또는 프리팹 활용
3. 플레이어 프리팹 추가 및 필수 매니저 오브젝트 설정
4. Build Settings에 새 씬 추가

### 새로운 무기 추가

1. 기존 무기 프리팹 복제 및 수정
2. 새로운 무기 스크립트 생성 (Weapon 클래스 상속)
3. 무기 동작 구현 (PrimaryAction/SecondaryAction 메서드 재정의)
4. WeaponHandler의 weapons 배열에 새 무기 추가

### 시각적 효과 조정

1. 프로젝트에 포함된 쉐이더 그래프로 쉐이더 수정 (`Shaders` 폴더)
2. 포스트 프로세싱 프로필 조정 (`Assets/PostProcessing` 폴더)
3. 애프터이미지 효과 커스터마이징 (`AfterImageMaterials` 컴포넌트)

## 추가 리소스

### 유니티 학습 자료

- [Unity 공식 튜토리얼](https://learn.unity.com/)
- [Unity 문서](https://docs.unity3d.com/Manual/index.html)
- [Unity 포럼](https://forum.unity.com/)

### 2D 게임 개발 자료

- [Unity 2D 게임 개발 가이드](https://docs.unity3d.com/Manual/Unity2D.html)
- [Unity 2D 애니메이션 시스템](https://docs.unity3d.com/Packages/com.unity.2d.animation@6.0/manual/index.html)

### 쉐이더 그래프 관련 자료

- [URP 쉐이더 그래프 문서](https://docs.unity3d.com/Packages/com.unity.shadergraph@12.0/manual/index.html)
- [쉐이더 그래프 예제](https://github.com/Unity-Technologies/ShaderGraph-Examples)

## 연락처 및 지원

프로젝트 관련 질문이나 문제가 있는 경우:

- GitHub 이슈 페이지: [Issues](https://github.com/tadadosii/2DTopDownIsometricShooterStudy/issues)
- 개발자 연락처: [Twitter @tadadosi](https://twitter.com/tadadosi) 또는 [Reddit u/tadadosi](https://www.reddit.com/user/tadadosi)

## 라이센스 정보

이 프로젝트는 MIT 라이센스 하에 배포됩니다. 라이센스 조건에 대한 자세한 내용은 프로젝트 루트 디렉토리의 LICENSE 파일을 참조하세요.

MIT 라이센스는 코드와 프로젝트 설정에만 적용됩니다. 오디오 파일과 스프라이트에는 별도의 라이센스 조건이 적용됩니다.