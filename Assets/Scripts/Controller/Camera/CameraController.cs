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

    [Header("-- Shot Shake")]
    [SerializeField] private float ShotStrength = 1f;
    [SerializeField] private int ShotVibrato = 1;

    [Header("-- Kill Shake")]
    [SerializeField] private float KillStrength = 1f;
    [SerializeField] private int KillVibrato = 1;

    [HideInInspector] private float CameraProjectionSize = 6f;

    #endregion

    #region Offset

    private void Offset()
    {
        Offset_SetVariable();
        Offset_State();
    }

    private void Offset_SetVariable()
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
    }

    private void Offset_State()
    {
        CameraProjectionSize = MainCamera.orthographicSize;
    }

    #endregion

    #region  Framework

    private void Start()
    {
        Offset();
    }

    private void LateUpdate()
    {
        Set_FollowTargetSmooth(TargetTF);

        MainCamera.transform.position = Get_TotalCameraPos();
    }

    #endregion

    #region Get

    private Vector3 Get_TotalCameraPos()
    {
        Vector2 totalPos = FollowTargetTF.position;

        DevTool.Set_ListDele(
            CameraElementTransformList,
            new Dele_RefT_U<Vector2, Transform>(Add_PosValue), 
            ref totalPos, 1);

        return new Vector3(totalPos.x, totalPos.y, -10f);
    }

    private void Add_PosValue(ref Vector2 _Variable, Transform _Value)
    {
        DevTool.Add_RefValue(ref _Variable, (Vector2)_Value.position);
    }

    #endregion

    #region Follow Target

    private void Set_FollowTargetSmooth(Transform _TargetTF)
    {
        Vector2 originPos = FollowTargetTF.position;

        Vector2 targetPos = _TargetTF.position;

        FollowTargetTF.position = Vector2.Lerp(
            originPos,
            targetPos,
            FollowSpeed * Time.deltaTime);
    }

    #endregion

    #region When?

    // 발사
    public void Play_ShotAnim(float _Dur, float _Strength)
    {
        Play_Shake(ShotShakeTF, _Dur, _Strength * ShotStrength, ShotVibrato);
    }

    // 적 타격
    public void Play_HitEnemyAnim()
    {
        Play_POVSize(0.3f, CameraProjectionSize - 0.05f);
    }

    // 적 처치
    public void Play_KillAnim(float _Dur)
    {
        Play_Shake(EnemyKillShakeTF, _Dur, KillStrength, KillVibrato);
        Play_POVSize(0.4f, CameraProjectionSize - 0.1f);
    }

    // 회피
    public void Play_AvoidAnim(float _Dur)
    {
        Play_SlowMotion(_Dur * 2, 0.5f);
        Play_POVSize(_Dur, CameraProjectionSize - 1f);
    }

    // 피격
    public void Play_DamagedAnim(float _Dur, float _Strength, Vector2 _Dir)
    {
        Play_Rebound(DamagedShakeTF, _Dur, _Strength, _Dir);
        Play_SlowMotion(_Dur * 2, 0.5f);
        Play_POVSize(_Dur, CameraProjectionSize + 0.75f);
    }

    #endregion

    #region Module

    private void Play_Shake(Transform _TF, float _Dur, float _Strength, int _Vibrato)
    {
        _Strength = Mathf.Min(_Strength, 20f) * 0.01f;
        _TF.DOShakePosition(_Dur, _Strength, _Vibrato, 0f);
    }

    private void Play_Rebound(Transform _TF, float _Dur, float _Strength, Vector2 _Dir)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_TF.DOMove(_Dir * _Strength, _Dur / 5).SetEase(Ease.OutBack));
        seq.Append(_TF.DOMove(-_Dir * _Strength * 0.5f, _Dur / 5).SetEase(Ease.OutBack));
        seq.Append(_TF.DOMove(Vector2.zero, _Dur * 3 / 5).SetEase(Ease.OutBack));
    }

    private void Play_SlowMotion(float _Dur, float _SlowMultiple)
    {
        Sequence seq = DOTween.Sequence();
        Time.timeScale = _SlowMultiple;
        seq.AppendInterval(_Dur);
        seq.SetUpdate(true)
            .OnComplete(() =>
            {
                if (Time.timeScale != 1)
                    Time.timeScale = 1;

                PlayerManager.Instance.PlayerController.SetOff_Invincible();
            });
    }

    private void Play_POVSize(float _Dur, float _PojectionSize)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => MainCamera.orthographicSize, x => MainCamera.orthographicSize = x, _PojectionSize, _Dur / 4));
        seq.Append(DOTween.To(() => MainCamera.orthographicSize, x => MainCamera.orthographicSize = x, CameraProjectionSize, _Dur * 3 / 4));

    }

    #endregion

}
