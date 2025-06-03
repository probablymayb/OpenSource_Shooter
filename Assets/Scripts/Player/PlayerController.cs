using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// This class receives inputs from TadaInput and calls methods from other classes based on those inputs. It 
/// also handles movement.
/// </summary>
public class PlayerController : MonoBehaviour
{
    // --------------------------------------
    // ----- 2D Isometric Shooter Study -----
    // ----------- by Tadadosi --------------
    // --------------------------------------
    // ---- Support my work by following ----
    // ---- https://twitter.com/tadadosi ----
    // --------------------------------------

    [TextArea(2, 10)]
    public string notes = "It receives inputs from TadaInput and calls methods from other classes based on those inputs. It " +
        "also handles movement.";

    #region ---------------------------- PROPERTIES

    private PhotonView _photonView;
    private CrosshairMouse _crossHairMouse;
    private Camera _playerCamera;

    private PlayerPhysics _PlayerPhysics;
    private PlayerSkills _PlayerSkills;
    private PlayerAnimations _PlayerAnimations;
    private WeaponHandler _WeaponHandler;

    #endregion

    #region ---------------------------- UNITY CALLBACKS

    // Ignore never invoked message, it's 
    private void Awake()
    {
        Initialize();

        _photonView = GetComponent<PhotonView>();
        _playerCamera = GetComponentInChildren<Camera>();
        _crossHairMouse = GetComponentInChildren<CrosshairMouse>();

        if (_photonView == null || _playerCamera == null || _crossHairMouse == null) return;

        if (_photonView.IsMine)
        {
            _playerCamera.enabled = true;
            _crossHairMouse.gameObject.SetActive(true);

            AudioListener listener = _playerCamera.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = true;
        }
        else
        {
            _playerCamera.enabled = false;
            _crossHairMouse.gameObject.SetActive(false);

            AudioListener listener = _playerCamera.GetComponent<AudioListener>();
            if (listener != null) listener.enabled = false;
        }
    }

    private void Update()
    {
        // 🔒 내 플레이어가 아니면 아무것도 하지 않음
        if (!_photonView.IsMine) return;

        CheckIfMissingClasses();

        if (PauseController.isGamePaused)
            return;

        #region ---------------------------- SKILLS
        if (TadaInput.GetKeyDown(TadaInput.ThisKey.Dash) && _PlayerPhysics.Velocity.sqrMagnitude > 0)
            _PlayerSkills.Dash();
        #endregion

        #region ---------------------------- ANIMATIONS
        _PlayerAnimations.PlayMoveAnimationsByMoveInputAndLookDirection(TadaInput.MoveAxisRawInput);

        float speed = _PlayerPhysics.Velocity.magnitude;
        _PlayerAnimations.SetAnimationSpeed(PlayerAnimations.AnimName.WalkForward, speed);
        _PlayerAnimations.SetAnimationSpeed(PlayerAnimations.AnimName.WalkBackwards, speed);
        #endregion

        #region ---------------------------- WEAPON ACTIONS
        if (TadaInput.GetKey(TadaInput.ThisKey.PrimaryAction))
            _WeaponHandler.UseWeapon(WeaponHandler.ActionType.Primary);

        if (TadaInput.GetKeyUp(TadaInput.ThisKey.PrimaryAction))
            _WeaponHandler.UseWeapon(WeaponHandler.ActionType.Primary, false);

        if (TadaInput.GetKey(TadaInput.ThisKey.SecondaryAction))
            _WeaponHandler.UseWeapon(WeaponHandler.ActionType.Secondary);

        if (TadaInput.GetKeyUp(TadaInput.ThisKey.SecondaryAction))
            _WeaponHandler.UseWeapon(WeaponHandler.ActionType.Secondary, false);
        #endregion

        #region ---------------------------- RELOAD
        if (Input.GetKeyDown(KeyCode.R))
        {
            _WeaponHandler.ReloadCurrentWeapon();
        }
        #endregion

        #region ---------------------------- WEAPON USE RATE
        if (TadaInput.GetKeyDown(TadaInput.ThisKey.NextUseRate))
            _WeaponHandler.SwitchUseRate(Weapon.SwitchUseRateType.Next);

        if (TadaInput.GetKeyDown(TadaInput.ThisKey.PreviousUseRate))
            _WeaponHandler.SwitchUseRate(Weapon.SwitchUseRateType.Previous);
        #endregion

        #region ---------------------------- WEAPON SWITCH
        if (Input.GetKeyDown(KeyCode.Alpha1))
            _WeaponHandler.SwitchWeapon(WeaponHandler.WeaponSwitchMode.ByIndex, 0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            _WeaponHandler.SwitchWeapon(WeaponHandler.WeaponSwitchMode.ByIndex, 1);

        if (TadaInput.GetKeyDown(TadaInput.ThisKey.NextWeapon))
            _WeaponHandler.SwitchWeapon(WeaponHandler.WeaponSwitchMode.Next);

        if (TadaInput.GetKeyDown(TadaInput.ThisKey.PreviousWeapon))
            _WeaponHandler.SwitchWeapon(WeaponHandler.WeaponSwitchMode.Previous);
        #endregion

        #region ---------------------------- GET ITEM

        if (TadaInput.GetKeyDown(TadaInput.ThisKey.GetItem)){
            //check is collided
            GameObject obj = _PlayerPhysics.getCollidedObject();
            if(obj != null){
                //If colObject is Item
                Item item = obj.GetComponent<Item>();
                if(item != null){
                   _WeaponHandler.GetItemWeapon(item);
                }
            }
        }
        
        #endregion
    }


    #endregion

    #region ---------------------------- METHODS

    /// <summary>
    /// All the actions that should be done on <see cref="Awake"/>
    /// </summary>
    private void Initialize()
    {
        TryGetComponent(out _PlayerAnimations);
        TryGetComponent(out _WeaponHandler);
        TryGetComponent(out _PlayerPhysics);
        TryGetComponent(out _PlayerSkills);
    }

    /// <summary>
    /// To display a warning message in the console if there is something missing 
    /// that should be added in the Inspector.
    /// </summary>
    private void CheckIfMissingClasses()
    {
        if (_PlayerAnimations == null || _WeaponHandler == null || _PlayerPhysics == null || _PlayerSkills == null)
        {
            Debug.LogWarning(gameObject.name + ": Missing behaviour classes!");
            return;
        }
    }

    #endregion
}