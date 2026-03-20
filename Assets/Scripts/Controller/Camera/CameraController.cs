using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    #region Value

    [Space(10)]
    [Header("=== Camera")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private List<Transform> cameraElementTransformList;
    /*
    0. 플레이어를 따라가는 객체
    1. 공격 시 화면 흔들림
    2. 적이 죽을 때 흔들림
    3. 공격 받을 시 흔들림
    */
    [HideInInspector] private Transform followTargetTF;
    [HideInInspector] private Transform shotShakeTF;
    [HideInInspector] private Transform enemyKillShakeTF;
    [HideInInspector] private Transform damagedShakeTF;

    [Header("-- Follow Target")]
    [SerializeField] public Transform targetTF;
    [SerializeField] private float followSpeed = 10f;

    [Header("-- Shot Shake")]
    [SerializeField] private float shotStrength = 1f;
    [SerializeField] private int shotVibrato = 1;

    [Header("-- Kill Shake")]
    [SerializeField] private float killStrength = 10f;
    [SerializeField] private int killVibrato = 10;

    [HideInInspector] private float cameraProjectionSize = 6f;

    #endregion

    #region Offset

    private void Offset()
    {
        Offset_SetVariable();
        Offset_State();
    }

    private void Offset_SetVariable()
    {
        cameraElementTransformList = new List<Transform>();
        foreach (Transform child in transform)
        {
            cameraElementTransformList.Add(child);
        }

        followTargetTF = cameraElementTransformList[0];
        shotShakeTF = cameraElementTransformList[1];
        enemyKillShakeTF = cameraElementTransformList[2];
        damagedShakeTF = cameraElementTransformList[3];
    }

    private void Offset_State()
    {
        cameraProjectionSize = mainCamera.orthographicSize;
    }

    #endregion

    #region  Framework

    private void Start()
    {
        Offset();
    }

    private void LateUpdate()
    {
        Set_FollowTargetSmooth(targetTF);

        mainCamera.transform.position = Get_TotalCameraPos();
    }

    #endregion

    #region Get

    private Vector3 Get_TotalCameraPos()
    {
        Vector2 totalPos = followTargetTF.position;

        DevTool.Set_ListDele(
            cameraElementTransformList,
            new Dele_RefT_U<Vector2, Transform>(Add_PosValue), 
            ref totalPos, 1);

        return new Vector3(totalPos.x, totalPos.y, -10f);
    }

    private void Add_PosValue(ref Vector2 refVariable, Transform value)
    {
        DevTool.Add_RefValue(ref refVariable, (Vector2)value.position);
    }

    #endregion

    #region Follow Target

    private void Set_FollowTargetSmooth(Transform targetTF)
    {
        Vector2 originPos = followTargetTF.position;

        Vector2 targetPos = targetTF.position;

        followTargetTF.position = Vector2.Lerp(
            originPos,
            targetPos,
            followSpeed * Time.deltaTime);
    }

    #endregion

    #region When?

    // 발사
    public void Play_ShotAnim(float dur, float strength)
    {
        Play_Shake(shotShakeTF, dur, strength * shotStrength, shotVibrato);
    }

    // 적 타격
    public void Play_HitEnemyAnim()
    {
        Play_POVSize(0.3f, cameraProjectionSize - 0.05f);
    }

    // 적 처치
    public void Play_KillAnim(float dur)
    {
        Play_Shake(enemyKillShakeTF, dur, killStrength, killVibrato);
        Play_POVSize(0.4f, cameraProjectionSize - 0.1f);
    }

    // 회피
    public void Play_AvoidAnim(float dur)
    {
        Play_SlowMotion(dur * 2, 0.5f);
        Play_POVSize(dur, cameraProjectionSize - 1f);
    }

    // 피격
    public void Play_DamagedAnim(float dur, float strength, Vector2 dir)
    {
        Play_Rebound(damagedShakeTF, dur, strength, dir);
        Play_SlowMotion(dur * 2, 0.5f);
        Play_POVSize(dur, cameraProjectionSize + 0.75f);
    }

    #endregion

    #region Module

    private void Play_Shake(Transform tf, float dur, float strength, int vibrato)
    {
        strength = Mathf.Min(strength, 20f) * 0.01f;
        tf.DOShakePosition(dur, strength, vibrato, 0f);
    }

    private void Play_Rebound(Transform tf, float dur, float strength, Vector2 dir)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(tf.DOMove(dir * strength, dur / 5).SetEase(Ease.OutBack));
        seq.Append(tf.DOMove(-dir * strength * 0.5f, dur / 5).SetEase(Ease.OutBack));
        seq.Append(tf.DOMove(Vector2.zero, dur * 3 / 5).SetEase(Ease.OutBack));
    }

    private Sequence SlowMotionSeq = null;
    private void Play_SlowMotion(float dur, float slowMultiple)
    {
        Stop_SlowMotion();

        SlowMotionSeq = DOTween.Sequence();
        Time.timeScale = slowMultiple;
        SlowMotionSeq.AppendInterval(dur);
        SlowMotionSeq.SetUpdate(true)
            .OnComplete(() =>
            {
                if (Time.timeScale != 1)
                    Time.timeScale = 1;

                PlayerManager.instance.playerController.SetOff_Invincible();
            });
    }

    public void Stop_SlowMotion()
    {
        DevTool.Set_KillTween(SlowMotionSeq); 
        Time.timeScale = 1;
    }

    private void Play_POVSize(float dur, float projectionSize)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => mainCamera.orthographicSize, x => mainCamera.orthographicSize = x, projectionSize, dur / 4));
        seq.Append(DOTween.To(() => mainCamera.orthographicSize, x => mainCamera.orthographicSize = x, cameraProjectionSize, dur * 3 / 4));

    }

    #endregion

}
