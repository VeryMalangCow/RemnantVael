using DG.Tweening;
using LeTai.TrueShadow;
using TMPro;
using UnityEngine;

public class WorldTxtEUIController : ElementUIController
{
    #region Value

    [SerializeField] private Canvas ThisCanvas;
    [SerializeField] private TMP_Text ThisTxt;
    [SerializeField] private TrueShadow ThisTS;

    [HideInInspector] private static readonly float NormalSize = 18;
    [HideInInspector] private static readonly float CriticalSize = 26;
    [HideInInspector] private static readonly float DischargeSize = 22;

    #endregion

    #region Framework

    private void Start()
    {
        ThisCanvas.sortingOrder = LayerOrderManager.Order_DmgTxt;
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
        ThisTxt.color = new Color(1f, 1f, 1f, 0f);
        ThisTxt.transform.localScale = Vector3.zero;
    }

    private void Reset_Comp(Vector2 _SetPos, string _Txt, Color _TxtColor, Color _TSColor, float _FontSize)
    {
        Reset_Comp();

        ThisTxt.text = _Txt;
        ThisTxt.color = _TxtColor;
        ThisTS.Color = _TSColor; 
        this.transform.position = _SetPos;
        ThisTxt.fontSize = _FontSize;
    }

    #endregion

    #region Tween

    private Sequence Play_DamageTxt(Vector2 _StartPos, string _Txt, Color _TxtColor, Color _TSColor, float _FontSize,
        Vector2 _Dir, float _DurTime)
    {
        Reset_Comp(_StartPos, _Txt, _TxtColor, _TSColor, _FontSize);

        Sequence totalSeq = DOTween.Sequence();

        totalSeq.Join(Play_MoveSeq(_StartPos, _Dir, _DurTime));
        totalSeq.Join(Play_ScaleSeq(_DurTime));
        totalSeq.Join(Play_FadeSeq(_DurTime));

        totalSeq
            .OnStart(() =>
            {
                this.gameObject.SetActive(true);
            })
            .OnComplete(() =>
            {
                this.gameObject.SetActive(false);
                PoolingManager.Instance.DmgTxtCanvases.Enqueue(this);
            });

        return totalSeq;
    }

    private Sequence Play_MoveSeq(Vector2 _StartPos, Vector2 _Dir, float _DurTime)
    {
        Sequence moveSeq = DOTween.Sequence();

        moveSeq.Append(this.transform.DOMove(_StartPos + _Dir, _DurTime));
        moveSeq
            .SetEase(Ease.OutCubic);

        return moveSeq;
    }

    private Sequence Play_ScaleSeq(float _DurTime)
    {
        Sequence scaleSeq = DOTween.Sequence();

        scaleSeq.Append(ThisTxt.transform.DOScale(1f, _DurTime * 0.4f).SetEase(Ease.Linear));
        scaleSeq.AppendInterval(_DurTime * 0.4f);
        scaleSeq.Append(ThisTxt.transform.DOScale(0f, _DurTime * 0.2f).SetEase(Ease.Linear));

        return scaleSeq;
    }

    private Sequence Play_FadeSeq(float _DurTime)
    {
        Sequence fadeSeq = DOTween.Sequence();

        fadeSeq.Append(ThisTxt.DOFade(1f, _DurTime * 0.4f).SetEase(Ease.Linear));
        fadeSeq.AppendInterval(_DurTime * 0.4f);
        fadeSeq.Append(ThisTxt.DOFade(0f, _DurTime * 0.2f).SetEase(Ease.Linear));

        return fadeSeq;
    }

    #endregion

    #region Set

    private void Set_Bold(bool _IsSet)
    {
        if (_IsSet)
        {
            ThisTxt.fontStyle = FontStyles.Bold;
        }
        else
        {
            ThisTxt.fontStyle = FontStyles.Normal;
        }
    }

    #endregion

    #region Get

    private Color Get_Color_BySpecialState(string _StateName)
    {
        if (_StateName == "DISCHARGE")
        { return Color.white; }
        return Color.white;
    }


    private float Get_FontSize_ByCritical(bool _IsCritical)
    {
        return _IsCritical ? CriticalSize : NormalSize;
    }

    #endregion

    #region Usable

    public void Offset_ByShieldDmg(Vector2 _TargetPos, float _Dmg, bool _IsCritical)
    {
        Set_Bold(_IsCritical);
        Play_DamageTxt(_TargetPos, string.Format("{0:F1}", _Dmg),
            Color.white, Color.black, 
            Get_FontSize_ByCritical(_IsCritical),
            new Vector2(0.2f, 0.2f), 1f);
    }

    public void Offset_ByPhysicDmg(Vector2 _TargetPos, float _Dmg, bool _IsCritical)
    {
        Set_Bold(_IsCritical);
        Play_DamageTxt(_TargetPos, string.Format("{0:F1}", _Dmg),
            PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Physics, _IsCritical), Color.black, 
            Get_FontSize_ByCritical(_IsCritical),
            new Vector2(-0.2f, 0.2f), 1f);
        
    }

    public void Offset_ByEnergyDmg(Vector2 _TargetPos, float _Dmg, bool _IsCritical)
    {
        Set_Bold(_IsCritical);
        Play_DamageTxt(_TargetPos, string.Format("{0:F1}", _Dmg),
            PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, _IsCritical), Color.black, 
            Get_FontSize_ByCritical(_IsCritical),
            new Vector2(-0.2f, 0.2f), 1f);
        
    }

    public void Offset_ByStateDischarge(Vector2 _TargetPos)
    {
        Set_Bold(false);
        Play_DamageTxt(_TargetPos, "DISCHARGE",
            Get_Color_BySpecialState("DISCHARGE"), Color.black, 
            DischargeSize,
            new Vector2(0f, 0.2f), 1f);
    }

    #endregion
}
