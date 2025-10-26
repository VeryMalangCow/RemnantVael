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

        EnemyManager.Instance.SetOn_BossEnemy(this);
        CurrentPhase = -1;
    }

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        HUD.ThisCanvas.worldCamera = MainGameUIManager.Instance.UICamera;

        BossPhaseData = BossPhaseData.OrderByDescending(obj => obj.ThisPhaseLimitPercentHP).ToList();
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
            Debug.Log("Boss Phase :" + actualCurrentPhase.ThisPhase);
            // Play new Pattern by Phase
            StartCoroutine(Set_NewPhase(actualCurrentPhase));
            
            // Icon
            ThisHUDIcon.sprite = ResourceManager.Instance.Get_BossPhaseSprite(actualCurrentPhase.ThisPhase);
            
            // Particle
            for (int i = 0; i < ThisAuraParticleGOList.Count; i++)
            {
                ThisAuraParticleGOList[i].gameObject.SetActive(actualCurrentPhase.ThisPhase > i);
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
            if (BossPhaseData[i].ThisPhaseLimitPercentHP >= percentHP
                && BossPhaseData[i].ThisPhase != CurrentPhase)
            {
                return BossPhaseData[i];
            }
        }
        return null;
    }

    private IEnumerator Set_NewPhase(BossPhaseData _Phase)
    {
        EndAll_Pattern();

        CurrentPhase = _Phase.ThisPhase;

        OrderOfPriorityEnemyPatternList = _Phase.OrderOfPriorityEnemyPatternList;
        SpecialPattern = _Phase.SpecialPattern;

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

        if (CoreDropItemPercent.CoreItemPercent != 0 && DevTool.Is_ChanceSuccess(CoreDropItemPercent.CoreItemPercent))
        {
            Gen_CoreItem(CoreDropItemPercent.CoreItemID);
        }
    }

    protected override void Set_Die_Extra()
    {
        EnemyManager.Instance.SetOff_BossEnemy();

        PoolingManager.Instance.Set_EnqueueBossEnemy(this);
    }

    #endregion
}
