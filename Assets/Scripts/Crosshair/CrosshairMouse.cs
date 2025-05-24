using UnityEngine;
using Photon.Pun;

/// <summary>
/// This class derives from the base class Crosshair. It overrides the method UpdateCrosshair 
/// to snap the crosshair position to <see cref="TadaInput.MouseWorldPos"/> for the local player only.
/// </summary>
public class CrosshairMouse : Crosshair
{
    [TextArea(3, 10)]
    public string notes = "This class derives from the base class Crosshair. It overrides the method UpdateCrosshair " +
    "to snap the crosshair position to TadaInput.MouseWorldPos";

    private Transform player;
    private PhotonView photonView;

    /// <summary>
    /// Vector that goes from player position to crosshair world position.
    /// </summary>
    public static Vector3 AimDirection
    {
        get { return _AimDirection; }
        private set { _AimDirection = value; }
    }
    private static Vector3 _AimDirection;

    protected override void Awake()
    {
        base.Awake();

        photonView = GetComponentInParent<PhotonView>();

        // 내 아바타일 경우에만 player 설정
        if (photonView != null && photonView.IsMine)
        {
            player = GetComponentInParent<PlayerController>()?.transform;
        }
    }

    public override void UpdateCrosshair()
    {
        // 내 아바타가 아니면 크로스헤어 업데이트 금지
        if (photonView == null || !photonView.IsMine)
            return;

        base.UpdateCrosshair();

        crosshair.transform.position = TadaInput.MouseWorldPos;

        _AimDirection = (crosshair.transform.position - player.position).normalized;
        _AimDirection.z = 0f;
    }
}
