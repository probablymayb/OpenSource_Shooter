using UnityEngine;
using System.Collections;

/// <summary>
/// Base class that has public methods for actions and a SwitchUseRate method to switch between a given 
/// float called UseRate which is used to control how often the actions can be called.
/// </summary>

public class Weapon : MonoBehaviour
{
    // --------------------------------------
    // ----- 2D Isometric Shooter Study -----
    // ----------- by Tadadosi --------------
    // --------------------------------------
    // ---- Support my work by following ----
    // ---- https://twitter.com/tadadosi ----
    // --------------------------------------

    #region ---------------------------- PROPERTIES

    /// <summary>
    /// Option for how to switch use rates using SwitchUseRate.
    /// </summary>
    public enum SwitchUseRateType { Next, Previous, ByIndex }

    [SerializeField] private bool debug = false;

    public float UseRate { get { return _UseRate; } private set { _UseRate = value; } }
    [SerializeField] protected float _UseRate;

    protected bool canUse;
    protected float[] useRateValues;
    protected int useRateIndex;
    protected float t;

    [Header("Ammo Settings")]
    [SerializeField] protected int maxAmmo = 30;
    [SerializeField] protected int currentAmmo;
    [SerializeField] protected float reloadTime = 2f;
    [SerializeField] protected bool infiniteAmmo = false;

    protected bool isReloading = false;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => isReloading;
    public bool HasAmmo => infiniteAmmo || currentAmmo > 0;
    #endregion

    #region ---------------------------- UNITY CALLBACKS
    protected virtual void Awake()
    {
        canUse = true;
        currentAmmo = maxAmmo; // 시작시 탄창 가득
    }

    protected virtual void Update()
    {
        if (!canUse)
        {
            t += Time.deltaTime;
            if (t >= _UseRate)
            {
                canUse = true;
                OnCanUse();
                t = 0.0f;
            }
        }
    }
    protected virtual void OnEnable()
    {
        // 무기 활성화시 탄약 확인
        if (currentAmmo == 0 && !infiniteAmmo)
        {
            currentAmmo = maxAmmo;
            Debug.Log($"Weapon enabled - Ammo set to {currentAmmo}");
        }
    }
    #endregion

    #region ---------------------------- METHODS

    public virtual void SwitchUseRate(SwitchUseRateType type, int index = 0)
    {
        if (useRateValues == null || useRateValues.Length == 0)
        {
            Debug.LogWarning(gameObject.name + ": Need to add at least one UseRate value!");
            return;
        }

        switch (type)
        {
            case SwitchUseRateType.Next:
                useRateIndex = ArraysHandler.GetNextIndex(useRateIndex, useRateValues.Length);
                break;

            case SwitchUseRateType.Previous:
                useRateIndex = ArraysHandler.GetPreviousIndex(useRateIndex, useRateValues.Length);
                break;

            case SwitchUseRateType.ByIndex:
                if (index >= 0 && index <= useRateValues.Length - 1)
                    useRateIndex = index;
                break;

            default:
                break;
        }

        _UseRate = useRateValues[useRateIndex];

        if (debug)
            Debug.Log(gameObject.name + " Weapon UseRate # " + useRateIndex + ": " + _UseRate);
    }

    protected virtual void OnCanUse()
    {
        if (debug)
            Debug.Log(gameObject.name + "Weapon: OnCanUse");
    }

    public virtual void PrimaryAction(bool value)
    {
        if (debug)
            Debug.Log(gameObject.name + "Weapon: PrimaryAction | " + value);
    }

    public virtual void SecondaryAction(bool value)
    {
        if (debug)
            Debug.Log(gameObject.name + "Weapon: SecondaryAction | " + value);
    }

    public virtual void Reload()
    {
        if (!isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    protected virtual IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Reload complete!");
    }

    protected virtual bool ConsumeAmmo()
    {
        if (infiniteAmmo) return true;

        if (currentAmmo > 0)
        {
            currentAmmo--;
            return true;
        }

        return false;
    }
    #endregion
}
