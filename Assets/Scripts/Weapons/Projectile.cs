using Photon.Pun;
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

    private Vector3 travelDirection;
    private float movement;
    private bool hasLaunched;
    private int impactCount;

    #endregion

    #region ---------------------------- UNITY CALLBACKS
    private void Awake()
    {
        TryGetComponent(out _Sfx);
        TryGetComponent(out _Renderer);
        TryGetComponent(out _Collider);
    }

    private void Start()
    {
        // Used to get a message on the console if any important component is missing.
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

    public void Fire(bool isRight)
    {
        photonView.RPC("RPC_Fire", RpcTarget.All, isRight);
    }

    [PunRPC]
    private void RPC_Fire(bool isRight)
    {
        hasLaunched = true;
        travelDirection = isRight ? Vector3.right : -Vector3.right;
        transform.parent = null;
        _Sfx?.PlaySound(0);

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
    /// To launch the projectile towards <see cref="travelDirection"/>.
    /// </summary>
    public void Fire()
    {
        //Debug.Log("Projectile: Fire()");
        hasLaunched = true;
        _Sfx.PlaySound(0);

        // Set travel direction based on the current direction of the body
        if (!PlayerBodyPartsHandler.isRightDirection)
            travelDirection = -Vector3.right;
        else
            travelDirection = Vector3.right;

        transform.parent = null;
        Destroy(gameObject, LifeTime);
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

        if (collision.CompareTag("Wall"))
        {
            impactCount++;

            PhotonNetwork.Instantiate("Particles/" + hitPFX.name, collision.ClosestPoint(transform.position), Quaternion.identity);
            PhotonNetwork.Destroy(gameObject); // 🧨 Destroy 대신 사용
        }

        if (collision.CompareTag("Player"))
        {
            PhotonView targetView = collision.GetComponent<PhotonView>();
            if (targetView != null && !targetView.IsMine)
            {
                targetView.RPC("TakeDamage", targetView.Owner, 10); // 피해 전달
                PhotonNetwork.Instantiate("Particles/" + hitPFX.name, transform.position, Quaternion.identity);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    #endregion
}
