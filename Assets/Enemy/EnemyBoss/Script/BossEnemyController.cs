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
    [SerializeField] private int nameId;
    public int GetNameID => nameId;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public RectTransform panelRt;
    [SerializeField] private Image hudIcon;
    [SerializeField] private List<GameObject> auraParticleGoList;

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private List<BossPhaseData> bossPhaseData;

    [Space(10)]
    [Header("=== Item")]
    [SerializeField] public CoreDropItemPercent coreDropItemPercent;

    [Space(10)]
    [Header("=== Reso")]
    [SerializeField] public Sprite battleProdSprite;

    #endregion

    #region - Hide

    [HideInInspector] private static readonly Vector2 hudBaseAnchorPos = new Vector2(-40, 290);
    [HideInInspector] private int currentPhase;

    #endregion

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        EnemyManager.instance.SetOn_BossEnemy(this);
        currentPhase = -1;
    }

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        hud.canvas.worldCamera = MainGameUIManager.instance.uiCamera;

        //bossPhaseData = bossPhaseData.OrderByDescending(obj => obj.thisPhaseLimitPercentHP).ToList();
        //Try_PlayNewPatternByPhase();
    }

    #endregion

    #region Pattern

    public override void Play_Pattern()
    {
        if (isDead) return;

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
            hudIcon.sprite = StaticResourceManager.instance.EnemyReso.enemyPhaseSprites[actualCurrentPhase.thisPhase];
            
            // Particle
            for (int i = 0; i < auraParticleGoList.Count; i++)
            {
                auraParticleGoList[i].gameObject.SetActive(actualCurrentPhase.thisPhase > i);
            }

            return true;
        }

        return false;
    }

    private BossPhaseData Get_CurrentPhase()
    {
        float percentHP = Get_PercentHP();
        for (int i = 0; i < bossPhaseData.Count; i++)
        {
            if (bossPhaseData[i].thisPhaseLimitPercentHP >= percentHP
                && bossPhaseData[i].thisPhase != currentPhase)
            {
                return bossPhaseData[i];
            }
        }
        return null;
    }

    private IEnumerator Set_NewPhase(BossPhaseData phase)
    {
        EndAll_Pattern();

        currentPhase = phase.thisPhase;

        orderOfPriorityEnemyPatternList = phase.orderOfPriorityEnemyPatternList;
        specialPattern = phase.specialPattern;

        bossPhaseData.RemoveAt(0);

        yield return new WaitForSeconds(1f);

        Start_PatternFromNone();
    }

    #endregion

    #region HUD

    public void Set_HUDPanelPos()
    {
        panelRt.anchoredPosition = hudBaseAnchorPos;
    }

    #endregion

    #region Die

    protected override void Set_Die_GenItem()
    {
        base.Set_Die_GenItem();

        if (coreDropItemPercent.coreItemPercent != 0 && DevTool.Is_ChanceSuccess(coreDropItemPercent.coreItemPercent))
        {
            Gen_CoreItem(coreDropItemPercent.coreItemID);
        }
    }

    protected override void Set_Die()
    {
        base.Set_Die();
        EnemyManager.instance.SetOff_BossEnemy();
        AllyRequestManager.instance.Play_KillEliteEnemy();
    }

    #endregion
}
