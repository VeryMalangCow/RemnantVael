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
    [SerializeField] private Collider2D col;

    [Space(10)]
    [Header("=== Grade")]
    [SerializeField] public int rating = 0;

    [Space(10)]
    [Header("=== Extra Upside")]
    [SerializeField] private SpriteRenderer upsideSr;
    [SerializeField] private Animator upsideAt;

    [Space(10)]
    [Header("=== Extra Icon")]
    [SerializeField] private SortingGroup extraSg;
    [SerializeField] private SpriteRenderer dangerIcon;
    [SerializeField] protected SpriteRenderer typeIcon;

    [Space(10)]
    [Header("=== Ally")]
    [SerializeField] private SortingGroup allySg;

    [Space(10)]
    [Header("=== Operator")]
    [SerializeField] public PrisonPuzzleOperatorController puzzleOper;
    [SerializeField] public PrisonPayOperatorController payOper;

    // Grade
    [HideInInspector] private int maxRating = 4;

    // AC
    [HideInInspector] private CoupleData<AnimationClip> onOffAc_Upside;
    [HideInInspector] private AnimatorOverrideController upsideAoc;
    [HideInInspector] private CoupleData<Material> onOffMaterial;

    // Extra State
    [HideInInspector] private TMP_Text dangerTxt;
    [HideInInspector] protected TMP_Text typeTxt;

    // Ally
    [HideInInspector] protected int allyAmount = 0; 
    [HideInInspector] protected List<Transform> prisonAllAllyTfList = new List<Transform>();
    [HideInInspector] protected List<Transform> prisonActivingAllyTfList = new List<Transform>();
    [HideInInspector] protected List<SpriteRenderer> prisonAllySrList = new List<SpriteRenderer>();
    [HideInInspector] protected List<SpriteRenderer> prisonAllyShadowList = new List<SpriteRenderer>();
    [HideInInspector] protected PrisonAllySprite allySprites;

    [HideInInspector] private static List<int> percentPrisonGrade = new List<int>
    { 5, 4, 3, 2, 1 };

    #endregion

    #region Offset

    private void Offset_Value()
    {
        rating = DevTool.Get_Grade(percentPrisonGrade);
        allyAmount = Random.Range(rating * 4, (rating * 4) + 4) + 1;
    }

    private void Offset_Comp()
    {
        dangerIcon.sprite = ResourceManager.instance.Get_PrisonRankSprite(rating);

        dangerTxt = DevTool.Get_ComponentTType(dangerIcon.gameObject.transform.GetChild(0).gameObject, out TMP_Text _dangerTxt) ? _dangerTxt : null;
        typeTxt = DevTool.Get_ComponentTType(typeIcon.gameObject.transform.GetChild(0).gameObject, out TMP_Text _typeTxt) ? _typeTxt : null;

        prisonAllAllyTfList = DevTool.Get_ChildList<Transform>(allySg.gameObject.transform);

        prisonActivingAllyTfList = new List<Transform>();
        while (true)
        {
            Transform randomTF = prisonAllAllyTfList[Random.Range(0, prisonAllAllyTfList.Count)];
            if (!prisonActivingAllyTfList.Contains(randomTF))
                prisonActivingAllyTfList.Add(randomTF);

            if (prisonActivingAllyTfList.Count >= allyAmount)
                break;
        }

        prisonAllySrList = new List<SpriteRenderer>();
        prisonAllyShadowList = new List<SpriteRenderer>();
        for (int i = 0; i < prisonAllAllyTfList.Count; i++)
        {
            if (prisonActivingAllyTfList.Contains(prisonAllAllyTfList[i]))
            {
                prisonAllySrList.Add(DevTool.Get_ComponentTType<SpriteRenderer>(prisonAllAllyTfList[i].transform.GetChild(0).gameObject));
                prisonAllyShadowList.Add(DevTool.Get_ComponentTType<SpriteRenderer>(prisonAllAllyTfList[i].transform.GetChild(1).gameObject));
                prisonAllAllyTfList[i].gameObject.SetActive(true);
            }
            else
            {
                prisonAllAllyTfList[i].gameObject.SetActive(false);
            }
        }
    }


    protected override void Offset()
    {
        Offset_Value();
        Set_AnimValue();
        Set_Rating(rating);

        base.Offset();

        Offset_Comp();
        Set_LanguageTxt();

        ResourceManager.instance.allPrisons.Add(this);
    }

    #endregion

    #region Set

    public override void Set_SortingOrder(int sortingOrder)
    {
        base.Set_SortingOrder(sortingOrder);

        upsideSr.sortingOrder = sortingOrder - 2;
        extraSg.sortingOrder = sortingOrder + 1;
        allySg.sortingOrder = sortingOrder;
    }


    protected override void Set_StateAnim()
    {
        base.Set_StateAnim();

        DevTool.Set_Anim(ref upsideAoc, upsideAt, onOffAc_Upside.Get_Special(isOn));

        at.speed = 1.5f;
        upsideAt.speed = 1.5f;

        thisSr.material = onOffMaterial.Get_Special(isOn);
        upsideSr.material = onOffMaterial.Get_Special(isOn);
    }

    private void Set_AnimValue()
    {
        onOffAc = ResourceManager.instance.prison_OnOffAC;
        onOffAc_Upside = ResourceManager.instance.prison_OnOffUpsideAC;
        onOffStateAc = ResourceManager.instance.prison_StateAC;

        onOffMaterial = ResourceManager.instance.Get_CoupleBuildMaterial("PrisonOff", "PrisonOn");
    }

    private void Set_Rating(int rate)
    {
        rating = Mathf.Clamp(rate, 0, maxRating);

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
        if (isOn) return;

        // Oper
        if (puzzleOper != null) puzzleOper.Set_TargetBuildBroken();
        if (payOper != null) payOper.Set_TargetBuildBroken();

        // Set
        isOn = true;
        Set_StateAnim();

    }

    private IEnumerator Play_Unlock_Cor()
    {
        float fallingDelay = 0.1f;
        float fallingTime = 0.3f;
        float standTime = 0.7f;
        float saluteTime = 1.2f;
        float fadeTime = 1.5f;

        for (int i = 0; i < prisonActivingAllyTfList.Count; i++)
        {
            yield return new WaitForSeconds(fallingDelay);

            int index = i;
            Play_EachMoveDown(fallingTime, index);
        }

        yield return new WaitForSeconds(fallingTime + standTime);

        for (int i = 0; i < prisonAllySrList.Count; i++)
        {
            prisonAllySrList[i].sprite = allySprites.salute;
        }

        yield return new WaitForSeconds(saluteTime);

        for (int i = 0; i < prisonAllySrList.Count; i++)
        {
            prisonAllySrList[i].DOFade(0, fadeTime);
            prisonAllyShadowList[i].DOFade(0, fadeTime);
        }

        yield return new WaitForSeconds(fadeTime);

        for (int i = 0; i < prisonActivingAllyTfList.Count; i++)
        {
            prisonActivingAllyTfList[i].gameObject.SetActive(false);
        }

        col.enabled = false;

        prisonAllAllyTfList.Clear();
        prisonActivingAllyTfList.Clear();
        prisonAllySrList.Clear();
        prisonAllyShadowList.Clear();

        prisonAllAllyTfList = null;
        prisonActivingAllyTfList = null;
        prisonAllySrList = null;
        prisonAllyShadowList = null;
        allySprites = null;
    }


    private void Play_EachMoveDown(float fallingTime, int index)
    {
        Sequence seq = DOTween.Sequence();

        Transform tf = prisonActivingAllyTfList[index];
        SpriteRenderer targetSr = DevTool.Get_ComponentTType(tf.GetChild(0).gameObject, out SpriteRenderer sr) ? sr : null;

        seq.Append(tf.GetChild(0).DOLocalMoveY(0, fallingTime)
            .OnStart(() => { targetSr.sprite = allySprites.fall; })
            .OnComplete(() => { targetSr.sprite = allySprites.stand; }));
    }

    #endregion

    #region Set (Language)

    public virtual void Set_LanguageTxt()
    {
        dangerTxt.text = $"{ResourceManager.instance.ratingString}: ({rating + 1}) {ResourceManager.instance.prisonRateStringArr[rating]} <size=150%>(</size>";

        if (puzzleOper != null) 
            puzzleOper.Set_Language();
    }

    #endregion
}
