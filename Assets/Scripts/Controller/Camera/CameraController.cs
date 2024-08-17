using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Value

    [Space(10)]
    [Header("=== Move")]
    [SerializeField] public Transform TargetTF;
    [SerializeField] private float FollowSpeed = 4f;
    [SerializeField] private float FollowRangeLimit = 1f;

    #endregion

    #region  Framework

    private void LateUpdate()
    {
        FollowTargetSmooth(TargetTF);
        FollowTargetRangeLimit();
    }

    #endregion

    #region Move

    private void FollowTargetSmooth(Transform _TargetTF)
    {
        Vector3 originPos = this.transform.position;
        Vector3 targetPos = _TargetTF.position;
        targetPos.z = originPos.z;

        transform.position = Vector3.Lerp(
            originPos,
            targetPos,
            FollowSpeed * Time.deltaTime);

    }

    private void FollowTargetRangeLimit()
    {
        Vector2 cameraPos = this.transform.position;
        Vector2 playerPos = TargetTF.transform.position;

        float dis = Vector2.Distance(cameraPos, playerPos);
        if (dis > FollowRangeLimit)
        {
            Vector2 dir = (cameraPos - playerPos).normalized;
            Vector3 targetPos = playerPos + (dir * FollowRangeLimit);
            targetPos.z = -10;
            transform.position = targetPos;
        }
    }

    #endregion
}
