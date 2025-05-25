using UnityEngine;

/// <summary>
/// This class has a primary action to shoot projectiles based on a UseRate value
/// and a secondary action with a timer that after reaching its duration will shoot a
/// secondary projectile.
/// </summary>

public class Weapon_ShotGun : Weapon
{
    // --------------------------------------
    // ----- 2D Isometric Shooter Study -----
    // ----------- by Tadadosi --------------
    // --------------------------------------
    // ---- Support my work by following ----
    // ---- https://twitter.com/tadadosi ----
    // --------------------------------------

    #region ---------------------------- VAR

    /// <summary>
    /// the attack interval of a weapon
    /// UseRateDefault / useRateValues
    /// </summary>
    public float UseRateStrong { 
        get { return _UseRateStrong; } 
        private set { _UseRateStrong = value; } }
    [SerializeField] protected float _UseRateStrong;
    
    /// <summary>
    /// the default attack interval of a weapon
    /// </summary>
    public float UseRateStrongDefault { 
        get { return _UseRateStrongDefault; } 
        private set { _UseRateStrongDefault = value; } }
    [SerializeField] protected float _UseRateStrongDefault;

    protected bool canUseStrong;
    protected bool canUseTotal;

    [TextArea(5, 10)]
    public string notes = "This is a derived class from the base class Weapon. It has a primary action to shoot projectiles " +
        "based on a UseRate value and a secondary action with a timer that after reaching its duration will shoot a secondary projectile.";

    public GameObject basicProjectilePrefab;
    public GameObject strongProjectilePrefab;
    public Transform projectileSpawnPoint;

    protected WeaponAnim_ShotGun anim;
    protected Projectile primaryProjectile;
    protected Projectile secondaryProjectile;

    private float t2;

    //the Vector of CameraShake(duration, shakeAmount, decreaseFactor)
    protected Vector3 CS_P;
    protected Vector3 CS_S;

    #endregion

    #region ---------------------------- DEFAULT FUNCTION

    public override void stop(){
        base.stop();
        canUseStrong = false;
    }

    protected override void Awake()
    {
        base.Awake();
        canUseStrong = true;
        canUseTotal = true;

        //set useRate
        _UseRateDefault = 0.5f;
        _UseRateStrongDefault = 1.0f;

        TryGetComponent(out anim);

        CS_P = new Vector3(0.075f, 0.1f, 3f);
        CS_S = new Vector3(0.075f, 0.2f, 3f);

        /*
        CS_P = Vector3.zero;
        CS_S = Vector3.zero;
        */
    }

    protected override void Update()
    {   
        if(!canUse || !canUseStrong){
            canUseTotal = false;
        }
        
        base.Update();
        if (!canUseStrong)
        {
            t2 += Time.deltaTime;
            if (t2 >= _UseRateStrong)
            {
                canUseStrong = true;
                OnCanUse();
                t2 = 0.0f;
            }
        }
    }

    protected override void calculateUseRate() {
        base.calculateUseRate();
        _UseRateStrong = _UseRateStrongDefault / useRateValues[useRateIndex];

        //change animation speed from default speed
        //anim.SetAnimationSpeed("BasicShot", (_UseRate<0.125f)?(0.125f/_UseRate):(1));
        anim.SetAnimationSpeed("BasicShot", 0.125f/_UseRate);
        anim.SetAnimationSpeed("StrongShot", 0.125f/_UseRate);
    }

    private void OnEnable()
    {
        SpawnProjectiles();
    }

    protected override void OnCanUse()
    {   
        if(canUse && canUseStrong){
            canUseTotal = true;
        }
        else return;

        base.OnCanUse();
        SpawnProjectiles();
        if (anim != null)
            anim.PlayAnimation(WeaponAnim_ShotGun.Animation.Idle);
    }

    private void SpawnProjectiles()
    {
        // Stop from executing if this variables are not set.
        if (basicProjectilePrefab == null || strongProjectilePrefab == null || projectileSpawnPoint == null)
        {
            Debug.LogError(gameObject.name + " missing prefabs or spawnPoint!");
            return;
        }

        // If there is no projectile, instantiate one and store its Projectile component.
        if (primaryProjectile == null)
        {
            // Instantiate.
            primaryProjectile = Instantiate(basicProjectilePrefab, projectileSpawnPoint.position,
            projectileSpawnPoint.rotation, projectileSpawnPoint).GetComponent<Projectile>();

            // Disable to hide it while it's behind the weapon.
            primaryProjectile.SetActive(false);
        }

        // If there is no projectile, instantiate one and store its Projectile component.
        if (secondaryProjectile == null)
        {
            // Instantiate.
            secondaryProjectile = Instantiate(strongProjectilePrefab, projectileSpawnPoint.position,
            projectileSpawnPoint.rotation, projectileSpawnPoint).GetComponent<Projectile>();

            // Disable to hide it while it's behind the weapon.
            secondaryProjectile.SetActive(false);
        }
    }

    #endregion

    #region ---------------------------- SHOOT FUNCTION

    //fire func
    protected virtual void PrimaryFire() {
        primaryProjectile.setVolume(2f/3);

        for(int i=0; i<3; i++) {
            primaryProjectile.Fire(Random.Range(-10, 10));
            primaryProjectile.Speed = 20f*Random.Range(0.9f, 1.1f);
        }
        primaryProjectile.Fire();
    }

    protected virtual void SecondaryFire() {
        secondaryProjectile.setVolume(2f/7);
        for(int i=0; i<7; i++) {
            secondaryProjectile.Fire(Random.Range(-20, 20));
            secondaryProjectile.Speed = 20f*Random.Range(0.9f, 1.1f);
        }
        secondaryProjectile.Fire();
    }

    public override void PrimaryAction(bool value)
    {
        base.PrimaryAction(value);

        // Can be executed only if there is a projectile available and canUse is true.
        if (primaryProjectile != null && canUseTotal)
        {
            // Play the basic animation if WeaponAnim_ShootProjectileCanCharge is available.
            if (anim != null)
                anim.PlayAnimation(WeaponAnim_ShotGun.Animation.BasicShot);

            // Make the camera Shake.
            CameraShake.Shake(CS_P.x, CS_P.y, CS_P.z);

            // Call the method Fire on the projectile to launch it towards the crosshair direction.
            PrimaryFire();

            // We make it false to execute the base Update actions which makes it true again after UseRate duration is reached,
            // which then calls the method OnCanUse() that's used to spawn new projectiles and to return to the Idle anim.
            canUse = false;
        }
    }

    public override void SecondaryAction(bool value)
    {
        base.SecondaryAction(value);

        // Can be executed only if there is a projectile available and canUse is true.
        if (secondaryProjectile != null && canUseTotal)
        {
            // Play the basic animation if WeaponAnim_ShootProjectileCanCharge is available.
            if (anim != null)
                anim.PlayAnimation(WeaponAnim_ShotGun.Animation.StrongShot);

            // Make the camera Shake.
            CameraShake.Shake(CS_S.x, CS_S.y, CS_S.z);

            // Call the method Fire on the projectile to launch it towards the crosshair direction.
            SecondaryFire();

            // We make it false to execute the base Update actions which makes it true again after UseRate duration is reached,
            // which then calls the method OnCanUse() that's used to spawn new projectiles and to return to the Idle anim.
            canUseStrong = false;
        }
    }

    #endregion
}
