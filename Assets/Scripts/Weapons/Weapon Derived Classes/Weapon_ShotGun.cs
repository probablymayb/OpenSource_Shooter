using Photon.Pun;
using UnityEngine;

/// <summary>
/// 기본 발사 속도로 여러발의 탄환 발사
/// 강한 발사 속도로 더 많고 큰 탄환 발사
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

///////////////////////애니메이션 구현?
/// ShootProjectileCnaCharge에서 basicShot이 2개가 되었다고 가정
    [SerializeField] protected Animator animator;

    private enum WeaponAction { Idle = 0, BasicShot = 1, StrongShot = 2,}

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
        canUseStrong = true;
        canUseTotal = true;
        t2 = 0.0f;
    }

    protected override void Awake()
    {
        base.Awake();
        canUseStrong = true;
        canUseTotal = true;

        //set useRate
        _UseRateDefault = 0.5f;
        _UseRateStrongDefault = 1.0f;
        calculateUseRate();

        CS_P = new Vector3(0.075f, 0.1f, 3f);
        CS_S = new Vector3(0.075f, 0.2f, 3f);
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
        animator.SetInteger("WeaponAction", (int)WeaponAction.Idle);
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
            if (!PhotonNetwork.InRoom)
            {
                secondaryProjectile = Instantiate(strongProjectilePrefab, projectileSpawnPoint.position,
                projectileSpawnPoint.rotation, projectileSpawnPoint).GetComponent<Projectile>();

                // Disable to hide it while it's behind the weapon.
                secondaryProjectile.SetActive(false);
            }
        }
    }

    #endregion

    #region ---------------------------- SHOOT FUNCTION

    //fire func
    protected virtual void PrimaryFire(bool isRight)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject bullet = PhotonNetwork.Instantiate("Projectiles/" + basicProjectilePrefab.name,
                projectileSpawnPoint.position, projectileSpawnPoint.rotation);

            Projectile proj = bullet.GetComponent<Projectile>();
            proj.Speed = 20f * Random.Range(0.9f, 1.1f);
            proj.setVolume(2f / 3);
            proj.isRPCFire = (PhotonNetwork.InRoom && PhotonManager._currentPhase == PhotonManager.GamePhase.InGame);
            proj.Fire(Random.Range(-10f, 10f), isRight);  // 각도 차이
        }
    }

    protected virtual void SecondaryFire(bool isRight)
    {
        for (int i = 0; i < 7; i++)
        {
            GameObject bullet = PhotonNetwork.Instantiate("Projectiles/" + strongProjectilePrefab.name,
                projectileSpawnPoint.position, projectileSpawnPoint.rotation);

            Projectile proj = bullet.GetComponent<Projectile>();
            proj.Speed = 20f * Random.Range(0.9f, 1.1f);
            proj.setVolume(2f / 5);
            proj.isRPCFire = (PhotonNetwork.InRoom && PhotonManager._currentPhase == PhotonManager.GamePhase.InGame);
            proj.Fire(Random.Range(-20f, 20f), isRight);
        }
    }


    public override void PrimaryAction(bool value)
    {
        base.PrimaryAction(value);

        // Can be executed only if there is a projectile available and canUse is true.
        if (canUseTotal)
        {
            bool isRight = PlayerBodyPartsHandler.isRightDirection;

            animator.SetInteger("WeaponAction", (int)WeaponAction.BasicShot);

            CameraShake.Shake(CS_P.x, CS_P.y, CS_P.z);

            PrimaryFire(isRight);

            canUse = false;
        }
    }

    public override void SecondaryAction(bool value)
    {
        base.SecondaryAction(value);

        if (canUseTotal)
        {
            bool isRight = PlayerBodyPartsHandler.isRightDirection;

            animator.SetInteger("WeaponAction", (int)WeaponAction.StrongShot);

            CameraShake.Shake(CS_S.x, CS_S.y, CS_S.z);

            SecondaryFire(isRight);

            canUseStrong = false;
        }
    }

    #endregion
}
