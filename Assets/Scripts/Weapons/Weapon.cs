using UnityEngine;

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

    /// the attack rate of a weapon
    /// UseRateDefault / useRateValues
    public float UseRate { 
        get { return _UseRate; } 
        private set { _UseRate = value; } }
    [SerializeField] protected float _UseRate;
    
    /// the default attack rate of a weapon
    public float UseRateDefault { 
        get { return _UseRateDefault; } 
        private set { _UseRateDefault = value; } }
    [SerializeField] protected float _UseRateDefault;

    protected bool canUse;
    protected float[] useRateValues;
    protected int useRateIndex;
    protected float t;

    #endregion

    #region ---------------------------- RESOURCE FUNC


    //ProjectilePrefab
    protected GameObject getProjPref(string objName){
        return getObject("Projectiles/" + objName);
    }

    //AudioClip
    protected AudioClip getAudioClip(string clipName){
        string path = "Audio/SFX/Weapons/" + clipName;
        AudioClip clip = Resources.Load<AudioClip>(path);
        if(clip == null) {Debug.LogError(path + " not Found");}
        return clip;
    }

    //try get Animator from parent
    protected Animator getAnimatior(){
        Transform current = transform;

        while(current != null){
            Animator animator = current.GetComponent<Animator>();
            if(animator != null) return animator;
            current = current.parent;
        }

        Debug.LogError("Animator not Found");
        return null;
    }


    protected Sprite getSpriteFromSheet(Sprite[] sprites, string sprName){
        foreach(Sprite sp in sprites){if(sp != null && sp.name == (sprName)) return sp;}

        Debug.LogError(sprName + " not Found");
        
        return null;
    }
    
    //return spriteSheet from its path
    protected Sprite[] getSpriteSheet(string sprName){
        string path = "Sprites/" + sprName;
        Object[] objs = Resources.LoadAll(path);

        Sprite[] sps = new Sprite[objs.Length];

        for(int i=0; i<objs.Length; i++){

            //Debug.Log(objs[i].ToString());

            if (objs[i] is Sprite) sps[i] = (Sprite)objs[i];
        }

        if(sps == null) {Debug.LogError(path + " not Found");}
        return sps;
    }

    protected GameObject getObject(string path){
        GameObject obj = Resources.Load<GameObject>(path);
        if(obj == null) {Debug.LogError(path + " not Found");}
        return obj;
    }
    
    //projectileSpawnPoint
    protected Transform createPSp(MonoBehaviour parent, Vector3 position = default(Vector3)){
        Transform trf = new GameObject("Projectile_SpawnPoint").transform;
        trf.SetParent(parent.transform);
        trf.localPosition = position;
        trf.localRotation = Quaternion.Euler(0, 0, 0);
        trf.localScale = new Vector3(-1, 1, 1);
        return trf;
    }

    //particle
    protected GameObject createPFX(string name, string objName, Transform parent, Vector3 position = default(Vector3)){
        GameObject PFX_IMSI = getObject("Particles/" + objName);
        GameObject Ins = Instantiate(PFX_IMSI, transform.position, Quaternion.identity);
        Ins.name = name;

        Transform trf = Ins.transform;
        trf.SetParent(parent);
        trf.localPosition = position;
        trf.localRotation = Quaternion.Euler(0, 0, 0);
        //trf.localScale = new Vector3(-1, 1, 1);

        Ins.SetActive(false);
        return Ins;
    }

    //return GameObject with Sprite
    private GameObject createSpriteObject(string name, Sprite sprite, int order, Transform parent, 
    Vector3 position = default(Vector3), Vector3 rotation = default(Vector3), Vector3 scale = default(Vector3)){
        if(scale == default(Vector3)) scale = Vector3.one;

        GameObject obj = new GameObject(name);
        Transform trf = obj.transform;
        trf.SetParent(parent);
        trf.localPosition = position;
        trf.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        trf.localScale = scale;

        SpriteRenderer spr = obj.gameObject.AddComponent<SpriteRenderer>();
        spr.sprite = sprite;
        spr.sortingOrder = order;

        return obj;
    }
    protected GameObject createSpriteObject(string name, Sprite sprite, int order, MonoBehaviour parent, 
    Vector3 position = default(Vector3), Vector3 rotation = default(Vector3), Vector3 scale = default(Vector3)){
        return createSpriteObject(name, sprite, order, parent.transform, position, rotation, scale);
    }
    protected GameObject createSpriteObject(string name, Sprite sprite, int order, GameObject parent, 
    Vector3 position = default(Vector3), Vector3 rotation = default(Vector3), Vector3 scale = default(Vector3)){
        return createSpriteObject(name, sprite, order, parent.transform, position, rotation, scale);
    }
    

    #endregion

    #region ---------------------------- UNITY CALLBACKS

    //reset weapon if get/drop weapon item
    public virtual void reload(){
        calculateUseRate();
    }
    public virtual void stop(){
        canUse = true;
        t = 0.0f;
    }

    protected virtual void Awake()
    {
        canUse = true;
        _UseRateDefault = 0.125f;
        useRateValues = new float[] { 1.0f, 2.5f };
        calculateUseRate();
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
    #endregion

    #region ---------------------------- METHODS

    //set userate from 'default' and 'values'
    protected virtual void calculateUseRate(){
        _UseRate = _UseRateDefault / useRateValues[useRateIndex];
    }

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

        calculateUseRate();

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

    #endregion
}
