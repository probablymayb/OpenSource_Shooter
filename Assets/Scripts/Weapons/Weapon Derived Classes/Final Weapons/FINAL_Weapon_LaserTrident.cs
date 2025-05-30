using UnityEngine;
using UnityEditor;

/// <summary>
/// This class has Final Weapon : LaserGun
/// </summary>


public class FINAL_Weapon_LaserTrident : Weapon_ShootProjectileCanCharge
{
    protected override void Awake()
    {
        base.Awake();

        //Projectile Position
        transform.localPosition = new Vector3(0.316f, 0.204f, 0);
        projectileSpawnPoint = createPSp(this, new Vector3(0.703f, 0, 0));

        //Prefab
        basicProjectilePrefab = getProjPref("pfab_Bullet_Laser_MidSize_Blue");
        chargedProjectilePrefab = getProjPref("pfab_Bullet_Laser_Huge_Yellow");


        //ChargingSound
        chargingSFX = gameObject.AddComponent<SoundHandlerLocal>();
        chargingSFX.sounds = new Sound[1];
        Sound sound = new Sound();
        sound.clip = getAudioClip("Lasers/440147__dpren__scifi-gun-mega-charge-cannon_ChargeOnly_01_CC0");
        sound.SetComp(0.75f, 0.9f, 1f);
        chargingSFX.sounds[0] = sound;


        //LaserRifle_Sprite
        SpriteRenderer spr = gameObject.AddComponent<SpriteRenderer>();
        Sprite[] sprites = getSpriteSheet("Weapons/Weapons_001");
        spr.sprite = getSpriteFromSheet(sprites, "LaserBeam_Rifle_Body");
        spr.sortingOrder = 10;

        createSpriteObject("EnergyLines_Battery", getSpriteFromSheet(sprites, "LaserBeam_Rifle_EnergyLines_Battery"), 
        11, this, position: new Vector3(-0.6875f, 0, 0));

        createSpriteObject("EnergyLines_Body", getSpriteFromSheet(sprites, "LaserBeam_Rifle_EnergyLines_Body"), 
        11, this, position: new Vector3(-0.0313f, 0, 0));

        createSpriteObject("EnergyLines_Muzzle", getSpriteFromSheet(sprites, "LaserBeam_Rifle_EnergyLines_Muzzle"), 
        11, this, position: new Vector3(-0.625f, 0, 0));

        //ChargingPFX
        chargingPFX = createPFX("pfx_Bullet_Laser_Charge", "pfx_Bullet_Laser_Charge_Yellow", 
        this.transform, new Vector3(0.789f, -0.002f, 0));

        
        //animation
        /*
        Animation ani = gameObject.AddComponent<Animation>();
        AnimationClip Idle = getAnimationClip("LaserBeam_Rifle/Idle.anim");
        ani.clip = Idle;
        ani.AddClip(Idle, "Idle");
        ani.AddClip(getAnimationClip("LaserBeam_Rifle/BasicShot.anim"), "BasicShot");
        ani.AddClip(getAnimationClip("LaserBeam_Rifle/Charging.anim"), "Charging");
        ani.AddClip(getAnimationClip("LaserBeam_Rifle/ChargedShot.anim"), "ChargedShot");
        anim = gameObject.AddComponent<WeaponAnim_ShootProjectileCanCharge>();
        */
        
    }

    public override void reload(){
        base.reload();
        this.animator = getAnimatior();
    }
    public override void stop(){
        base.stop();
        this.animator = null;
    }
}
