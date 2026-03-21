using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Serialization;

public class EnemyPattern_Thrust : EnemyPattern
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Thrust")]

    [Space(10)]
    [Header("=== State")]
    [FormerlySerializedAs("ThisAS")][SerializeField] private AttackerState _as;

    [Space(10)]
    [Header("=== Value")]
    [FormerlySerializedAs("Guiding")][SerializeField] private float guiding = 0f;
    [FormerlySerializedAs("DirForTarget")][SerializeField] private Vector2 dirForTarget = Vector2.zero;
    [FormerlySerializedAs("ThisAC")][SerializeField] private AnimationClip ac;
    [FormerlySerializedAs("JugeAndTweenTime")][SerializeField] private float jugeAndTweenTime = 0.5f;
    [FormerlySerializedAs("AnimSpeed")][SerializeField] private float animSpeed = 2.6f;
    [FormerlySerializedAs("Speed")][SerializeField] private float speed = 1.5f;
    [FormerlySerializedAs("LightOn")][SerializeField] bool lightOn = false;
    [FormerlySerializedAs("LightSize")][SerializeField] float lightSize = 1f;

    [Space(10)]
    [Header("=== Condition")]
    [FormerlySerializedAs("MaxRange")][SerializeField] private float maxRange = 1.8f;
    [FormerlySerializedAs("MinRange")][SerializeField] private float minRange = 0f;

    [Space(10)]
    [Header("=== Component")]
    [FormerlySerializedAs("SpawnDepthList")][SerializeField] private List<DepthController> spawnDepthList;
    [FormerlySerializedAs("BeforeEffectDepthList")][SerializeField] private List<DepthController> beforeEffectDepthList;
    [FormerlySerializedAs("BeforeEffectColor")][SerializeField] private Color beforeEffectColor;

    [HideInInspector] private bool isThrusting = false;
    [HideInInspector] private Sequence beforeEffectSeq;

    #endregion

    #region Framework

    private void OnEnable()
    {
        Reset_EffectComp();
    }

    private void Update()
    {
        if (!isPlaying)
        {
            StopCoroutine(Play_ThisPattern_Cor());
        }

        if (isThrusting && guiding > 0)
        {
            Vector2 targetDir = Vector2.Lerp(
                dirForTarget, 
                DevTool.Get_DirForPlayer(enemy),
                guiding * Time.deltaTime);

            if (DevTool.TryGetDirNavMeshEnd(
            enemy.transform.position,
            targetDir,
            out Vector2 endPoint))
            {
                enemy.Set_NavDir(endPoint);
                dirForTarget = targetDir;
            }
        }
    }

    #endregion

    #region Can Check

    public override bool Can_PlayPattern()
    {
        if (isSpecialPattern) return true;

        float forPlayerDis = DevTool.Get_DisForPlayer(enemy);
        if (forPlayerDis >= minRange && forPlayerDis < maxRange)
        {
            return true;
        }

        return false;
    }

    #endregion

    #region Actual

    protected override IEnumerator Play_ThisPattern_Cor()
    {
        Play_BeforeEffect(startDelay);
        yield return new WaitForSeconds(startDelay);

        #region Actual

        currentRepeatAmount++;

        Vector2 targetDir = DevTool.Get_DirForPlayer(enemy);
        dirForTarget = targetDir;

        Play_ActualPattern(targetDir);

        #endregion

        Play_AfterEffect(endDelay);
        yield return new WaitForSeconds(endDelay);

        Stop_ActualPattern();
        if (currentRepeatAmount >= repeatAmount) // 반복을 마침
        {
            End_Pattern();
            enemy.Play_Pattern();
        }
        else
        {
            enemy.currentPatternCor = Play_ThisPattern_Cor();
            StartCoroutine(enemy.currentPatternCor);
        }
    }

    protected virtual void Play_ActualPattern(Vector2 targetDir)
    {
        isThrusting = true;

        // Attacker
        for (int i = 0; i < spawnDepthList.Count; i++)
            Play_ActualPattern_Each(spawnDepthList[i], targetDir);

        if (DevTool.TryGetDirNavMeshEnd(
            enemy.transform.position, 
            targetDir,
            out Vector2 endPoint))
        {
            enemy.Set_NavDir(endPoint);
            enemy.Set_MoveSpeed(speed);
        }

        SoundManager.instance.Play_2D_SFX_EnemyAttack_Random(enemy.Get_AS(), "Thrust", 2);
    }

    private void Play_ActualPattern_Each(DepthController depth, Vector2 targetDir)
    {
        EnemyAttackerController attacker = PoolingManager.instance.Get_OP_EnemyAttacker();
        attacker.enemy = enemy;
        float targetShadow = depth.targetRange;

        attacker.Set_State(
            _as,
            State_Juge(),
            State_Anim(),
            State_StartTF(targetDir),
            State_EndTF(targetDir),
            targetShadow,
            parent: depth.transform,
            isLocal: true);

        if (lightOn)
            attacker.Set_Light(
                lightSize, jugeAndTweenTime);
    }

    private void Stop_ActualPattern()
    {
        isThrusting = false;
        dirForTarget = Vector2.zero;
        enemy.Set_NavDir(Vector2.zero);
        enemy.Set_MoveSpeed(0);
    }

    #endregion

    #region State

    private AttackerState_Juge<CircleCollider2D> State_Juge()
    {
        return new AttackerState_Juge<CircleCollider2D>(Vector2.one);
    }

    private State_Anim State_Anim()
    {
        return new State_Anim(ac, animSpeed);
    }

    private State_TF2D State_StartTF(Vector2 targetDir)
    {
        return new State_TF2D(
            Vector2.zero,
            DevTool.Get_RotFromDir(targetDir),
            Vector2.one);
    }

    private AttackerState_EndTF State_EndTF(Vector2 targetDir)
    {
        return new AttackerState_EndTF(
            Vector2.zero,
            DevTool.Get_RotFromDir(targetDir),
            Vector2.one, jugeAndTweenTime);
    }

    #endregion

    #region Effect

    private void Reset_EffectComp()
    {
        for (int i = 0; i < beforeEffectDepthList.Count; i++)
        {
            beforeEffectDepthList[i].thisSr.transform.localScale = Vector3.one;
            beforeEffectDepthList[i].thisSr.color = Color.white;
        }
    }

    private void Play_BeforeEffect(float startDelay)
    {
        DevTool.Set_KillTween(beforeEffectSeq);
        beforeEffectSeq = DOTween.Sequence();

        startDelay *= 0.8f;
        for (int i = 0; i < beforeEffectDepthList.Count; i++)
        {
            beforeEffectSeq.Join(beforeEffectDepthList[i].thisSr.transform.DOScale(1.5f, startDelay).SetEase(Ease.OutCubic));
            beforeEffectSeq.Join(beforeEffectDepthList[i].thisSr.DOColor(beforeEffectColor, startDelay).SetEase(Ease.OutCubic));
        }
    }

    private void Play_AfterEffect(float endDelay)
    {
        DevTool.Set_KillTween(beforeEffectSeq);
        beforeEffectSeq = DOTween.Sequence();

        endDelay *= 0.8f;
        for (int i = 0; i < beforeEffectDepthList.Count; i++)
        {
            beforeEffectSeq.Join(beforeEffectDepthList[i].thisSr.transform.DOScale(1f, endDelay).SetEase(Ease.OutCubic));
            beforeEffectSeq.Join(beforeEffectDepthList[i].thisSr.DOColor(Color.white, endDelay).SetEase(Ease.OutCubic));
        }
    }

    #endregion
}
