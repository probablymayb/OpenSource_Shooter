using Photon.Pun;
using UnityEngine;

/// <summary>
/// This class has conditions to determine which of the crosshairs should be enabled and updating. 
/// </summary>
public class CrosshairHandler : MonoBehaviour
{
    // --------------------------------------
    // ----- 2D Isometric Shooter Study -----
    // ----------- by Tadadosi --------------
    // --------------------------------------
    // ---- Support my work by following ----
    // ---- https://twitter.com/tadadosi ----
    // --------------------------------------

    [TextArea(3, 10)]
    public string notes = "This class has conditions to determine which of the crosshairs should be enabled and updating.";

    private CrosshairMouse mouseCrosshair;
    private PhotonView photonView;

    private bool isReady;

    private void Awake()
    {
        photonView = transform.root.GetComponent<PhotonView>();
        mouseCrosshair = transform.root.GetComponentInChildren<CrosshairMouse>();
    }

    private void Update()
    {
        if (photonView == null || !photonView.IsMine) return;

        CheckIfReady();

        if (!TadaInput.IsMouseActive)
        {
            if (mouseCrosshair.IsActive)
            {
                mouseCrosshair.IsActive = false;
            }
        }
        else
        {
            if (!mouseCrosshair.IsActive)
            {
                mouseCrosshair.IsActive = true;
            }

            mouseCrosshair.UpdateCrosshair();
        }
    }

    private void CheckIfReady()
    {
        if (!isReady)
        {
            if (mouseCrosshair == null)
            {
                Debug.LogError(gameObject.name + ": mouse crosshair missing!");
                return;
            }

            isReady = true;
        }
    }
}
