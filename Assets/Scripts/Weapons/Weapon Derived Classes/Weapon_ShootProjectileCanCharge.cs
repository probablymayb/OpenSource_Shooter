using Photon.Pun;
using UnityEngine;

/// <summary>
/// This class has a primary action to shoot projectiles based on a UseRate value
/// and a secondary action with a timer that after reaching its duration will shoot a
/// secondary projectile.
/// </summary>

public class Weapon_ShootProjectileCanCharge : Weapon
{
    // --------------------------------------
    // ----- 2D Isometric Shooter Study -----
    // ----------- by Tadadosi --------------
    // --------------------------------------
    // ---- Support my work by following ----
    // ---- https://twitter.com/tadadosi ----
    // --------------------------------------

    protected float CHARGE_DURATION { 
        get { return _CHARGE_DURATION; } 
        private set { _CHARGE_DURATION = value; } }
    [SerializeField] protected float _CHARGE_DURATION;

    protected float CHARGE_DURATION_Default { 
        get { return _CHARGE_DURATION_Default; } 
        private set { _CHARGE_DURATION_Default = value; } }
    [SerializeField] protected float _CHARGE_DURATION_Default;

    [TextArea(5, 10)]
    public string notes = "This is a derived class from the base class Weapon. It has a primary action to shoot projectiles " +
        "based on a UseRate value and a secondary action with a timer that after reaching its duration will shoot a secondary projectile.";

    public GameObject basicProjectilePrefab;
    public GameObject chargedProjectilePrefab;

    public Transform projectileSpawnPoint;
    public GameObject chargingPFX;
    public SoundHandlerLocal chargingSFX;

    [SerializeField] protected Animator animator;

    private enum WeaponAction { Idle = 0, BasicShot = 1, Charging = 2, ChargedShot = 3 }

    protected Projectile primaryProjectile;
    protected Projectile secondaryProjectile;
    private bool isReceivingInput = false;
    private bool isCharging = false;
    private float chargingTime;

    //카메라 진동 설정(duration, shakeAmount, decreaseFactor)
    protected Vector3 CS_P;
    protected Vector3 CS_SC;
    protected Vector3 CS_SF;

    public override void stop(){
        base.stop();
        isReceivingInput = false;
        OnChargeCancel();
    }

    protected override void Awake()
    {
        _CHARGE_DURATION_Default = 2f;
        base.Awake();
        //useRateValues = new float[] { 0.125f, 0.05f };
        //TryGetComponent(out animator);

        CS_P = new Vector3(0.075f, 0.1f, 3f);
        CS_SC = new Vector3(0, 0.065f, 1f);
        CS_SF = new Vector3(0.2f, 1f, 3f);
    }

    protected override void Update()
    {
        base.Update();
        if (isReceivingInput)
        {
            // Execute the initial actions that take place in the first frame after isReceivingInput is set to true.
            OnChargingStart();

            // Increase the value of charging time by adding Time.deltaTime on each frame.
            chargingTime += Time.deltaTime;

            // Update the OnCharging actions and pass chargingTime as argument.
            OnCharging(chargingTime);

            // If charging time is equal or greater than the constant charge duration, execute the last actions.
            if (chargingTime >= CHARGE_DURATION)
                OnChargingEnd();
        }

        // If the player releases the secondary action button making isReceivingInput false and the weapon is
        // currently charging, execute the cancel actions.
        if (!isReceivingInput && isCharging)
            OnChargeCancel();
    }

    protected override void calculateUseRate(){
        base.calculateUseRate();
        _CHARGE_DURATION = _CHARGE_DURATION_Default / useRateValues[useRateIndex];

/////////////////////////////////////////////////////
/// 애니메이션 구현?
/// 발사 속도가 빠르면 애니메이션도 똑같이 빠르게 재생되도록 설정
/// 기본 발사는 속도가 느려도 최소치가 있음
        /*
        //change animation speed from default speed
        anim.SetAnimationSpeed("BasicShot", (_UseRate<0.125f)?(0.125f/_UseRate):(1));
        anim.SetAnimationSpeed("Charging", 2f/_CHARGE_DURATION);
        anim.SetAnimationSpeed("ChargedShot", 2f/_CHARGE_DURATION);
        */
    }

    private void OnEnable()
    {
        SpawnProjectiles();
    }

    protected override void OnCanUse()
    {
        base.OnCanUse();
        SpawnProjectiles();
        animator.SetInteger("WeaponAction", (int)WeaponAction.Idle);
    }

    private void SpawnProjectiles()
    {
        // Stop from executing if this variables are not set.
        if (basicProjectilePrefab == null || chargedProjectilePrefab == null || projectileSpawnPoint == null)
        {
            Debug.LogError(gameObject.name + " missing prefabs or spawnPoint!");
            return;
        }

        // If there is no projectile, instantiate one and store its Projectile component.
        if (primaryProjectile == null)
        {
            // Instantiate.
            if (!PhotonNetwork.InRoom)
            {
                primaryProjectile = Instantiate(basicProjectilePrefab, projectileSpawnPoint.position,
                projectileSpawnPoint.rotation, projectileSpawnPoint).GetComponent<Projectile>();

                // Disable to hide it while it's behind the weapon.
                primaryProjectile.SetActive(false);
            }
        }

        // If there is no projectile, instantiate one and store its Projectile component.
        if (secondaryProjectile == null)
        {
            // Instantiate.
            if (!PhotonNetwork.InRoom)
            {
                secondaryProjectile = Instantiate(chargedProjectilePrefab, projectileSpawnPoint.position,
                projectileSpawnPoint.rotation, projectileSpawnPoint).GetComponent<Projectile>();

                // Disable to hide it while it's behind the weapon.
                secondaryProjectile.SetActive(false);
            }
        }
    }

    //발사 함수(상속받은 클래스가 수정 가능)
    protected virtual void PrimaryFire() {
        primaryProjectile.Fire();
    }
    protected virtual void SecondaryFire() {
        secondaryProjectile.Fire();
    }

    public override void PrimaryAction(bool value)
    {
        base.PrimaryAction(value);

        if(primaryProjectile == null && canUse)
        {
            string prefabName = basicProjectilePrefab.name;
            GameObject bullet = PhotonNetwork.Instantiate("Projectiles/" + prefabName, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            bullet.transform.SetParent(projectileSpawnPoint);
            primaryProjectile = bullet.GetComponent<Projectile>();
        }

        // Can be executed only if there is a projectile available and canUse is true.
        if (primaryProjectile != null && canUse)
        {
            // Play the basic animation if WeaponAnim_ShootProjectileCanCharge is available.
            animator.SetInteger("WeaponAction", (int)WeaponAction.BasicShot);

            // Make the camera Shake.
            CameraShake.Shake(CS_P.x, CS_P.y, CS_P.z);

            // Call the method Fire on the projectile to launch it towards the crosshair direction.

            //투사체를 직접 발사하지 않음, 여전히 비활성화
            // Enable the projectile.
            //primaryProjectile.SetActive(true);

            //네트워크에 있을시 발사 방향을 건네줌?
            primaryProjectile.isRPCFire = (PhotonNetwork.InRoom && PhotonManager._currentPhase == PhotonManager.GamePhase.InGame);

            //발사 함수 호출
            PrimaryFire();
            
            //투사체를 삭제하지 않음
            // We make it null to give room to a new instantiated projectile.
            //primaryProjectile = null;

            // We make it false to execute the base Update actions which makes it true again after UseRate duration is reached,
            // which then calls the method OnCanUse() that's used to spawn new projectiles and to return to the Idle anim.
            canUse = false;
        }
    }

    public override void SecondaryAction(bool value)
    {
        base.SecondaryAction(value);

        // The purpose of this action is to let the player hold the secondary action button to make the bool
        // isReceivingInput true, which in turn enables a timer and a series of actions to ultimately launch the
        // secondary projectile.

        // After firing the projectile, canUse is set to false and because the player can continuously call this method,
        // we use this to stop isReceivingInput from getting a true value.
        if (!canUse)
        {
            // Cancel inputs after use.
            isReceivingInput = false;
            return;
        }

        // We stop the code here if one of the needed variables is missing.
        if (!IsSecondaryDataAvailable())
        {
            Debug.LogWarning(gameObject.name + ": missing prefabs!");
            return;
        }

        // We make it true if the player is pressing the secondary action button or false if not.
        // When it's true, it activates the actions on the Update method of this class.
        isReceivingInput = value;
    }

    private bool IsSecondaryDataAvailable()
    {
        if (PhotonNetwork.InRoom)
            return chargingPFX != null && chargingSFX != null;
        else
            return secondaryProjectile != null && chargingPFX != null && chargingSFX != null;
    }

    /// <summary>
    /// Initial actions that take place in the first frame after isReceivingInput is set to true.
    /// </summary>
    private void OnChargingStart()
    {
        // This actions can only be executed if isCharging is false.
        if (!isCharging)
        {
            // Play the charging animation if WeaponAnim_ShootProjectileCanCharge is available.
            animator.SetInteger("WeaponAction", (int)WeaponAction.Charging);

            // We set it to true to avoid calling this method more than once.
            isCharging = true;

            // NOTE: Weapon Charging Shake. Right now this shake behaviour will override the OnChargingEnd shake.
            // In order to fix this I need to add timer to stop the charging option from being rapidly reused.
            CameraShake.Shake(CHARGE_DURATION, CS_SC.y, CS_SC.z);

            // Enable the charging visual effects.
            chargingPFX.SetActive(true);

            // Play the first sound of SoundHandlerLocal.
            chargingSFX.PlaySound();
        }
    }

    /// <summary>
    /// Actions that take place while isReceivingInput is true and time is being added to chargingTime.
    /// </summary>
    private void OnCharging(float t)
    {
        // Increase the size of the charging fx to enhance it with a feeling a anticipation.
        chargingPFX.transform.localScale = Vector2.one * t;
    }

    /// <summary>
    /// Last actions that take place when chargingTime value is equal or greater than CHARGE_DURATION.
    /// </summary>
    private void OnChargingEnd()
    {
        // Play the charged shot animation if WeaponAnim_ShootProjectileCanCharge is available.
        animator.SetInteger("WeaponAction", (int)WeaponAction.ChargedShot);

        // Set it to false to allow OnChargingStart to be called again.
        isCharging = false;

        // Set it to false to stop the player from making isReceivingInput true if it's holding the secondary action button.
        canUse = false;

        // Reset the timer to allow correctly restarting the charging action.
        chargingTime = 0.0f;

        // Make the camera Shake by a greater value.
        CameraShake.Shake(CS_SF.x, CS_SF.y, CS_SF.z);

        // Call the method Fire on the projectile to launch it towards the crosshair direction.

        bool isRight = PlayerBodyPartsHandler.isRightDirection;
        // Enable the projectile.
        if (secondaryProjectile == null)
        {
            string prefabName = chargedProjectilePrefab.name;
            GameObject bullet = PhotonNetwork.Instantiate("Projectiles/" + prefabName, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            bullet.transform.SetParent(projectileSpawnPoint);
            secondaryProjectile = bullet.GetComponent<Projectile>();
        }

        //secondaryProjectile.SetActive(true);

        secondaryProjectile.isRPCFire = (PhotonNetwork.InRoom && PhotonManager._currentPhase == PhotonManager.GamePhase.InGame);
        SecondaryFire();

        // Make it null to give room to a new instantiated projectile.
        //secondaryProjectile = null;

        // Reset the scale of the charging visual fx.
        chargingPFX.transform.localScale = Vector2.one;

        // Disable the charge visual fx.
        chargingPFX.SetActive(false);

        // Stop the charging SoundHandlerLocal sounds.
        chargingSFX.StopSound();
    }

    /// <summary>
    /// Actions that take place if the secondary action is cancelled (when the player released the button before calling
    /// OnChargingEnd).
    /// </summary>
    private void OnChargeCancel()
    {
        // Return to Idle animation if WeaponAnim_ShootProjectileCanCharge is available.
        animator.SetInteger("WeaponAction", (int)WeaponAction.Idle);

        // Is set to false to allow OnChargingStart to be called again.
        isCharging = false;

        // Is set to zero to reset the timer so that it can correctly count the time again.
        chargingTime = 0.0f;

        // Stop the camera from shaking.
        CameraShake.Shake(0f, 0f, 0f);

        // Stop the charging SoundHandlerLocal sounds.
        chargingSFX.StopSound();

        // Reset the scale of the charging visual fx.
        chargingPFX.transform.localScale = Vector2.one;

        // Disable the charging visual fx.
        chargingPFX.SetActive(false);
    }
}
