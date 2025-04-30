# 2D 아이소메트릭 멀티플레이 슈터

![게임플레이 미리보기](https://github.com/tadadosii/ImageStorage/blob/master/Isometric_Shooter_Study_v0.2_Gameplay.gif)

## 프로젝트 소개

유니티 2019.4.0f1로 개발된 사이파이 분위기의 2D 아이소메트릭 슈터 게임 프로토타입입니다. 이 프로젝트는 Tadadosi에 의해 개발되었으며, 25-1 단국대학교 오픈소스기초SW 수업의 프로젝트의 일환입니다.

## 주요 기능

- 키보드/마우스 및 게임패드 지원 듀얼 컨트롤 시스템
- 충전 가능한 레이저 무기 시스템
- 에프터이미지 효과가 있는 대시 기능
- 유니버설 렌더 파이프라인(URP)을 활용한 후처리 효과
- 유지보수 용이한 컴포넌트 기반 아키텍처

## 문서 링크

이 프로젝트에 대한 상세 정보는 다음 문서들을 참조하세요:

- [사용자 가이드](USER_GUIDE.md) - 설치 및 게임 플레이 방법
- [개발자 가이드](DEVELOPER_GUIDE.md) - 프로젝트 구조 및 주요 시스템 설명
- [설치 가이드](INSTALLATION.md) - 개발 환경 구성 방법

## 빠른 시작

### 게임 플레이
1. [Releases](https://github.com/tadadosii/2DTopDownIsometricShooterStudy/releases) 페이지에서 최신 빌드 다운로드
2. 압축 파일 해제 후 실행 파일 실행

### 개발
1. 저장소 복제: `git clone https://github.com/tadadosii/2DTopDownIsometricShooterStudy.git`
2. Unity 2019.4.0f1에서 프로젝트 열기
3. `Assets/Scenes/MainScene.unity` 씬 열기

## 주요 조작법

### 키보드 및 마우스
- **이동**: WASD 키
- **조준**: 마우스
- **발사**: 마우스 왼쪽 버튼
- **차지 샷**: 마우스 오른쪽 버튼 (누르고 있기)
- **대시**: 스페이스바
- **무기 전환**: Q/E 또는 마우스 휠

### 게임패드 (Xbox 레이아웃)
- **이동**: 왼쪽 스틱
- **조준**: 오른쪽 스틱
- **발사**: RT
- **차지 샷**: LT
- **대시**: RB
- **무기 전환**: A/B 버튼

## 스크린샷

<p align="center">
<img src="https://i.imgur.com/OfmTyZ6.png" width="30%">
<img src="https://i.imgur.com/Bpkg4dB.gif" width="30%">
<img src="https://i.imgur.com/DjfttAc.gif" width="30%">
</p>

## 기여 방법

1. 저장소 포크
2. 기능 브랜치 생성 (`git checkout -b feature/amazing-feature`)
3. 변경사항 커밋 (`git commit -m 'Add some amazing feature'`)
4. 브랜치 푸시 (`git push origin feature/amazing-feature`)
5. Pull Request 오픈

## 제작 정보

### 스크립트 크레딧
- [CameraShake](https://gist.github.com/ftvs/5822103) by ftvs on Github
- [Singleton pattern](https://github.com/UnityCommunity/UnitySingleton) MIT Licence @ Unity Community

### 사운드 크레딧
- 효과음: [Freesound.org](https://freesound.org)에서 제공
  - Short Laser Shots by [Emanuele_Correani](https://freesound.org/people/Emanuele_Correani/sounds/260155/) - CC-BY-3.0
  - Sci-Fi Force Field Impact 15 by [StormwaveAudio](https://freesound.org/people/StormwaveAudio/sounds/330629/) - CC-BY-3.0
  - Sci_FI_Weapon_01 by [ST303](https://freesound.org/people/ST303/sounds/338783/) - CC0 1.0
  - SciFi Gun - Mega Charge Cannon by [dpren](https://freesound.org/people/dpren/sounds/440147/) - CC0 1.0

### 음악 크레딧
- Azimutez by [Sci Fi Industries](https://freemusicarchive.org/music/Sci_Fi_Industries/Blame_the_Lord/01_sci_fi_industries_-_azimutez) - CC BY-NC-SA 3.0

## 지원 및 연락처

### 원본 프로젝트 개발자
- [Twitter @tadadosi](https://twitter.com/tadadosi)
- [Reddit u/tadadosi](https://www.reddit.com/user/tadadosi)

### 현재 프로젝트 관리자
- [GitHub @probablymayb](https://github.com/probablymayb) - 포크 프로젝트 관리자 및 문서화 담당

### 이슈 및 문의
- [GitHub Issues](https://github.com/probablymaybe/2DTopDownIsometricShooterStudy/issues) - 버그 리포트 및 기능 요청

## 라이센스

이 프로젝트는 MIT 라이센스 하에 제공됩니다. 자세한 내용은 [LICENSE](LICENSE) 파일을 참조하세요.

* MIT 라이센스는 코드와 프로젝트 설정에만 적용됩니다. 오디오 파일과 스프라이트에는 별도의 라이센스 조건이 적용됩니다.
* 스프라이트는 개인 사용에 한해 무료로 제공됩니다.
