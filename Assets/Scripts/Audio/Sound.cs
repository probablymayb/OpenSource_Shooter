using UnityEngine;

[System.Serializable]
public class Sound
{
    // NOTE: To show this kind of classes in the Inspector, make sure you add 
    // [System.Serializable] and also remove the Monobehaviour type.

    public string notes;
    public AudioClip clip;

    [Range(0f, 1f)]
    [SerializeField] private float _Volume = 1f;
    public float Volume { get { return _Volume; } private set { _Volume = value; } }

    [Range(0f, 1f)]
    [SerializeField] private float _MinPitch = 1f;
    public float MinPitch { get { return _MinPitch; } private set { _MinPitch = value; } }

    [Range(0f, 1f)]
    [SerializeField] private float _MaxPitch = 1f;
    public float MaxPitch { get { return _MaxPitch; } private set { _MaxPitch = value; } }

    public void SetComp(float volume, float minPitch, float maxPitch){
        if(volume != -1){
            if(volume < 0) this._Volume = 0;
            else if(volume > 1) this._Volume = 1;
            else this._Volume = volume;
        }
        if(minPitch != -1){
            if(minPitch < 0) this._MinPitch = 0;
            else if(minPitch > 1) this._MinPitch = 1;
            else this._MinPitch = minPitch;
        }
        if(maxPitch != -1){
            if(maxPitch < 0) this._MaxPitch = 0;
            else if(maxPitch > 1) this._MaxPitch = 1;
            else this._MaxPitch = maxPitch;
        }
    }
}
