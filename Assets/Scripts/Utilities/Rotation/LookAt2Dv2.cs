using UnityEngine;
using Photon.Pun;

/// <summary>
/// New v2. The gameobject that has this component attached will instantly rotate to make its x or y axis look 
/// towards the assigned target or towards mouse world position if a exposed enum is selected. The direction can be
/// inverted by checking isFlipAxis. Also there is an option to disable local update if a linked control is 
/// needed. It can also use a smooth rotation by enabling isSmoothRotationEnable.
/// </summary>
public class LookAt2Dv2 : MonoBehaviour, IPunObservable
{
    [TextArea(4, 10)]
    public string notes = "New v2. The gameobject that has this component attached will instantly rotate to make its x or y axis look " +
        "towards the assigned target or towards mouse world position if a exposed enum is selected. The direction can be inverted by " +
        "checking isFlipAxis. Also there is an option to disable local update if a linked control is needed. It can also use a " +
        "smooth rotation by enabling isSmoothRotationEnable.";

    public enum LookAtTarget { TargetTransform, MouseWorldPosition }
    [SerializeField] private LookAtTarget lookAtTarget = LookAtTarget.TargetTransform;

    [Tooltip("If you are using a Transform, select TargetTransform from lookAtTarget dropdown list.")]
    public Transform targetTransform;

    private enum Axis { X, Y }
    [SerializeField] private Axis axis = Axis.Y;

    [Tooltip("Used when isSmoothRotationEnable is true.")]
    [SerializeField] private float turnRate = 10f;

    [Tooltip("Use to set an initial offset angle or use SetOffsetAngle method to do it via code.")]
    [SerializeField] private float offsetLookAtAngle = 0f;

    [Tooltip("e.g. writing 30 will make the axis have a range of -30 to 30 degrees.")]
    [SerializeField] private float maxAngle = 360f;

    [Tooltip("Check to let this behaviour be run by the local Update() method and Uncheck if you want to call it from any other class by using UpdateLookAt().")]
    [SerializeField] private bool isUpdateCalledLocally = false;

    [Tooltip("Check to smoothly rotate towards target rotation using turnRate as variable.")]
    public bool isSmoothRotationEnable = false;

    [Tooltip("Check to flip the axis and use the negative side to look at")]
    public bool isFlipAxis = false;

    [Header("Debug")]
    [SerializeField] private Color debugColor = Color.green;
    [SerializeField] private bool debug = false;

    private Vector3 targetPosition;
    private Vector3 direction;
    private Vector3 upwardAxis;

    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!isUpdateCalledLocally)
            return;

        // 내 로컬 오브젝트일 때만 회전 계산
        if (photonView != null && photonView.IsMine)
        {
            UpdateLookAt();
        }
    }

    public void UpdateLookAt()
    {
        Vector3 myPosition = transform.position;

        // 대상 위치 계산
        if (lookAtTarget == LookAtTarget.MouseWorldPosition)
        {
            targetPosition = TadaInput.MouseWorldPos;
        }
        else if (lookAtTarget == LookAtTarget.TargetTransform)
        {
            if (targetTransform == null)
            {
                Debug.LogError(gameObject.name + " target missing!");
                return;
            }
            targetPosition = targetTransform.position;
        }

        // Z 위치 보정 (2D 기준)
        targetPosition.z = myPosition.z;

        // 회전 방향 벡터 계산
        direction = (targetPosition - myPosition).normalized;

        // 축에 따라 기준 벡터 계산
        switch (axis)
        {
            case Axis.X:
                upwardAxis = Quaternion.Euler(0, 0, isFlipAxis ? -90 + offsetLookAtAngle : 90 + offsetLookAtAngle) * direction;
                break;

            case Axis.Y:
                upwardAxis = isFlipAxis ? -direction : direction;
                break;
        }

        // 회전 계산
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, upwardAxis);

        if (debug)
        {
            Debug.DrawLine(transform.position, targetPosition, debugColor);
        }

        if (!isSmoothRotationEnable)
        {
            if (Quaternion.Angle(Quaternion.identity, targetRotation) < maxAngle)
                transform.rotation = targetRotation;
        }
        else
        {
            Quaternion rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnRate * Time.deltaTime);
            if (Quaternion.Angle(Quaternion.identity, rotation) < maxAngle)
                transform.rotation = rotation;
        }
    }

    public void SwitchToTarget(LookAtTarget target)
    {
        lookAtTarget = target;
    }

    public void SetOffsetAngle(float value)
    {
        offsetLookAtAngle = value;
    }

    // 🔄 회전 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 내 회전값 전송
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 다른 유저의 회전값 수신
            Quaternion receivedRotation = (Quaternion)stream.ReceiveNext();
            transform.rotation = receivedRotation;
        }
    }
}
