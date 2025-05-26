using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class EnemyPattern_Range : EnemyPattern
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Melee")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private BulletState ThisBS;

    [Space(10)]
    [Header("=== Size")]
    [SerializeField] private Vector2 BulletShadowScale;
    [SerializeField] private Vector2 BulletColSize;

    [Space(10)]
    [Header("=== Trail")]
    [SerializeField] private float TrailTime;
    [SerializeField] private float TrailStartWidth;
    [SerializeField] private Gradient TrailGradient;

    [Space(10)]
    [Header("=== Light")]
    [SerializeField] private float LightIntensity;


    [Space(10)]
    [Header("=== Condition")]
    [SerializeField] private float MaxRange = 4f;
    [SerializeField] private float MinRange = 3f;

    [Space(10)]
    [Header("=== Effect")]
    [SerializeField] private int ShootExplAmount = 3;
    [SerializeField] private int ExplAmount = 3;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private List<DepthController> SpawnDepthList;
    [SerializeField] private List<DepthController> BeforeEffectDepthList;
    [SerializeField] private Color BeforeEffectColor;
    [SerializeField] private AnimationClip BulletAC;

    [HideInInspector] private Sequence BeforeEffectSeq;
    [HideInInspector] private float BulletRadiusCondition = 0;

    #endregion

    #region Framework

    private void Start()
    {
        BulletRadiusCondition = Get_BulletMaximumRadius();
    }

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
    }

    #endregion

    #region Can Check

    public override bool Can_PlayPattern()
    {
        float forPlayerDis = Vector2.Distance(ThisEnemy.transform.position, PlayerManager.Instance.PlayerController.transform.position);

        if (forPlayerDis >= MinRange && forPlayerDis < MaxRange && Can_ShootByBulletRadius())
        {
            return true;
        }

        return false;
    }

    private bool Can_ShootByBulletRadius()
    {
        for (int i = 0; i < SpawnDepthList.Count; i++)
        {
            if (DevTool.Is_Exist_UseCircle(SpawnDepthList[i].transform, PlayerManager.Instance.PlayerController.transform, "Wall", BulletRadiusCondition * 2))
            {
                return false;
            }
        }

        return true;
    }

    private float Get_BulletMaximumRadius()
    {
        return (BulletColSize.x > BulletColSize.y ? BulletColSize.x : BulletColSize.y) 
            * (BulletShadowScale.x > BulletShadowScale.y ? BulletShadowScale.x : BulletShadowScale.y);
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
        EnemyBulletController bullet = PoolingManager.Instance.Get_OP_EnemyBullet();
        bullet.Enemy = ThisEnemy;
        float targetShadow = _Depth.TargetRange;
        bullet.Set_State(
            ThisBS, 
            State_PosAndRot(_Depth.transform, _TargetDir), 
            State_Size(), 
            State_Anim(),
            State_Effect(),
            targetShadow);

        bullet.SetOn_LightIntensity(LightIntensity);
        bullet.SetOn_TrailState(TrailTime, TrailStartWidth, TrailGradient);

        // Effect
        UnitManager.Instance.Enemy_ExplImgGenerator.Expl_Enemy_Shoot(
            (Vector2)_Depth.TargetObject.transform.position + (_TargetDir * 0.3f),
            _TargetDir, ShootExplAmount);
    }

    #endregion

    #region State

    private BulletState_PosAndRot State_PosAndRot(Transform _TF, Vector2 _TargetDir)
    {
        return new BulletState_PosAndRot(_TF.position, _TargetDir, 0, BulletRadiusCondition);
    }

    private BulletState_Size State_Size()
    {
        return new BulletState_Size(BulletShadowScale, BulletColSize);
    }

    private State_Anim State_Anim()
    {
        return new State_Anim(BulletAC, 1);
    }

    private BulletState_Effect State_Effect()
    {
        return new BulletState_Effect(ExplAmount, 1f);
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

