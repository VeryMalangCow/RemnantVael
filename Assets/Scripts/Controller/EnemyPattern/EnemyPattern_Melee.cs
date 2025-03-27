using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class EnemyPattern_Melee : EnemyPattern
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Melee")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private AttackerState ThisAS;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private AnimationClip ThisAC;
    [SerializeField] private float JugeAndTweenTime = 0.5f;
    [SerializeField] private float AnimSpeed = 2.6f;
    [SerializeField] private float SpawnDis = 1f;
    [SerializeField] private float EndDis = 1.5f;
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

    [HideInInspector] private Sequence BeforeEffectSeq;

    #endregion

    #region Framework

    private void Update()
    {
        if (!IsPlaying)
        {
            StopCoroutine(Play_ThisPattern_Cor());
        }
    }

    #endregion

    #region Can Check

    public override bool Can_PlayPattern()
    {
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

        Vector2 targetDir = DevTool.Get_DirForPlayer(ThisEnemy);

        for (int i = 0; i < SpawnDepthList.Count; i++)
            Play_ActualPattern(SpawnDepthList[i], targetDir);
        
        #endregion

        Play_AfterEffect(EndDelay);
        yield return new WaitForSeconds(EndDelay);

        End_Pattern();
        ThisEnemy.Play_Pattern();
    }

    private void Play_ActualPattern(DepthController _Depth, Vector2 _TargetDir)
    {
        EnemyAttackerController attacker = PoolingManager.Instance.Get_OP_EnemyAttacker();
        attacker.Enemy = ThisEnemy;
        float targetShadow = _Depth.TargetRange;
        attacker.Set_State(
            ThisAS,
            State_Juge(),
            State_Anim(),
            State_StartTF(_Depth.transform.position, _TargetDir),
            State_EndTF(_Depth.transform.position, _TargetDir),
            targetShadow);

        if (LightOn)
            attacker.Set_Light(
                LightSize, JugeAndTweenTime);
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

    private State_TF2D State_StartTF(Vector2 _SpawnPos, Vector2 _TargetDir)
    {
        return new State_TF2D(
            _SpawnPos + (_TargetDir * SpawnDis),
            DevTool.Get_RotFromDir(_TargetDir),
            Vector2.one);
    }

    private AttackerState_EndTF State_EndTF(Vector2 _SpawnPos, Vector2 _TargetDir)
    {
        return new AttackerState_EndTF(
            _SpawnPos + (_TargetDir * EndDis),
            DevTool.Get_RotFromDir(_TargetDir),
            Vector2.one, JugeAndTweenTime);
    }


    #endregion

    #region Effect

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
