using Photon.Pun;
using UnityEngine;

/// <summary>
/// This class has a unique action to do a continuous shooting (e.g. Continuous Laser Beam).
/// </summary>
public class Weapon_ChargeContinuousShooting : Weapon
{
    // --------------------------------------
    // ----- 2D Isometric Shooter Study -----
    // ----------- by Tadadosi --------------
    // --------------------------------------
    // ---- Support my work by following ----
    // ---- https://twitter.com/tadadosi ----
    // --------------------------------------

    public GameObject basicProjectilePrefab;
    public GameObject chargedProjectilePrefab;
    public Transform projectileSpawnPoint;
    public GameObject chargingPFX;
    public SoundHandlerGlobal chargingSFX;

    private Projectile primaryProjectile;
    private Projectile secondaryProjectile;
    private bool isReceivingInput = false;
    private bool isChargingStarted = false;
    private bool isChargeShooting = false;
    private float chargingTime;

    private const float CHARGE_DELAY = 2f;

    // NOTE: Work in progress.

    protected override void Awake()
    {
        base.Awake();
        useRateValues = new float[] { 0.15f };
        SwitchUseRate(0);
    }

    protected override void Update()
    {
        base.Update();
        if (isReceivingInput)
        {
            if (!isChargeShooting)
            {
                OnChargingStart();

                chargingTime += Time.deltaTime;
                OnCharging(chargingTime);

                if (chargingTime >= CHARGE_DELAY)
                {
                    isChargeShooting = true;
                    OnChargingEnd();
                }
            }
            else
                OnChargeShooting();
        }

        if (!isReceivingInput && (isChargingStarted || isChargeShooting))
            OnChargeCancel();
    }

    private void OnEnable()
    {
        SpawnProjectiles();
    }

    protected override void OnCanUse()
    {
        base.OnCanUse();
        SpawnProjectiles();
    }

    private void SpawnProjectiles()
    {
        if (basicProjectilePrefab == null || chargedProjectilePrefab == null || projectileSpawnPoint == null)
        {
            Debug.LogError(gameObject.name + " missing prefabs or spawnPoint!");
            return;
        }

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

        if (primaryProjectile != null && canUse)
        {
            bool isRight = PlayerBodyPartsHandler.isRightDirection;
            CameraShake.Shake(duration: 0.075f, shakeAmount: 0.1f, decreaseFactor: 3f);

            //primaryProjectile.SetActive(true);

            primaryProjectile.isRPCFire = (PhotonNetwork.InRoom && PhotonManager._currentPhase == PhotonManager.GamePhase.InGame);

            primaryProjectile.Fire();

            //primaryProjectile = null;
            canUse = false;
        }
    }

    public override void SecondaryAction(bool value)
    {
        base.SecondaryAction(value);
        if (!IsSecondaryDataAvailable())
        {
            Debug.LogWarning(gameObject.name + ": missing prefabs!");
            return;
        }
        isReceivingInput = value;
    }

    private bool IsSecondaryDataAvailable()
    {
        if (PhotonNetwork.InRoom)
            return chargingPFX != null && chargingSFX != null;
        else
            return secondaryProjectile != null && chargingPFX != null && chargingSFX != null;
    }

    private void OnChargingStart()
    {
        if (!isChargingStarted)
        {
            isChargingStarted = true;

            // NOTE: Weapon Charging Shake. Right now this shake behaviour will override the OnChargingEnd shake.
            // In order to fix this I need to add timer to stop the charging option from being rapidly reused.
            CameraShake.Shake(duration: CHARGE_DELAY, shakeAmount: 0.065f, decreaseFactor: 1f);

            chargingPFX.SetActive(true);
            chargingSFX.PlaySound();
        }
    }

    private void OnCharging(float t)
    {
        chargingPFX.transform.localScale = Vector2.one * t;
    }

    private void OnChargingEnd()
    {
        isReceivingInput = false;
        isChargingStarted = false;
        chargingTime = 0.0f;

        bool isRight = PlayerBodyPartsHandler.isRightDirection;
        CameraShake.Shake(duration: 0.2f, shakeAmount: 1f, decreaseFactor: 3f);

        if(secondaryProjectile == null && canUse)
        {
            string prefabName = chargedProjectilePrefab.name;
            GameObject bullet = PhotonNetwork.Instantiate("Projectiles/" + prefabName, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            bullet.transform.SetParent(projectileSpawnPoint);
            secondaryProjectile = bullet.GetComponent<Projectile>();
        }

        //secondaryProjectile.SetActive(true);

        secondaryProjectile.isRPCFire = (PhotonNetwork.InRoom && PhotonManager._currentPhase == PhotonManager.GamePhase.InGame);
        secondaryProjectile.Fire();
        
        secondaryProjectile = null;
        chargingPFX.transform.localScale = Vector2.one;
        chargingPFX.SetActive(false);
    }

    private void OnChargeShooting()
    {
        
    }

    private void OnChargeCancel()
    {
        isChargeShooting = false;
        isReceivingInput = false;
        isChargingStarted = false;
        chargingTime = 0.0f;
        CameraShake.Shake(0f, 0f, 0f);
        chargingSFX.StopSound();
        chargingPFX.transform.localScale = Vector2.one;
        chargingPFX.SetActive(false);
    }
}
