using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Serialization;

public class EnemyPattern_Range : EnemyPattern
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Range")]

    [Space(10)]
    [Header("=== State")]
    [FormerlySerializedAs("ThisBS")][SerializeField] private BulletState bulletState;
    [FormerlySerializedAs("BaseAngle")][SerializeField] private float baseAngle = 0f;

    [Space(10)]
    [Header("=== Size")]
    [FormerlySerializedAs("BulletShadowScale")][SerializeField] private Vector2 bulletShadowScale;
    [FormerlySerializedAs("BulletColSize")][SerializeField] private Vector2 bulletColSize;

    [Space(10)]
    [Header("=== Trail")]
    [FormerlySerializedAs("TrailTime")][SerializeField] private float trailTime;
    [FormerlySerializedAs("TrailStartWidth")][SerializeField] private float trailStartWidth;
    [FormerlySerializedAs("TrailGradient")][SerializeField] private Gradient trailGradient;

    [Space(10)]
    [Header("=== Light")]
    [FormerlySerializedAs("LightIntensity")][SerializeField] private float lightIntensity;


    [Space(10)]
    [Header("=== Condition")]
    [FormerlySerializedAs("MaxRange")][SerializeField] private float maxRange = 4f;
    [FormerlySerializedAs("MinRange")][SerializeField] private float minRange = 3f;

    [Space(10)]
    [Header("=== Effect")]
    [FormerlySerializedAs("ShootExplAmount")][SerializeField] private int shootExplAmount = 3;
    [FormerlySerializedAs("ExplAmount")][SerializeField] private int explAmount = 3;

    [Space(10)]
    [Header("=== Component")]
    [FormerlySerializedAs("SpawnDepthList")][SerializeField] private List<DepthController> spawnDepthList;
    [FormerlySerializedAs("BeforeEffectDepthList")][SerializeField] private List<DepthController> beforeEffectDepthList;
    [FormerlySerializedAs("BeforeEffectColor")][SerializeField] private Color beforeEffectColor;
    [FormerlySerializedAs("BulletAC")][SerializeField] private AnimationClip bulletAc;

    [HideInInspector] private Sequence beforeEffectSeq;
    [HideInInspector] private float bulletRadiusCondition = 0;

    #endregion

    #region Framework

    private void Start()
    {
        bulletRadiusCondition = Get_BulletMaximumRadius();
    }

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
    }

    #endregion

    #region Can Check

    public override bool Can_PlayPattern()
    {
        if (isSpecialPattern) return true;

        float forPlayerDis = Vector2.Distance(enemy.transform.position, PlayerManager.instance.playerController.transform.position);

        if (forPlayerDis >= minRange && forPlayerDis < maxRange && Can_ShootByBulletRadius())
        {
            return true;
        }

        return false;
    }

    private bool Can_ShootByBulletRadius()
    {
        for (int i = 0; i < spawnDepthList.Count; i++)
        {
            if (DevTool.Is_Exist_UseCircle(spawnDepthList[i].transform, PlayerManager.instance.playerController.transform, "Wall", bulletRadiusCondition * 2))
            {
                return false;
            }
        }

        return true;
    }

    private float Get_BulletMaximumRadius()
    {
        return (bulletColSize.x > bulletColSize.y ? bulletColSize.x : bulletColSize.y) 
            * (bulletShadowScale.x > bulletShadowScale.y ? bulletShadowScale.x : bulletShadowScale.y);
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

        Play_ActualPattern(targetDir);
        SoundManager.instance.Play_2D_SFX_EnemyAttack_Random(enemy.Get_AS(), "Bullet", 2);

        #endregion

        Play_AfterEffect(endDelay);
        yield return new WaitForSeconds(endDelay);

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
        for (int i = 0; i < spawnDepthList.Count; i++)
            Play_ActualPattern_Each(spawnDepthList[i], 
                DevTool.Get_DirFromAngle(
                    DevTool.Get_AngleFromDir(targetDir) + baseAngle));
    }

    private void Play_ActualPattern_Each(DepthController depth, Vector2 targetDir)
    {
        EnemyBulletController bullet = PoolingManager.instance.Get_OP_EnemyBullet();
        bullet.ownEnemy = enemy;
        float targetShadow = depth.targetRange;
        bullet.Set_State(
            this.bulletState,
            State_PosAndRot(depth.transform, targetDir),
            State_Size(),
            State_Anim(),
            State_Effect(),
            targetShadow);

        bullet.SetOn_LightIntensity(lightIntensity);
        bullet.SetOn_TrailState(trailTime, trailStartWidth, trailGradient);

        // Effect
        UnitManager.instance.enemy_ExplImgGenerator.Expl_Enemy_Shoot(
            (Vector2)depth.targetObject.transform.position + (targetDir * 0.3f),
            targetDir, shootExplAmount);
    }

    #endregion

    #region State

    private BulletState_PosAndRot State_PosAndRot(Transform tf, Vector2 targetDir)
    {
        return new BulletState_PosAndRot(tf.position, targetDir, 0, bulletRadiusCondition);
    }

    private BulletState_Size State_Size()
    {
        return new BulletState_Size(bulletShadowScale, bulletColSize);
    }

    private State_Anim State_Anim()
    {
        return new State_Anim(bulletAc, 1);
    }

    private BulletState_Effect State_Effect()
    {
        return new BulletState_Effect(explAmount, 1f);
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

