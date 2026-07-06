using DG.Tweening;
using TMPro;
using UnityEngine;

public class WorldTxtEUIController : ElementUIController, IPoolable
{
    #region Value

    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text txt;

    [HideInInspector] private static readonly float normalSize = 18;
    [HideInInspector] private static readonly float criticalSize = 26;
    [HideInInspector] private static readonly float dischargeSize = 22;

    #endregion

    public int PoolIndex { get; set; } = -1;
    public int ActiveIndex { get; set; } = -1;


    #region Pool
    public void PoolOffset()
    {
        gameObject.SetActive(false);
    }

    public void SetActiveOn()
    {
        gameObject.SetActive(true);
    }

    public void SetActiveOff()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Framework

    private void Start()
    {
        canvas.sortingOrder = SortingOrderManager.order_DmgTxt;
    }

    #endregion

    #region Offset

    public override void Offset()
    {
        Reset_Comp();
    }

    #endregion

    #region Reset

    private void Reset_Comp()
    {
        txt.color = new Color(1f, 1f, 1f, 0f);
        txt.transform.localScale = Vector3.zero;
    }

    private void Reset_Comp(Vector2 setPos, string txt, Color txtClr, Color tsClr, float fontSize)
    {
        Reset_Comp();

        this.txt.text = txt;
        this.txt.color = txtClr;
        this.transform.position = setPos;
        this.txt.fontSize = fontSize;
    }

    #endregion

    #region Tween

    private Sequence Play_DamageTxt(Vector2 startPos, string txt, Color txtClr, Color tsClr, float fontSize, Vector2 dir, float durTime)
    {
        Reset_Comp(startPos, txt, txtClr, tsClr, fontSize);

        Sequence totalSeq = DOTween.Sequence();

        totalSeq.Join(Play_MoveSeq(startPos, dir, durTime));
        totalSeq.Join(Play_ScaleSeq(durTime));
        totalSeq.Join(Play_FadeSeq(durTime));

        totalSeq
            .OnStart(() =>
            {
                this.gameObject.SetActive(true);
            })
            .OnComplete(() =>
            {
                this.gameObject.SetActive(false);
                VfxManager.instance.RemoveDmgTxtCanvas(this);
            });

        return totalSeq;
    }

    private Sequence Play_MoveSeq(Vector2 startPos, Vector2 dir, float durTime)
    {
        Sequence moveSeq = DOTween.Sequence();

        moveSeq.Append(this.transform.DOMove(startPos + dir, durTime));
        moveSeq
            .SetEase(Ease.OutCubic);

        return moveSeq;
    }

    private Sequence Play_ScaleSeq(float durTime)
    {
        Sequence scaleSeq = DOTween.Sequence();

        scaleSeq.Append(txt.transform.DOScale(1f, durTime * 0.4f).SetEase(Ease.Linear));
        scaleSeq.AppendInterval(durTime * 0.4f);
        scaleSeq.Append(txt.transform.DOScale(0f, durTime * 0.2f).SetEase(Ease.Linear));

        return scaleSeq;
    }

    private Sequence Play_FadeSeq(float durTime)
    {
        Sequence fadeSeq = DOTween.Sequence();

        fadeSeq.Append(txt.DOFade(1f, durTime * 0.4f).SetEase(Ease.Linear));
        fadeSeq.AppendInterval(durTime * 0.4f);
        fadeSeq.Append(txt.DOFade(0f, durTime * 0.2f).SetEase(Ease.Linear));

        return fadeSeq;
    }

    #endregion

    #region Set

    private void Set_Bold(bool isSet)
    {
        if (isSet)
        {
            txt.fontStyle = FontStyles.Bold;
        }
        else
        {
            txt.fontStyle = FontStyles.Normal;
        }
    }

    #endregion

    #region Get

    private Color Get_Color_BySpecialState(string stateName)
    {
        if (stateName == "DISCHARGE")
        { return Color.white; }
        return Color.white;
    }


    private float Get_FontSize_ByCritical(bool isCritical)
    {
        return isCritical ? criticalSize : normalSize;
    }

    #endregion

    #region Usable

    public void Offset_ByShieldDmg(Vector2 targetPos, float dmg, bool isCritical)
    {
        Set_Bold(isCritical);
        Play_DamageTxt(targetPos, string.Format("{0:F1}", dmg),
            Color.white, Color.black, 
            Get_FontSize_ByCritical(isCritical),
            new Vector2(0.2f, 0.2f), 1f);
    }

    public void Offset_ByPhysicDmg(Vector2 targetPos, float dmg, bool isCritical)
    {
        Set_Bold(isCritical);
        Play_DamageTxt(targetPos, string.Format("{0:F1}", dmg),
            PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Physics, isCritical), Color.black, 
            Get_FontSize_ByCritical(isCritical),
            new Vector2(-0.2f, 0.2f), 1f);
        
    }

    public void Offset_ByEnergyDmg(Vector2 targetPos, float dmg, bool isCritical)
    {
        Set_Bold(isCritical);
        Play_DamageTxt(targetPos, string.Format("{0:F1}", dmg),
            PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, isCritical), Color.black, 
            Get_FontSize_ByCritical(isCritical),
            new Vector2(-0.2f, 0.2f), 1f);
        
    }

    public void Offset_ByStateDischarge(Vector2 targetPos)
    {
        Set_Bold(false);
        Play_DamageTxt(targetPos, "DISCHARGE",
            Get_Color_BySpecialState("DISCHARGE"), Color.black, 
            dischargeSize,
            new Vector2(0f, 0.2f), 1f);
    }

    #endregion
}
