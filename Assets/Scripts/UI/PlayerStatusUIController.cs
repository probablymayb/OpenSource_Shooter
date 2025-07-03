using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;
using Photon.Pun;

public class PlayerStatusUIController : MonoBehaviour
{
    private PhotonView _photonView;
    
    void Awake()
    {
        _photonView = GetComponentInParent<PhotonView>();
    }

    void Start()
    {
        if (_photonView == null) _photonView = GetComponentInParent<PhotonView>();

        if (!_photonView.IsMine)
        {
            Debug.Log("[PlayerStatusUIController] 이 캐릭터는 내 것이 아님. UI 조정 시작.");

            GameObject myAvatar = InGamePhotonManager.InGame.GetMyAvatar();
            if (myAvatar != null)
            {
                var myUI = myAvatar.GetComponentInChildren<PlayerStatusUIController>();
                if (myUI != null)
                {
                    float myX = myUI.GetComponent<RectTransform>().anchoredPosition.x;

                    RectTransform rect = GetComponent<RectTransform>();
                    Debug.Log(rect.transform.parent.name);
                    rect.anchoredPosition = new Vector2(-myX, rect.anchoredPosition.y);

                    Image img = GetComponentInChildren<Image>();
                    if (img != null)
                    {
                        img.color = Color.red;
                        Debug.Log("→ HP 바 색상 변경 완료");
                    }

                    Debug.Log($"→ 위치 조정 완료: x={-myX}");
                }
            }
            else
            {
                Debug.LogWarning("→ 내 아바타를 찾지 못했습니다.");
            }
        }
        else
        {
            Debug.Log("[PlayerStatusUIController] 이건 내 캐릭터입니다. UI 변경 없음.");
        }
    }

}
