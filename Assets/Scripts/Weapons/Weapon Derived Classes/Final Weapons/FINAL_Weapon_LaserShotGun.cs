using UnityEngine;
using UnityEditor;

/// <summary>
/// This class has Final Weapon : LaserGun
/// </summary>


public class FINAL_Weapon_LaserShotGun : Weapon_ShotGun
{
    protected override void Awake()
    {
        base.Awake();

        //Projectile Position
        transform.localPosition = new Vector3(0.085f, 0.094f, 0);
        projectileSpawnPoint = createPSp(this, new Vector3(0.227f, 0.091f, 0));

        //Prefab
        basicProjectilePrefab = getProjPref("pfab_Bullet_Laser_Small_Cyan.prefab");
        strongProjectilePrefab = getProjPref("pfab_Bullet_Laser_MidSize_Blue.prefab");

        //LaserRifle_Sprite
        SpriteRenderer spr = gameObject.AddComponent<SpriteRenderer>();
        Sprite[] sprites = getSpriteSheet("Weapons/Weapons_001.png");
        spr.sprite = getSpriteFromSheet(sprites, "LaserRifle_Handle");
        spr.sortingOrder = 10;

        GameObject energy = createSpriteObject("LaserRifle_Energy", getSpriteFromSheet(sprites, "LaserRifle_Energy"), 9, 
        this, scale: new Vector3(0.87f, 0.83f, 1f));

        Sprite Sprite_BP = getSpriteFromSheet(sprites, "LaserRifle_BarrelPiece");
        GameObject[] BPs = new GameObject[4];
        BPs[0] = createSpriteObject("BarrelPiece_00", Sprite_BP, 10, this, position: new Vector3(0.06f, 0, 0));
        for(int i=1; i<4; i++){
            BPs[i] = createSpriteObject("BarrelPiece_0" + i, Sprite_BP, 10, BPs[i-1], position: new Vector3(0.12f, 0, 0));
        }

        GameObject muzzle = createSpriteObject("Muzzle", getSpriteFromSheet(sprites, "LaserRifle_Muzzle"), 10, 
        BPs[3], position: new Vector3(0.12f, 0, 0));


        
        //animation
        Animation ani = gameObject.AddComponent<Animation>();
        AnimationClip Idle = getAnimationClip("LaserRifle_Auto-Charge/Idle.anim");
        ani.clip = Idle;
        ani.AddClip(Idle, "Idle");
        ani.AddClip(getAnimationClip("LaserRifle_Auto-Charge/BasicShot.anim"), "BasicShot");
        ani.AddClip(getAnimationClip("LaserRifle_Auto-Charge/BasicShot.anim"), "StrongShot");
        anim = gameObject.AddComponent<WeaponAnim_ShotGun>();
        
    }
}
