# SW 오픈소스 기반 멀티플레이어 슈팅게임 개발자 가이드

## 목차

- [1. 게임 실행 및 테스트 시 유의사항](#1-게임-실행-및-테스트-시-유의사항)
- [2. GameManager 구성 및 매니저 구조](#2-gamemanager-구성-및-매니저-구조)
- [3. UI 관련 가이드 (MVP패턴, UIManager)](#3-ui-관련-가이드-mvp패턴-uimanager)
- [4. ResourceManager 가이드](#4-resourcemanager-가이드)
- [5. PhotonManager 가이드 (멀티플레이 시스템)](#5-photonmanager-가이드-멀티플레이-시스템)
- [6. Photon 사용 가이드 (네트워크 동기화 및 RPC)](#6-photon-사용-가이드-네트워크-동기화-및-rpc)
- [7. 애니메이션 시스템 가이드](#7-애니메이션-시스템-가이드)
- [8. 무기 시스템 전용 리소스 로딩 함수 가이드 (Weaponcs)](#8-무기-시스템-전용-리소스-로딩-함수-가이드-weaponcs)
- [9. 신규 무기 기초 클래스: ShotGun](#9-신규-무기-기초-클래스-shotgun)
- [10. 아이템 무기 교체 및 상호작용 시스템](#10-아이템-무기-교체-및-상호작용-시스템)


## 1. 게임 실행 및 테스트 시 유의사항

* 본 프로젝트는 **멀티플레이 게임**이므로, **두 명 이상의 플레이어가 동시에 접속**해야 기능을 테스트할 수 있습니다.
* 가장 간편한 테스트 방법:

  * 하나는 Build(Build Settings → Build) 하여 실행 (.exe 또는 Android APK 등)
  * 다른 하나는 **에디터(Play 모드)** 에서 실행
* 게임 시작 시 **반드시 LobbyScene부터 실행**해야 Photon 서버와 정상적으로 연결됩니다.

## 2. GameManager 구성 및 매니저 구조

`GameManager`는 프로젝트 내 모든 매니저들의 중앙 초기화 및 릴리즈, 싱글톤 접근 방식을 제공합니다.

```csharp
public static DataManager Data { get; } = new DataManager();

var managers = new List<IManager>
{
    UI,
    Scene,
    Resource,
    Photon,
    Data,
};

private static void Relese()
{
    Scene?.Release();
    UI?.Release();
    Resource?.Release();
    Photon?.Release();
    Data?.Release();
}
```

* 각 매니저는 `static`으로 선언되므로 `GameManager.Data.기능()`처럼 접근합니다.
* 씬 전환 시에도 유지되며 전역에서 재사용 가능합니다.

## 3. UI 관련 가이드 (MVP패턴, UIManager)

확장성과 유지보수성을 고려하여 **MVP (Model-View-Presenter) 패턴**을 UI 시스템에 적용

#### 구성 요소 역할

| 구성 요소     | 설명                                                                 |
|--------------|----------------------------------------------------------------------|
| `ModelBase`  | UI 상태 데이터 보관. DTO 기반 이벤트를 발생시켜 Presenter에 변경사항을 전달함 |
| `ViewBase<T>`| 사용자 입력(UI 요소)을 Presenter로 전달하고, Presenter로부터 UI 갱신 요청을 받음 |
| `PresenterBase<T>` | View와 Model을 연결하는 중재자. 이벤트 등록, 데이터 전달, View 업데이트를 담당 |

이 구조를 통해 다음과 같은 효과를 얻을 수 있다:

- UI 로직과 상태 데이터의 분리 → 유지보수 용이
- Presenter를 통해 외부에서 UI 제어 → 명확한 책임 구분
- 타입 안전한 이벤트 흐름 → 컴파일 타임에서 오류 방지

---
#### PresenterBase 주요 메서드

| 메서드 정의 | 설명 |
|-------------|------|
| `AddListener<TDto>(Enum, UnityAction<TDto>)` | 특정 이벤트에 DTO 기반 리스너 등록 |
| `TriggerEvent<TDto>(Enum, TDto dto)`         | DTO 전달과 함께 이벤트 발생 |
| `TriggerEvent(Enum)`                         | 매개변수 없는 이벤트 발생 (Unit.Default 사용) |

---

#### ViewBase 주요 메서드

| 메서드 정의 | 설명 |
|-------------|------|
| `AddUIListener<T, TDto>(uiElement, eventEnum, callback)` | UI 요소(Button, Slider 등)와 이벤트 연결 |
| `UpdateView(TModel model)`  | 전체 View 갱신 (모델 초기 상태 반영) |
| `UpdateView<TDto>(TDto dto)`| 부분 View 갱신 (DTO 이벤트 기반) |

---

#### ModelBase 주요 메서드

| 메서드 정의 | 설명 |
|-------------|------|
| `TriggerEvent<TDto>(Enum, TDto dto)`         | DTO 전달과 함께 이벤트 발생 |
| `AddListener<TDto>(Enum, UnityAction<TDto>)` | Presenter 또는 View로부터 DTO 기반 리스너 등록 |
| `ClearListener()`                            | 모든 리스너 초기화 (씬 전환 등) |

---
#### 간단 예시 - 볼륨 조절
```csharp
// View: 슬라이더 값 변경 시 presenter에게 이벤트 전달
AddUIListener<Slider, float>(bgmSlider, SettingEvent.OnVolumeChange, (evt, val) => {
    presenter.TriggerEvent(evt, new VolumeSettingDto(val, sfxSlider.value));
});

// Presenter: 이벤트 등록
AddListener<VolumeSettingDto>(SettingEvent.OnVolumeChange, dto => {
    model.SetVolume(dto);
});

model.AddListener<VolumeSettingDto>(SettingEvent.OnVolumeChange, dto => {
    view.UpdateView(dto);
});

// Model: 값 저장 및 이벤트 트리거
public void SetVolume(VolumeSettingDto dto)
{
    currentVolume = dto;
    TriggerEvent(SettingEvent.OnVolumeChange, currentVolume);
}

```
---
* `UIManager`는 전체 Presenter를 등록/조회하고, 팝업을 스택 기반으로 생성, 표시, 숨김 처리하는 중심 역할을 한다.

| 메서드                              | 설명                                                |
|-----------------------------------|-----------------------------------------------------|
| `FindAllPresenter()`              | 현재 씬에 존재하는 모든 Presenter 수집              |
| `ShowPopup(string)`              | 팝업 비동기 생성 및 표시 (재사용 포함)              |
| `ShowPopup<T>(string)`           | 생성과 동시에 제네릭 Presenter 반환                 |
| `HidePopup()`                    | 가장 최근의 팝업을 숨김                             |
| `HidePopup(IPresenter)`         | 특정 팝업 인스턴스를 숨김                           |
| `GetScenePresenter<T>(string)`   | 특정 Presenter 반환                                 |
| `GetPopupPresenter(string)`      | 현재 팝업 스택에 존재하는 특정 이름의 팝업 반환      |

```csharp
void GetPlayerInput()
{
    if (Input.GetKeyDown(KeyCode.Tab))
    {
        var popup = GameManager.UI.IsPopupStack(matchPopupName);
        if (popup != null)
            GameManager.UI.HidePopup(popup);
        else
            GameManager.UI.ShowPopup(matchPopupName).Forget();
    }
}
```

### 연동 조건

* 팝업 프리팹은 ResourceManager를 통해 로드 가능해야 함
* 최소한 `ShowView()`, `HideView()` 메서드를 구현해야 함

### 추후 확장 가능 요소

* PopupType 기반 분기 확장

## 4. ResourceManager 가이드

`Unity Addressables` 기반으로 비동기 방식 리소스 로딩/캐싱을 지원합니다.

### 주요 기능

* 주소 기반 GameObject 비동기 로딩
* 이름 정리하여 캐시 키 일관성 유지
* 특정 컴포넌트 단위 로딩 지원
* 내부 핸들 캐싱

```csharp
private readonly Dictionary<string, AsyncOperationHandle<GameObject>> _instantiateResource;

async UniTask<GameObject> Instantiate(string address, Transform parent = null);
async UniTask<T> Instantiate<T>(string address, Transform parent = null) where T : class;
```

예시:

```csharp
GameObject avatar = await GameManager.Resource.Instantiate("PlayerInfo", parent);
return await GameManager.Resource.Instantiate<IPopupPresenter>(popupName, container);
```

## 5. PhotonManager 가이드 (멀티플레이 시스템)

`PhotonManager`는 서버 연결, 매칭, 룸 생성, 씬 전환 등의 상위 네트워크 로직을 관리합니다.

### 구성 구조

* 상위 매니저: `PhotonManager`

  * 서버 연결, 매칭, GamePhase에 따라 씬 전환
  * 각 씬별 `IScenePhotonManager` 초기화
* 하위 매니저:

  * `WaitingRoomPhotonManager`: 대기방 입장/준비 상태 처리
  * `InGamePhotonManager`: 인게임 조작, 승패, 캐릭터 동기화 등 처리

### 기본 흐름

```text
ConnectToPhoton()
→ OnConnectedToMaster() → JoinLobby()
→ OnJoinedLobby() → StartRandomMatch()
→ JoinRandomRoom() → OnJoinRandomFailed() → CreateRoom()
→ OnJoinedRoom() → 씬 전환
```

## 6. Photon 사용 가이드 (네트워크 동기화 및 RPC)

### 필수 조건

* 동기화 대상 오브젝트에는 반드시 **PhotonView 컴포넌트**가 있어야 함
* `photonView.IsMine` 체크를 통해 **로컬 플레이어만 조작 가능**하도록 처리

```csharp
void Update()
{
    if (!photonView.IsMine) return; // 내 캐릭터만 조작

    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");
    transform.Translate(new Vector3(h, 0, v));
}
```

### 동기화 예시: 탄환 발사

```csharp
GameObject bullet = PhotonNetwork.Instantiate("Projectiles/Bullet", spawnPos, rotation);
bullet.GetComponent<Projectile>().isRPCFire = true;
```

* 로컬에서만 Instantiate 가능하도록 분기 처리
* 탄환이 이동 중일 때, 위치는 `photonView.IsMine`인 경우만 이동시키고 그 외에는 동기화 대기

### RPC 사용 예시

```csharp
[PunRPC]
void RPC_Fire(float angle, bool isRight)
{
    SetActive(true);
    hasLaunched = true;
    transform.rotation = Quaternion.Euler(0, 0, angle);
    // 기타 이펙트 실행
}
```

* RPC는 `photonView.RPC("RPC_Fire", RpcTarget.All, angle, isRight);` 형식으로 호출

### Animator/Transform 동기화 가이드

* **움직임 및 애니메이션 동기화**를 위해 PhotonView의 `Observed Components`에 `Transform`, `Animator`를 포함시켜야 함
* `Animator`가 없으면 Mecanim 기반 동기화가 불가능하므로, **애니메이션 시스템은 반드시 Mecanim(Animator Controller) 기반으로 구현**해야 함

### 직접 동기화 방식: OnPhotonSerializeView

* PhotonStream을 이용해 좌우 방향, 각도 등 개별 데이터를 직접 보내는 방식으로 동기화할 수 있습니다.

```csharp
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        bool isLeftDirection = crosshairMouse.AimDirection.x < 0.1f;
        stream.SendNext(isLeftDirection);
    }
    else
    {
        bool isLeftDirection = (bool)stream.ReceiveNext();
        Debug.Log("isLeftDirection: " + isLeftDirection);

        isRightDirection = isLeftDirection;
        SetBodyPartsDirection(isLeftDirection ? Direction.Left : Direction.Right);
    }
}
```

* 주로 캐릭터의 회전 방향, 부위 상태, 에임 방향 등을 실시간으로 다른 플레이어에게 전달할 때 사용됩니다.

## 7. 애니메이션 시스템 가이드

기존 오픈소스는 `Animation Clip`만 사용하는 방식이었으나, 본 프로젝트에서는 **Photon과의 호환을 위해 ****`Animator Controller`**** 기반의 Mecanim 구조로 변경**하였습니다.

### 주요 구현 사항

* 각 무기마다 전용 Animator Controller 생성 (`Idle`, `BasicShot`, `StrongShot` 등 상태 정의)
* `animator.SetInteger("WeaponAction", ...)` 방식으로 상태 전이 구현
* `Animator` 컴포넌트는 PhotonView의 Observed Components에 포함하여 네트워크 애니메이션 동기화 가능

이를 통해 멀티플레이 환경에서도 **모든 사용자에게 일관된 애니메이션 재생**이 가능해졌으며, 커스텀 무기 동작 및 피격/발사 이펙트도 정확히 동기화됩니다.

> 본 가이드는 멀티플레이 기반의 슈팅 게임에서 발생하는 전반적인 시스템 구성과 Photon 환경에서의 동기화, 입력 분기, 애니메이션 처리 등을 중심으로 작성되었으며, 추후 팀 프로젝트나 상용 개발 환경에서도 확장 가능한 구조로 구성되어 있습니다.

## 8. 무기 시스템 전용 리소스 로딩 함수 가이드 (Weapon.cs)

### 개요

`Weapon.cs`는 각 무기별로 필요한 스프라이트, 프리팹, 오디오 클립 등을 빠르게 로드하기 위해 자체적인 리소스 로딩 함수를 포함하고 있습니다. 이 방식은 범용 리소스 매니저를 사용하지 않고, **무기 시스템처럼 제한된 범위의 클래스에서만 사용하는 리소스를 간단히 로드할 수 있도록 설계**되었습니다.

> ※ 범용 리소스 로딩이 필요한 경우에는 `ResourceManager` + `AddressableAssets` 구조 사용을 권장합니다.
> 단, 무기 시스템처럼 **한 클래스에 한정된 리소스만 필요한 경우**, 현재 구조처럼 `Resources.Load()` 기반의 직접 로딩 방식이 유리할 수 있습니다.

---

### 제공되는 함수 목록

| 메서드명                                   | 설명                                            |
| -------------------------------------- | --------------------------------------------- |
| `getProjPref(string)`                  | 탄환 프리팹 로드 (`Resources/Projectiles/`)          |
| `getAudioClip(string)`                 | 무기 발사 오디오 로드 (`Resources/Audio/SFX/Weapons/`) |
| `getAnimator()`                        | 자기 자신 또는 부모에서 `Animator` 탐색                   |
| `getSpriteSheet(string)`               | 스프라이트 시트 전체 로드 (`Resources/Sprites/`)         |
| `getSpriteFromSheet(Sprite[], string)` | 시트에서 특정 스프라이트 추출                              |
| `getObject(string)`                    | 경로 기반 GameObject 로드                           |
| `createPSp(...)`                       | Projectile 발사 위치용 Transform 생성                |
| `createPFX(...)`                       | 파티클 프리팹 인스턴스화 및 비활성화 상태로 리턴                   |
| `createSpriteObject(...)`              | 스프라이트가 포함된 GameObject를 생성하고 원하는 위치에 배치        |

---

### 사용 예시

```csharp
// 무기 초기화 시
basicProjectilePrefab = getProjPref("pfab_Bullet_Laser_Small_Cyan");
strongProjectilePrefab = getProjPref("pfab_Bullet_Laser_MidSize_Blue");

AudioClip fireSFX = getAudioClip("shotgun_fire.wav");

Sprite[] sprites = getSpriteSheet("Weapons/Weapons_001");
Sprite muzzleSprite = getSpriteFromSheet(sprites, "LaserRifle_Muzzle");

createSpriteObject("Muzzle", muzzleSprite, 10, this, position: new Vector3(0.12f, 0, 0));
```

---

### ResourceManager 방식과의 비교

| 항목     | Weapon 전용 방식       | ResourceManager + Addressables |
| ------ | ------------------ | ------------------------------ |
| 사용 범위  | 단일 클래스 (ex. 무기)    | 전체 게임 범위 (UI, 캐릭터 등)           |
| 리소스 경로 | 코드에 하드코딩           | Addressables 주소 기반             |
| 캐싱     | 수동 관리              | 자동 캐싱 및 해제 가능                  |
| 비동기 지원 | ❌ (Resources.Load) | ✅ UniTask 기반 비동기 처리            |
| 장점     | 빠르고 직관적, 의존성 없음    | 대규모 프로젝트에 적합, 재사용성 높음          |

---

### ✅ 가이드라인

* **무기 하나에서만 사용하는 리소스**: `Weapon.cs` 내부 함수 사용 (빠르고 단순함)
* **여러 시스템에서 공유되는 리소스**: `ResourceManager`와 Addressables를 통해 관리 (확장성과 유지보수 우수)

---

### 확장 시 참고 사항

* 이 함수들은 모두 `protected`로 선언되어 있어, 하위 무기 클래스에서 바로 사용 가능
* `Resources.Load()` 기반이므로 경로 오타 시 null 반환 → 반드시 예외 처리 필요
* 유지보수 중 Addressables로 전환하려면 `getObject()` → `GameManager.Resource.Instantiate()` 방식으로 변경 가능

---

> 이 구조는 무기처럼 리소스의 범위가 명확하고 클래스에 국한되어 있는 경우 매우 적합합니다. 반면 UI, 캐릭터, 효과 등 다양한 시스템에서 참조되는 리소스는 ResourceManager로 관리하는 것이 이상적입니다.

## 9. 신규 무기 기초 클래스: ShotGun

### 개요

`Weapon_ShotGun`은 기존 무기 시스템(`Weapon`)을 상속하여, **한 번에 여러 발의 탄환을 퍼뜨리는 샷건 계열 무기**를 구현하기 위한 기초 클래스입니다. 기본 공격/강화 공격 모두 다탄 발사를 지원하며, 발사 간격과 탄환 개수 설정이 각각 분리되어 있습니다.

`FINAL_Weapon_LaserShotGun`은 이를 상속하여 실제 구현된 샷건 무기로, 스프라이트 설정 및 발사 위치, 탄환 프리팹을 초기화합니다.

---

### 기본 동작 사양

| 공격 유형 | 발사 탄환 수 | 발사 간격 (초) |
|-----------|--------------|----------------|
| 일반 공격 | 4발          | 0.5초          |
| 강화 공격 | 8발          | 1.0초          |

- 각 탄환은 랜덤 회전 각도와 속도를 적용하여 퍼지는 형태로 발사됨

---

### 무기 발사 함수

```csharp
protected override void PrimaryFire(bool isRight)
{
    for (int i = 0; i < 4; i++)
    {
        GameObject bullet = PhotonNetwork.Instantiate("Projectiles/" + basicProjectilePrefab.name,
            projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        Projectile proj = bullet.GetComponent<Projectile>();
        proj.Speed = 20f * Random.Range(0.9f, 1.1f);
        proj.setVolume(2f / 3);
        proj.isRPCFire = (PhotonNetwork.InRoom && PhotonManager._currentPhase == PhotonManager.GamePhase.InGame);
        proj.Fire(Random.Range(-10f, 10f), isRight);
    }
}
```

## 10. 아이템 무기 교체 및 상호작용 시스템

### 개요

본 시스템은 플레이어가 **F 키를 눌러 바닥에 떨어진 무기 아이템과 현재 장비 중인 무기를 교체할 수 있는 기능**을 제공합니다.
기존 오픈소스 구조에 포함된 `TadaInput`, `PlayerController`, `WeaponHandler` 클래스들을 그대로 활용하여 구현됩니다.

### 키 입력 처리 (TadaInput)

F 키는 `TadaInput.ThisKey.GetItem`으로 지정하여 등록할 수 있습니다.

```csharp
if (Input.GetKeyDown(KeyCode.F))
    StoreCurrentKeyDown(ThisKey.GetItem);
```

등록된 키는 입력이 필요한 위치 (예: `PlayerController`)에서 다음과 같이 사용합니다:

```csharp
if (TadaInput.GetKeyDown(TadaInput.ThisKey.GetItem))
{
    GameObject obj = _PlayerPhysics.getCollidedObject();
    if (obj != null)
    {
        Item item = obj.GetComponent<Item>();
        if (item != null)
        {
            _WeaponHandler.GetItemWeapon(item);
        }
    }
}
```

* 플레이어가 충돌한 오브젝트가 `Item`일 경우에만 상호작용 실행
* `WeaponHandler`를 통해 무기 스왑 처리

### 무기 교체 처리 (`WeaponHandler.GetItemWeapon()`)

```csharp
public void GetItemWeapon(Item item)
{
    Weapon now = currentWeapon;
    Weapon get = item.weapon;

    weapons[currentWeaponIndex] = get;
    currentWeapon = get;

    get.transform.SetParent(now.transform.parent, false);
    now.transform.SetParent(item.transform, false);

    item.weapon = now;

    get.reload();
    now.stop();
}
```

* 현재 장비 무기와 아이템 무기를 서로 교체
* `Transform` 부모를 변경하여 위치 시각적으로 조정
* 새 무기는 `reload()`, 기존 무기는 `stop()`을 호출하여 무기 상태를 초기화

### 구현 요약

| 항목      | 설명                                     |
| ------- | -------------------------------------- |
| 입력 키    | F 키 (`TadaInput.ThisKey.GetItem`)      |
| 충돌 감지   | `PlayerPhysics.getCollidedObject()` 사용 |
| 상호작용 대상 | `Item` 컴포넌트를 가진 오브젝트                   |
| 처리 방식   | `WeaponHandler`를 통해 무기 객체 간 스왑         |

> 본 구조는 직관적인 상호작용 흐름을 제공하며, 확장 시 다양한 아이템 종류(체력, 방어구, 기타 소비 아이템 등)로도 연계할 수 있습니다.
