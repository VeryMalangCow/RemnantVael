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
    [SerializeField] private float ShotStrength = 1f;
    [SerializeField] private int ShotVibrato = 1;

    [Header("-- Kill Shake")]
    [SerializeField] private float KillStrength = 1f;
    [SerializeField] private int KillVibrato = 1;

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
        PlayShake(CameraElementTransformList[1], _Dur, _Strength * ShotStrength, ShotVibrato);
    }

    #endregion

    #region Execution Kill Shake

    public void PlayKillShake(float _Dur)
    {
        PlayShake(CameraElementTransformList[2], _Dur, KillStrength, KillVibrato);
    }

    #endregion

    private void PlayShake(Transform _TF, float _Dur, float _Strength, int _Vibrato)
    {
        _TF.DOShakePosition(_Dur, _Strength / 100, _Vibrato, 0f);
    }
}
