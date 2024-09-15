using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Value

    [Space(10)]
    [Header("=== Camera")]
    [SerializeField] private Camera MainCamera;
    [SerializeField] private List<Transform> CameraElementTransformList;

    [Header("-- Follow Target")]
    [SerializeField] public Transform TargetTF;
    [SerializeField] private float FollowSpeed = 4f;
    [SerializeField] private float FollowRangeLimit = 1f;

    [Header("-- Shot Shake")]
    [SerializeField] private float Strength = 1f;
    [SerializeField] private int Vibrato = 1;

    #endregion

    #region  Framework

    private void Start()
    {
        CameraElementTransformList = new List<Transform>();
        foreach (Transform child in transform)
        {
            CameraElementTransformList.Add(child);
        }
    }

    private void LateUpdate()
    {
        FollowTargetSmooth(TargetTF);
        FollowTargetRangeLimit();

        MainCamera.transform.position = GetTotalCameraPos();
    }

    #endregion

    #region Camera

    private Vector3 GetTotalCameraPos()
    {
        Vector2 totalPos = Vector2.zero;

        foreach (Transform cameraElementTransform in CameraElementTransformList)
        {
            totalPos += (Vector2)cameraElementTransform.position;
        }

        return new Vector3(totalPos.x, totalPos.y, -10f);
    }

    #endregion

    #region Follow Target

    private void FollowTargetSmooth(Transform _TargetTF)
    {
        Vector2 originPos = CameraElementTransformList[0].position;
        Vector2 targetPos = _TargetTF.position;

        CameraElementTransformList[0].position = Vector2.Lerp(
            originPos,
            targetPos,
            FollowSpeed * Time.deltaTime);

    }

    private void FollowTargetRangeLimit()
    {
        Vector2 cameraPos = CameraElementTransformList[0].position;
        Vector2 playerPos = TargetTF.transform.position;

        float dis = Vector2.Distance(cameraPos, playerPos);
        if (dis > FollowRangeLimit)
        {
            Vector2 dir = (cameraPos - playerPos).normalized;
            Vector3 targetPos = playerPos + (dir * FollowRangeLimit);
            targetPos.z = -10;
            CameraElementTransformList[0].position = targetPos;
        }
    }

    #endregion

    #region Shot Shake


    public void PlayShotShake(float _Dur, float _Strength)
    {
        CameraElementTransformList[1].DOShakePosition(_Dur, _Strength * Strength / 100, Vibrato, 0f);
    }

    #endregion
}
