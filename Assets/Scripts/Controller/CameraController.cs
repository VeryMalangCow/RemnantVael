using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    #region Value

    [Header("=== Move")]
    [HideInInspector] public Transform TargetTF;
    [SerializeField] private float followSpeed = 4f;
    [SerializeField] private float followRangeLimit = 1f;

    #endregion

    #region  Framework

    public override void Offset()
    {
        TargetTF = PlayerController.Instance.gameObject.transform;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void LateUpdate()
    {
        FollowTargetSmooth(TargetTF);
        FollowTargetRangeLimit();
    }

    #endregion

    #region Move

    private void FollowTargetSmooth(Transform targetTF)
    {
        Vector3 originPos = this.transform.position;
        Vector3 targetPos = targetTF.position;
        targetPos.z = originPos.z;

        transform.position = Vector3.Lerp(
            originPos,
            targetPos,
            followSpeed * Time.deltaTime);

    }

    private void FollowTargetRangeLimit()
    {
        Vector2 cameraPos = this.transform.position;
        Vector2 playerPos = TargetTF.transform.position;

        float dis = Vector2.Distance(cameraPos, playerPos);
        if (dis > followRangeLimit)
        {
            Vector2 dir = (cameraPos - playerPos).normalized;
            Vector3 targetPos = playerPos + (dir * followRangeLimit);
            targetPos.z = -10;
            transform.position = targetPos;
        }
    }

    #endregion
}
