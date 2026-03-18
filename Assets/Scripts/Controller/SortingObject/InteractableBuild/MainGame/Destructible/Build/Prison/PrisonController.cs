using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class PrisonController : InteractableBuildController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Prison ")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private Collider2D ThisCol;

    [Space(10)]
    [Header("=== Grade")]
    [SerializeField] public int Rating = 0;

    [Space(10)]
    [Header("=== Extra Upside")]
    [SerializeField] private SpriteRenderer ThisUpsideSR;
    [SerializeField] private Animator ThisUpsideAT;

    [Space(10)]
    [Header("=== Extra Icon")]
    [SerializeField] private SortingGroup ExtraSG;
    [SerializeField] private SpriteRenderer DangerIcon;
    [SerializeField] protected SpriteRenderer TypeIcon;

    [Space(10)]
    [Header("=== Ally")]
    [SerializeField] private SortingGroup AllySG;

    [Space(10)]
    [Header("=== Operator")]
    [SerializeField] public PrisonPuzzleOperatorController PuzzleOper;
    [SerializeField] public PrisonPayOperatorController PayOper;

    // Grade
    [HideInInspector] private int MaxRating = 4;

    // AC
    [HideInInspector] private CoupleData<AnimationClip> OnOffAC_Upside;
    [HideInInspector] private AnimatorOverrideController UpsideAOC;
    [HideInInspector] private CoupleData<Material> OnOffMaterial;

    // Extra State
    [HideInInspector] private TMP_Text DangerTxt;
    [HideInInspector] protected TMP_Text TypeTxt;

    // Ally
    [HideInInspector] protected int AllyAmount = 0; 
    [HideInInspector] protected List<Transform> PrisonAllAllyTFList = new List<Transform>();
    [HideInInspector] protected List<Transform> PrisonActivingAllyTFList = new List<Transform>();
    [HideInInspector] protected List<SpriteRenderer> PrisonAllySRList = new List<SpriteRenderer>();
    [HideInInspector] protected List<SpriteRenderer> PrisonAllyShadowList = new List<SpriteRenderer>();
    [HideInInspector] protected PrisonAllySprite AllySprites;

    [HideInInspector] private static List<int> PercentPrisonGrade = new List<int>
    { 5, 4, 3, 2, 1 };

    #endregion

    #region Offset

    private void Offset_Value()
    {
        Rating = DevTool.Get_Grade(PercentPrisonGrade);
        AllyAmount = Random.Range(Rating * 4, (Rating * 4) + 4) + 1;
    }

    private void Offset_Comp()
    {
        DangerIcon.sprite = ResourceManager.instance.Get_PrisonRankSprite(Rating);

        DangerTxt = DevTool.Get_ComponentTType(DangerIcon.gameObject.transform.GetChild(0).gameObject, out TMP_Text dangerTxt) ? dangerTxt : null;
        TypeTxt = DevTool.Get_ComponentTType(TypeIcon.gameObject.transform.GetChild(0).gameObject, out TMP_Text typeTxt) ? typeTxt : null;

        PrisonAllAllyTFList = DevTool.Get_ChildList<Transform>(AllySG.gameObject.transform);

        PrisonActivingAllyTFList = new List<Transform>();
        while (true)
        {
            Transform randomTF = PrisonAllAllyTFList[Random.Range(0, PrisonAllAllyTFList.Count)];
            if (!PrisonActivingAllyTFList.Contains(randomTF))
                PrisonActivingAllyTFList.Add(randomTF);

            if (PrisonActivingAllyTFList.Count >= AllyAmount)
                break;
        }

        PrisonAllySRList = new List<SpriteRenderer>();
        PrisonAllyShadowList = new List<SpriteRenderer>();
        for (int i = 0; i < PrisonAllAllyTFList.Count; i++)
        {
            if (PrisonActivingAllyTFList.Contains(PrisonAllAllyTFList[i]))
            {
                PrisonAllySRList.Add(DevTool.Get_ComponentTType<SpriteRenderer>(PrisonAllAllyTFList[i].transform.GetChild(0).gameObject));
                PrisonAllyShadowList.Add(DevTool.Get_ComponentTType<SpriteRenderer>(PrisonAllAllyTFList[i].transform.GetChild(1).gameObject));
                PrisonAllAllyTFList[i].gameObject.SetActive(true);
            }
            else
            {
                PrisonAllAllyTFList[i].gameObject.SetActive(false);
            }
        }
    }


    protected override void Offset()
    {
        Offset_Value();
        Set_AnimValue();
        Set_Rating(Rating);

        base.Offset();

        Offset_Comp();
        Set_LanguageTxt();

        ResourceManager.instance.allPrisons.Add(this);
    }

    #endregion

    #region Set

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);

        ThisUpsideSR.sortingOrder = _SortingOrder - 2;
        ExtraSG.sortingOrder = _SortingOrder + 1;
        AllySG.sortingOrder = _SortingOrder;
    }


    protected override void Set_StateAnim()
    {
        base.Set_StateAnim();

        DevTool.Set_Anim(ref UpsideAOC, ThisUpsideAT, OnOffAC_Upside.Get_Special(IsOn));

        ThisAnimator.speed = 1.5f;
        ThisUpsideAT.speed = 1.5f;

        ThisSR.material = OnOffMaterial.Get_Special(IsOn);
        ThisUpsideSR.material = OnOffMaterial.Get_Special(IsOn);
    }

    private void Set_AnimValue()
    {
        OnOffAC = ResourceManager.instance.prison_OnOffAC;
        OnOffAC_Upside = ResourceManager.instance.prison_OnOffUpsideAC;
        OnOffStateAC = ResourceManager.instance.prison_StateAC;

        OnOffMaterial = ResourceManager.instance.Get_CoupleBuildMaterial("PrisonOff", "PrisonOn");
    }

    private void Set_Rating(int _Rate)
    {
        Rating = Mathf.Clamp(_Rate, 0, MaxRating);

        Set_AnimValue();
        Set_StateAnim();
    }

    #endregion

    #region Unlock

    public virtual void Set_Unlock()
    {
        // Sound
        SoundManager.instance.Play_2D_SFX_Build("PrisonUnlock");

        Set_UnlockData();
        StartCoroutine(Play_Unlock_Cor());
    }

    private void Set_UnlockData()
    {
        if (IsOn) return;

        // Oper
        if (PuzzleOper != null) PuzzleOper.Set_TargetBuildBroken();
        if (PayOper != null) PayOper.Set_TargetBuildBroken();

        // Set
        IsOn = true;
        Set_StateAnim();

    }

    private IEnumerator Play_Unlock_Cor()
    {
        float fallingDelay = 0.1f;
        float fallingTime = 0.3f;
        float standTime = 0.7f;
        float saluteTime = 1.2f;
        float fadeTime = 1.5f;

        for (int i = 0; i < PrisonActivingAllyTFList.Count; i++)
        {
            yield return new WaitForSeconds(fallingDelay);

            int index = i;
            Play_EachMoveDown(fallingTime, index);
        }

        yield return new WaitForSeconds(fallingTime + standTime);

        for (int i = 0; i < PrisonAllySRList.Count; i++)
        {
            PrisonAllySRList[i].sprite = AllySprites.Salute;
        }

        yield return new WaitForSeconds(saluteTime);

        for (int i = 0; i < PrisonAllySRList.Count; i++)
        {
            PrisonAllySRList[i].DOFade(0, fadeTime);
            PrisonAllyShadowList[i].DOFade(0, fadeTime);
        }

        yield return new WaitForSeconds(fadeTime);

        for (int i = 0; i < PrisonActivingAllyTFList.Count; i++)
        {
            PrisonActivingAllyTFList[i].gameObject.SetActive(false);
        }

        ThisCol.enabled = false;

        PrisonAllAllyTFList.Clear();
        PrisonActivingAllyTFList.Clear();
        PrisonAllySRList.Clear();
        PrisonAllyShadowList.Clear();

        PrisonAllAllyTFList = null;
        PrisonActivingAllyTFList = null;
        PrisonAllySRList = null;
        PrisonAllyShadowList = null;
        AllySprites = null;
    }


    private void Play_EachMoveDown(float _FallingTime, int _Index)
    {
        Sequence seq = DOTween.Sequence();

        Transform tf = PrisonActivingAllyTFList[_Index];
        SpriteRenderer targetSr = DevTool.Get_ComponentTType(tf.GetChild(0).gameObject, out SpriteRenderer sr) ? sr : null;

        seq.Append(tf.GetChild(0).DOLocalMoveY(0, _FallingTime)
            .OnStart(() => { targetSr.sprite = AllySprites.Fall; })
            .OnComplete(() => { targetSr.sprite = AllySprites.Stand; }));
    }

    #endregion

    #region Set (Language)

    public virtual void Set_LanguageTxt()
    {
        DangerTxt.text = $"{ResourceManager.instance.ratingString}: ({Rating + 1}) {ResourceManager.instance.prisonRateStringArr[Rating]} <size=150%>(</size>";

        if (PuzzleOper != null) 
            PuzzleOper.Set_Language();
    }

    #endregion
}
