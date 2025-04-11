using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class PrisonController : InteractableBuildController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Prison ")]

    [Space(10)]
    [Header("=== Grade")]
    [SerializeField] public int Rating = 0;

    [Space(10)]
    [Header("=== Extra Upside")]
    [SerializeField] private SpriteRenderer ThisUpsideSR;
    [SerializeField] private Animator ThisUpsideAT;

    [Space(10)]
    [Header("=== Icon")]
    [SerializeField] private SortingGroup ExtraSG;
    [SerializeField] private SpriteRenderer DangerIcon;
    [SerializeField] private TMP_Text DangerTxt;
    [SerializeField] protected SpriteRenderer TypeIcon;
    [SerializeField] protected TMP_Text TypeTxt;

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

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Rating = Mathf.Clamp(Rating + 1, 0, MaxRating);
            Offset_DangerIconTxt();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Rating = Mathf.Clamp(Rating - 1, 0, MaxRating);
            Offset_DangerIconTxt();
        }
    }

    #region Offset

    private void Offset_DangerIconTxt()
    {
        DangerIcon.sprite = UnitManager.Instance.PrisonRateIconList[Rating];
        DangerTxt.text = $"{UnitManager.Instance.RatingString }: ({Rating + 1}) { UnitManager.Instance.PrisonRateStringList[Rating]} <size=150%>(</size>";
    }

    protected override void Offset()
    {
        Set_AnimValue();
        Set_Rating(0);

        base.Offset();

        Offset_DangerIconTxt();
    }

    #endregion

    #region Set

    public override void Set_SortingOrder(int _SortingOrder)
    {
        base.Set_SortingOrder(_SortingOrder);

        ThisUpsideSR.sortingOrder = _SortingOrder - 2;
        ExtraSG.sortingOrder = _SortingOrder + 1;
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
        OnOffAC = UnitManager.Instance.Prison_OnOffAC;
        OnOffAC_Upside = UnitManager.Instance.Prison_OnOffUpsideAC;
        OnOffStateAC = UnitManager.Instance.Prison_StateAC;

        OnOffMaterial = UnitManager.Instance.Prison_OnOffMaterial;
    }

    private void Set_Rating(int _Rate)
    {
        Rating = Mathf.Clamp(_Rate, 0, MaxRating);

        Set_AnimValue();
        Set_StateAnim();
    }

    public void Set_Unlock()
    {
        if (IsOn) return;

        // Oper
        if (PuzzleOper != null) PuzzleOper.Set_TargetBuildBroken();
        if (PayOper != null) PayOper.Set_TargetBuildBroken();

        // Set
        IsOn = true;
        Set_StateAnim();
    }

    #endregion
}
