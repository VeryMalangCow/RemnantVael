using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Net;

public class EnemyPattern_Thrust : EnemyPattern
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Thrust")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private AttackerState ThisAS;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float Guiding = 0f;
    [SerializeField] private Vector2 DirForTarget = Vector2.zero;
    [SerializeField] private AnimationClip ThisAC;
    [SerializeField] private float JugeAndTweenTime = 0.5f;
    [SerializeField] private float AnimSpeed = 2.6f;
    [SerializeField] private float Speed = 1.5f;
    [SerializeField] bool LightOn = false;
    [SerializeField] float LightSize = 1f;

    [Space(10)]
    [Header("=== Condition")]
    [SerializeField] private float MaxRange = 1.8f;
    [SerializeField] private float MinRange = 0f;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private List<DepthController> SpawnDepthList;
    [SerializeField] private List<DepthController> BeforeEffectDepthList;
    [SerializeField] private Color BeforeEffectColor;

    [HideInInspector] private bool IsThrusting = false;
    [HideInInspector] private Sequence BeforeEffectSeq;

    #endregion

    #region Framework

    private void OnEnable()
    {
        Reset_EffectComp();
    }

    private void Update()
    {
        if (!IsPlaying)
        {
            StopCoroutine(Play_ThisPattern_Cor());
        }

        if (IsThrusting && Guiding > 0)
        {
            Vector2 targetDir = Vector2.Lerp(
                DirForTarget, 
                DevTool.Get_DirForPlayer(ThisEnemy),
                Guiding * Time.deltaTime);

            if (DevTool.TryGetDirNavMeshEnd(
            ThisEnemy.transform.position,
            targetDir,
            out Vector2 endPoint))
            {
                ThisEnemy.Set_NavDir(endPoint);
                DirForTarget = targetDir;
            }
        }
    }

    #endregion

    #region Can Check

    public override bool Can_PlayPattern()
    {
        if (IsSpecialPattern) return true;

        float forPlayerDis = DevTool.Get_DisForPlayer(ThisEnemy);
        if (forPlayerDis >= MinRange && forPlayerDis < MaxRange)
        {
            return true;
        }

        return false;
    }

    #endregion

    #region Actual

    protected override IEnumerator Play_ThisPattern_Cor()
    {
        Play_BeforeEffect(StartDelay);
        yield return new WaitForSeconds(StartDelay);

        #region Actual

        CurrentRepeatAmount++;

        Vector2 targetDir = DevTool.Get_DirForPlayer(ThisEnemy);
        DirForTarget = targetDir;

        Play_ActualPattern(targetDir);

        #endregion

        Play_AfterEffect(EndDelay);
        yield return new WaitForSeconds(EndDelay);

        Stop_ActualPattern();
        if (CurrentRepeatAmount >= RepeatAmount) // 반복을 마침
        {
            End_Pattern();
            ThisEnemy.Play_Pattern();
        }
        else
        {
            ThisEnemy.CurrentPatternCor = Play_ThisPattern_Cor();
            StartCoroutine(ThisEnemy.CurrentPatternCor);
        }
    }

    protected virtual void Play_ActualPattern(Vector2 _TargetDir)
    {
        IsThrusting = true;

        // Attacker
        for (int i = 0; i < SpawnDepthList.Count; i++)
            Play_ActualPattern_Each(SpawnDepthList[i], _TargetDir);

        if (DevTool.TryGetDirNavMeshEnd(
            ThisEnemy.transform.position, 
            _TargetDir,
            out Vector2 endPoint))
        {
            ThisEnemy.Set_NavDir(endPoint);
            ThisEnemy.Set_MoveSpeed(Speed);
        }

        SoundManager.instance.Play_2D_SFX_EnemyAttack_Random(ThisEnemy.Get_AS(), "Thrust", 2);
    }

    private void Play_ActualPattern_Each(DepthController _Depth, Vector2 _TargetDir)
    {
        EnemyAttackerController attacker = PoolingManager.instance.Get_OP_EnemyAttacker();
        attacker.enemy = ThisEnemy;
        float targetShadow = _Depth.TargetRange;

        attacker.Set_State(
            ThisAS,
            State_Juge(),
            State_Anim(),
            State_StartTF(_TargetDir),
            State_EndTF(_TargetDir),
            targetShadow,
            parent: _Depth.transform,
            isLocal: true);

        if (LightOn)
            attacker.Set_Light(
                LightSize, JugeAndTweenTime);
    }

    private void Stop_ActualPattern()
    {
        IsThrusting = false;
        DirForTarget = Vector2.zero;
        ThisEnemy.Set_NavDir(Vector2.zero);
        ThisEnemy.Set_MoveSpeed(0);
    }

    #endregion

    #region State

    private AttackerState_Juge<CircleCollider2D> State_Juge()
    {
        return new AttackerState_Juge<CircleCollider2D>(Vector2.one);
    }

    private State_Anim State_Anim()
    {
        return new State_Anim(ThisAC, AnimSpeed);
    }

    private State_TF2D State_StartTF(Vector2 _TargetDir)
    {
        return new State_TF2D(
            Vector2.zero,
            DevTool.Get_RotFromDir(_TargetDir),
            Vector2.one);
    }

    private AttackerState_EndTF State_EndTF(Vector2 _TargetDir)
    {
        return new AttackerState_EndTF(
            Vector2.zero,
            DevTool.Get_RotFromDir(_TargetDir),
            Vector2.one, JugeAndTweenTime);
    }

    #endregion

    #region Effect

    private void Reset_EffectComp()
    {
        for (int i = 0; i < BeforeEffectDepthList.Count; i++)
        {
            BeforeEffectDepthList[i].ThisSR.transform.localScale = Vector3.one;
            BeforeEffectDepthList[i].ThisSR.color = Color.white;
        }
    }

    private void Play_BeforeEffect(float _StartDelay)
    {
        DevTool.Set_KillTween(BeforeEffectSeq);
        BeforeEffectSeq = DOTween.Sequence();

        _StartDelay *= 0.8f;
        for (int i = 0; i < BeforeEffectDepthList.Count; i++)
        {
            BeforeEffectSeq.Join(BeforeEffectDepthList[i].ThisSR.transform.DOScale(1.5f, _StartDelay).SetEase(Ease.OutCubic));
            BeforeEffectSeq.Join(BeforeEffectDepthList[i].ThisSR.DOColor(BeforeEffectColor, _StartDelay).SetEase(Ease.OutCubic));
        }
    }

    private void Play_AfterEffect(float _EndDelay)
    {
        DevTool.Set_KillTween(BeforeEffectSeq);
        BeforeEffectSeq = DOTween.Sequence();

        _EndDelay *= 0.8f;
        for (int i = 0; i < BeforeEffectDepthList.Count; i++)
        {
            BeforeEffectSeq.Join(BeforeEffectDepthList[i].ThisSR.transform.DOScale(1f, _EndDelay).SetEase(Ease.OutCubic));
            BeforeEffectSeq.Join(BeforeEffectDepthList[i].ThisSR.DOColor(Color.white, _EndDelay).SetEase(Ease.OutCubic));
        }
    }

    #endregion
}
