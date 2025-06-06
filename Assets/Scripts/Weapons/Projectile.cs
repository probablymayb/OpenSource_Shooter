﻿using Photon.Pun;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// To shoot a projectile towards the current body direction using Transform.Translate
/// and a public speed variable. It also checks for 2D trigger collisions.
/// </summary>
public class Projectile : MonoBehaviourPun
{
    // --------------------------------------
    // - 2D TopDown Isometric Shooter Study -
    // ----------- by Tadadosi --------------
    // --------------------------------------
    // ---- https://twitter.com/tadadosi ----
    // --------------------------------------

    // NOTE: Remove destroy actions if a pool system is implemented

    #region ---------------------------- PROPERTIES

    public float Speed { get { return _Speed; } set {  _Speed = value; } }
    [SerializeField] private float _Speed = 20f;

    public float LifeTime { get { return _LifeTime; } set { _LifeTime = value; } }
    [SerializeField] private float _LifeTime = 1f;

    [Header("FXs")]
    [SerializeField] private GameObject hitPFX = null;

    private BoxCollider2D _Collider;
    private SpriteRenderer _Renderer;
    private SoundHandlerLocal _Sfx;
    private Transform cloneProjectiles_t;

    private Vector3 travelDirection;
    private float movement;
    private bool hasLaunched;
    private int impactCount;

    public bool isRPCFire = false;

    #endregion

    #region ---------------------------- UNITY CALLBACKS
    private void Awake()
    {
        TryGetComponent(out _Sfx);
        TryGetComponent(out _Renderer);
        TryGetComponent(out _Collider);


            if (_Collider == null)
            {
                _Collider = gameObject.AddComponent<BoxCollider2D>();
                _Collider.isTrigger = true;
                // 크기 설정
                _Collider.size = new Vector2(0.5f, 0.2f);
                Debug.LogWarning($"[Projectile] Collider was missing, added new one!");
            }

        cloneProjectiles_t = GameObject.Find("CloneProjectiles").transform;

    }

    private void Start()
    {
       
            if (PhotonNetwork.InRoom && _Collider == null)
            {
                _Collider = GetComponent<BoxCollider2D>();
                if (_Collider == null)
                {
                    _Collider = gameObject.AddComponent<BoxCollider2D>();
                    _Collider.isTrigger = true;
                    _Collider.size = new Vector2(0.5f, 0.2f);
                    Debug.LogError("[Projectile] Collider missing after network instantiation, added!");
                }
            }

            // 기존 경고 메시지
            if (_Sfx == null || _Renderer == null || _Collider == null)
            {
                Debug.LogWarning(gameObject.name + ": BoxCollider2D || SpriteRenderer || SoundFXHandler!");
            }
    }

    private void Update()
    {
        // 내 총알일 때만 이동
        if (PhotonNetwork.InRoom)
        {
            if (!photonView.IsMine) return;

            if (hasLaunched)
                Travel();
        }
        else
        {
            if (hasLaunched)
                Travel();
        }
    }
    #endregion

    #region ---------------------------- METHODS
    /// <summary>
    /// To enable or disable this projectile <see cref="SpriteRenderer"/> and <see cref="BoxCollider2D"/>.
    /// </summary>
    /// <param name="value"></param>
    public void SetActive(bool value)
    {
        _Renderer.enabled = _Collider.enabled = value;
    }

    //set projectile launch sound volume
    public void setVolume(float volume){
        _Sfx.setVolume(volume);
    }

    /// <summary>
    /// To launch the projectile towards <see cref="travelDirection"/>.
    /// </summary>
    
    //네트워크가 없을 때 실행되는 함수
    public void Fire(bool isRight){
        this.Fire(0, isRight);
    }
    public void Fire(float rotateDiff, bool isRight)
    {
        if(isRPCFire) {
            photonView.RPC("RPC_Fire", RpcTarget.All, rotateDiff, isRight);
            Debug.Log($"[DEBUG] PhotonNetwork.IsConnected: {PhotonNetwork.IsConnected}, IsConnectedAndReady: {PhotonNetwork.IsConnectedAndReady}, InRoom: {PhotonNetwork.InRoom}");
            Debug.Log($"[DEBUG] PhotonManager._currentPhase : {PhotonManager._currentPhase}");
        }
        else {
            Debug.Log($"[DEBUG] PhotonNetwork.IsConnected: {PhotonNetwork.IsConnected}, IsConnectedAndReady: {PhotonNetwork.IsConnectedAndReady}, InRoom: {PhotonNetwork.InRoom}");
            Debug.Log($"[DEBUG] PhotonManager._currentPhase : {PhotonManager._currentPhase}");
            /*GameObject clonedProjectile = Instantiate(gameObject, transform.position, transform.rotation);
            Projectile clonedProjectileScript = clonedProjectile.GetComponent<Projectile>();
            //clonedProjectileScript.setVolume(_Sfx.getVolume());
            clonedProjectileScript.Fire_Cloned(rotateDiff);*/
        }

    }

    private void Fire_Cloned(float rotateDiff){
        bool isRight = PlayerBodyPartsHandler.isRightDirection;

        //총알이 보이도록 설정
        SetActive(true);
        hasLaunched = true;

        //방향 설정
        travelDirection = isRight ? Vector3.right : -Vector3.right;
        Quaternion rotate = Quaternion.Euler(0, 0, rotateDiff);
        transform.rotation = rotate * transform.rotation;

        transform.parent = null;
        _Sfx.PlaySound(0);

        Destroy(gameObject, LifeTime);
    }
    //네트워크가 있을 때 실행되는 함수
    [PunRPC]
    private void RPC_Fire(float rotateDiff, bool isRight)
    {
        transform.SetParent(cloneProjectiles_t);
        tag = "clone";
        setVolume(_Sfx.getVolume());
        RPC_Fire_Cloned(rotateDiff, isRight);
    }

    [PunRPC]
    private void RPC_Fire_Cloned(float rotateDiff, bool isRight)
    {
        SetActive(true);
        hasLaunched = true;

        //방향 설정
        travelDirection = isRight ? Vector3.right : -Vector3.right;
        Quaternion rotate = Quaternion.Euler(0, 0, rotateDiff);
        transform.rotation = rotate * transform.rotation;

        _Sfx.PlaySound(0);

        //주의: IsMine인 쪽만 Destroy() 예약
        if (photonView.IsMine)
            DestroyAfterLifetimeAsync().Forget();
    }

    private async UniTaskVoid DestroyAfterLifetimeAsync()
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(LifeTime));

            if (photonView != null && photonView.IsMine)
                PhotonNetwork.Destroy(gameObject);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Projectile] DestroyAfterLifetimeAsync 예외 발생: {e.Message}");
        }
    }


    /// <summary>
    /// Moves the projectile towards <see cref="travelDirection"/> using transform.Translate.
    /// </summary>
    private void Travel()
    {
        movement = Time.deltaTime * Speed;
        transform.Translate(travelDirection.normalized * movement, Space.Self);
    }
    #endregion

    #region ---------------------------- COLLISIONS
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine || impactCount > 0)
            return;

        Debug.Log(" BVullet Triggered");

        // 태그 대신 PlayerStatus 컴포넌트로 확인
        PlayerStatus playerStatus = collision.GetComponent<PlayerStatus>();
        if (playerStatus == null && collision.transform.parent != null)
        {
            playerStatus = collision.GetComponentInParent<PlayerStatus>();
        }

        if (playerStatus != null)
        {
            PhotonView targetView = playerStatus.GetComponent<PhotonView>();
            if (targetView != null && !targetView.IsMine)
            {
                if (targetView != null && !targetView.IsMine)
                {
                    Debug.Log($"Sending RPC to {targetView.Owner.NickName}");

                    Debug.Log("Player Damage trigger" + 10);
                    targetView.RPC("TakeDamage", targetView.Owner, 10); // 피해 전달
                    PhotonNetwork.Instantiate("Particles/" + hitPFX.name, transform.position, Quaternion.identity);
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }

        if (collision.CompareTag("Wall"))
        {
            impactCount++;

            PhotonNetwork.Instantiate("Particles/" + hitPFX.name, collision.ClosestPoint(transform.position), Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }

        //if (collision.CompareTag("Player"))
        //{
        //    Debug.Log($"Player hit - GameObject: {collision.gameObject.name}");
        //    Debug.Log($"Has PhotonView: {collision.GetComponent<PhotonView>() != null}");
        //    Debug.Log($"Has PlayerStatus: {collision.GetComponent<PlayerStatus>() != null}");

        //    PhotonView targetView = collision.GetComponent<PhotonView>();
        //    if (targetView != null && !targetView.IsMine)
        //    {
        //        Debug.Log($"Sending RPC to {targetView.Owner.NickName}");

        //        Debug.Log("Player Damage trigger" + 10);
        //        targetView.RPC("TakeDamage", targetView.Owner, 10); // 피해 전달
        //        PhotonNetwork.Instantiate("Particles/" + hitPFX.name, transform.position, Quaternion.identity);
        //        PhotonNetwork.Destroy(gameObject);
        //    }
        //}
    }
    #endregion
}
