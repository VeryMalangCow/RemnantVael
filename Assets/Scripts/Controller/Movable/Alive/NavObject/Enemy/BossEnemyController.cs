using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class BossEnemyController : EnemyController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Boss")]

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private int NameID;
    public int GetNameID => NameID;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public RectTransform PanelRT;
    [SerializeField] private Image ThisHUDIcon;
    [SerializeField] private List<GameObject> ThisAuraParticleGOList;

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private List<BossPhaseData> BossPhaseData;

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] public CoreDropItemPercent CoreDropItemPercent;

    [Space(10)]
    [Header("=== Reso")]
    [SerializeField] public Sprite BattleProdSprite;

    #endregion

    #region - Hide

    [HideInInspector] private static readonly Vector2 HUDBaseAnchorPos = new Vector2(-40, 290);
    [HideInInspector] private int CurrentPhase;

    #endregion

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        EnemyManager.instance.SetOn_BossEnemy(this);
        CurrentPhase = -1;
    }

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        HUD.ThisCanvas.worldCamera = MainGameUIManager.instance.uiCamera;

        BossPhaseData = BossPhaseData.OrderByDescending(obj => obj.thisPhaseLimitPercentHP).ToList();
        Try_PlayNewPatternByPhase();
    }

    #endregion

    #region Pattern

    public override void Play_Pattern()
    {
        if (IsDead) return;

        if (Try_PlayNewPatternByPhase()) return;

        base.Play_Pattern();
    }

    #endregion

    #region Phase

    private bool Try_PlayNewPatternByPhase()
    {
        BossPhaseData actualCurrentPhase = Get_CurrentPhase();
        if (actualCurrentPhase != null)
        {
            Debug.Log("Boss Phase :" + actualCurrentPhase.thisPhase);
            // Play new Pattern by Phase
            StartCoroutine(Set_NewPhase(actualCurrentPhase));
            
            // Icon
            ThisHUDIcon.sprite = ResourceManager.instance.Get_BossPhaseSprite(actualCurrentPhase.thisPhase);
            
            // Particle
            for (int i = 0; i < ThisAuraParticleGOList.Count; i++)
            {
                ThisAuraParticleGOList[i].gameObject.SetActive(actualCurrentPhase.thisPhase > i);
            }

            return true;
        }

        return false;
    }

    private BossPhaseData Get_CurrentPhase()
    {
        float percentHP = Get_PercentHP();
        for (int i = 0; i < BossPhaseData.Count; i++)
        {
            if (BossPhaseData[i].thisPhaseLimitPercentHP >= percentHP
                && BossPhaseData[i].thisPhase != CurrentPhase)
            {
                return BossPhaseData[i];
            }
        }
        return null;
    }

    private IEnumerator Set_NewPhase(BossPhaseData _Phase)
    {
        EndAll_Pattern();

        CurrentPhase = _Phase.thisPhase;

        OrderOfPriorityEnemyPatternList = _Phase.orderOfPriorityEnemyPatternList;
        SpecialPattern = _Phase.specialPattern;

        BossPhaseData.RemoveAt(0);

        yield return new WaitForSeconds(1f);

        Start_PatternFromNone();
    }

    #endregion

    #region HUD

    public void Set_HUDPanelPos()
    {
        PanelRT.anchoredPosition = HUDBaseAnchorPos;
    }

    #endregion

    #region Die

    protected override void Set_Die_GenItem()
    {
        base.Set_Die_GenItem();

        if (CoreDropItemPercent.coreItemPercent != 0 && DevTool.Is_ChanceSuccess(CoreDropItemPercent.coreItemPercent))
        {
            Gen_CoreItem(CoreDropItemPercent.coreItemID);
        }
    }

    protected override void Set_Die_Extra()
    {
        EnemyManager.instance.SetOff_BossEnemy();

        PoolingManager.instance.Set_EnqueueBossEnemy(this);
    }

    #endregion
}
