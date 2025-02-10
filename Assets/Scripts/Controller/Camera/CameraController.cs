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
    /*
    0. 플레이어를 따라가는 객체
    1. 공격 시 화면 흔들림
    2. 적이 죽을 때 흔들림
    3. 공격 받을 시 흔들림
    */
    [HideInInspector] private Transform FollowTargetTF;
    [HideInInspector] private Transform ShotShakeTF;
    [HideInInspector] private Transform EnemyKillShakeTF;
    [HideInInspector] private Transform DamagedShakeTF;

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

    [HideInInspector] private float CameraProjectionSize = 6f;

    #endregion

    #region  Framework

    private void Start()
    {
        CameraElementTransformList = new List<Transform>();
        foreach (Transform child in transform)
        {
            CameraElementTransformList.Add(child);
        }

        FollowTargetTF = CameraElementTransformList[0];
        ShotShakeTF = CameraElementTransformList[1];
        EnemyKillShakeTF = CameraElementTransformList[2];
        DamagedShakeTF = CameraElementTransformList[3];

        CameraProjectionSize = MainCamera.orthographicSize;
    }

    private void LateUpdate()
    {
        /*if (StageManager.Instance.CurrentRoomController != null)
        {
            FollowTargetSmooth(TargetTF, 
                StageManager.Instance.CurrentRoomController.RoomCameraCenter);
        }
        else*/

        FollowTargetSmooth(TargetTF);
        FollowTargetRangeLimit();

        MainCamera.transform.position = GetTotalCameraPos();
    }

    #endregion

    #region Camera

    private Vector3 GetTotalCameraPos()
    {
        Vector2 totalPos = FollowTargetTF.position;

        for (int i = 1; i < CameraElementTransformList.Count; i++)
        {
            totalPos += (Vector2)CameraElementTransformList[i].position;
        }

        return new Vector3(totalPos.x, totalPos.y, -10f);
    }

    #endregion

    #region Follow Target

    private void FollowTargetSmooth(Transform _TargetTF)
    {
        Vector2 originPos = FollowTargetTF.position;

        Vector2 targetPos = _TargetTF.position;

        FollowTargetTF.position = Vector2.Lerp(
            originPos,
            targetPos,
            FollowSpeed * Time.deltaTime);
    }

    private void FollowTargetSmooth(Transform _TargetTF, Transform _TargetTF2)
    {
        Vector2 originPos = FollowTargetTF.position;

        Vector2 targetPos = 
            (((Vector2)_TargetTF.position * 10) + (Vector2)_TargetTF2.position) / 10;

        FollowTargetTF.position = Vector2.Lerp(
            originPos,
            targetPos,
            FollowSpeed * Time.deltaTime);
    }

    private void FollowTargetRangeLimit()
    {
        Vector2 cameraPos = FollowTargetTF.position;
        Vector2 playerPos = TargetTF.transform.position;

        float dis = Vector2.Distance(cameraPos, playerPos);
        if (dis > FollowRangeLimit)
        {
            Vector2 dir = (cameraPos - playerPos).normalized;
            Vector3 targetPos = playerPos + (dir * FollowRangeLimit);
            targetPos.z = -10;
            FollowTargetTF.position = targetPos;
        }
    }

    #endregion

    #region Shot

    public void PlayShotAnim(float _Dur, float _Strength)
    {
        PlayShake(ShotShakeTF, _Dur, _Strength * ShotStrength, ShotVibrato);
    }

    #endregion

    #region Execution Kill

    public void PlayHitEnemyAnim()
    {
        PlaySlowMotion(0.3f, 0.95f);
        PlayPOVSize(0.3f, CameraProjectionSize - 0.05f);
    }

    public void PlayKillAnim(float _Dur)
    {
        PlayShake(EnemyKillShakeTF, _Dur, KillStrength, KillVibrato);
        PlaySlowMotion(0.4f, 0.9f);
        PlayPOVSize(0.4f, CameraProjectionSize - 0.1f);
    }

    #endregion

    #region Dmg

    public void PlayAvoidAnim(float _Dur)
    {
        PlaySlowMotion(_Dur * 0.8f, 0.5f);
        PlayPOVSize(_Dur, CameraProjectionSize - 1f);
    }

    public void PlayDamagedAnim(float _Dur, float _Strength, Vector2 _Dir)
    {
        PlayRebound(DamagedShakeTF, _Dur, _Strength, _Dir);
        PlaySlowMotion(_Dur * 0.5f, 0.25f);
        PlayPOVSize(_Dur, CameraProjectionSize + 0.75f);
    }



    #endregion

    #region Module

    private void PlayShake(Transform _TF, float _Dur, float _Strength, int _Vibrato)
    {
        _TF.DOShakePosition(_Dur, _Strength / 100, _Vibrato, 0f);
    }

    private void PlayRebound(Transform _TF, float _Dur, float _Strength, Vector2 _Dir)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_TF.DOMove(_Dir * _Strength, _Dur / 5).SetEase(Ease.OutBack));
        seq.Append(_TF.DOMove(-_Dir * _Strength * 0.5f, _Dur / 5).SetEase(Ease.OutBack));
        seq.Append(_TF.DOMove(Vector2.zero, _Dur * 3 / 5).SetEase(Ease.OutBack));
    }

    private void PlaySlowMotion(float _Dur, float _SlowMultiple)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, _SlowMultiple, _Dur / 4));
        seq.AppendInterval(_Dur / 2);
        seq.Append(DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, _Dur / 4));
        seq.SetUpdate(true)
            .OnComplete(() =>
            {
                if (Time.timeScale != 1)
                {
                    Time.timeScale = 1;
                }
            });
    }

    private void PlayPOVSize(float _Dur, float _CameraPojectionSize)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => MainCamera.orthographicSize, x => MainCamera.orthographicSize = x, _CameraPojectionSize, _Dur / 4));
        seq.Append(DOTween.To(() => MainCamera.orthographicSize, x => MainCamera.orthographicSize = x, CameraProjectionSize, _Dur * 3 / 4));

    }

    #endregion

}
