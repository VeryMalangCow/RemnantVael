using DG.Tweening;
using LeTai.TrueShadow;
using TMPro;
using UnityEngine;

public class ModifyEffectWorldTxt : UIModule
{
    #region Value

    [HideInInspector] private Canvas ThisCanvas;
    [SerializeField] private TMP_Text ThisTxt;
    [HideInInspector] private TrueShadow ThisTS;

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisTxt.color = new Color(1f, 1f, 1f, 0f);
        ThisTxt.transform.localScale = Vector3.zero;

        if (ThisCanvas == null && this.gameObject.TryGetComponent(out Canvas canvas))
        {
            ThisCanvas =  canvas;
            ThisCanvas.sortingOrder = 4000;
        }

        if (ThisTS == null && ThisTxt.gameObject.TryGetComponent(out TrueShadow ts))
        { 
            ThisTS = ts; 
        }
    }

    #endregion

    #region Get or Set Basic

    private void Set_Component(Vector2 _TargetPos, string _Txt, Color _TxtColor, Color _TSColor, float _FontSize)
    {
        ThisTxt.text = _Txt;
        ThisTxt.color = _TxtColor;
        ThisTS.Color = _TSColor; 
        this.transform.position = _TargetPos;
        ThisTxt.fontSize = _FontSize;
    }

    public void Start_DamageTxt(Vector2 _TargetPos, string _Txt, Color _TxtColor, Color _TSColor, float _FontSize,
        Vector2 _Dir, float _DurTime)
    {
        Offset();
        Set_Component(_TargetPos, _Txt, _TxtColor, _TSColor, _FontSize);

        this.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        Sequence seq2 = DOTween.Sequence();

        seq.Append(this.transform.DOMove(_TargetPos + _Dir, _DurTime));

        seq2.Append(ThisTxt.transform.DOScale(1f, _DurTime / 5).SetEase(Ease.Linear));
        seq2.Join(ThisTxt.DOFade(1f, _DurTime / 5).SetEase(Ease.Linear));

        seq2.AppendInterval(_DurTime * 3 / 5);

        seq2.Append(ThisTxt.transform.DOScale(0f, _DurTime / 5).SetEase(Ease.Linear));
        seq2.Join(ThisTxt.DOFade(0f, _DurTime / 5).SetEase(Ease.Linear));

        Sequence totalSeq = DOTween.Sequence();
        totalSeq.Join(seq);
        totalSeq.Join(seq2);

        totalSeq.OnComplete(() =>
        {
            this.gameObject.SetActive(false);
            PoolingManager.Instance.DmgTxtCanvases.Queue.Enqueue(this);
        });
    }

    private int Get_DefaultSize()
    {
        return 25;
    }

    private int Get_Size(bool _IsCritical)
    {
        if (_IsCritical)
        {
            return 30;
        }
        else
        {
            return 20;
        }
    }

    private Color Get_Color(eDamageType _DamageType, bool _IsCritical)
    {
        switch (_DamageType)
        {
            case eDamageType.Physics:
                if (!_IsCritical)
                { return new Color(1f, 0.4f, 0.4f, 1f); }
                else
                { return new Color(1f, 0f, 0f, 1f); }

            case eDamageType.Energy:
                if (!_IsCritical)
                { return new Color(0.4f, 0.4f, 1f, 1f); }
                else
                { return new Color(0f, 0f, 1f, 1f); }

            default:
                return Color.white;
        }
    }

    private Color Get_Color(string _StateName)
    {
        if (_StateName == "DISCHARGE")
        { return Color.white; }
        return Color.white;
    }

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

    #region Usable

    public void Offset_ByShieldDmg(Vector2 _TargetPos, float _Dmg, bool _IsCritical)
    {
        Set_Bold(_IsCritical);
        Start_DamageTxt(_TargetPos, string.Format("{0:F1}", _Dmg),
            Color.white, Color.black, Get_Size(_IsCritical),
            new Vector2(0.2f, 0.2f), 1f);
    }

    public void Offset_ByPhysicDmg(Vector2 _TargetPos, float _Dmg, bool _IsCritical)
    {
        Set_Bold(_IsCritical);
        Start_DamageTxt(_TargetPos, string.Format("{0:F1}", _Dmg),
            Get_Color(eDamageType.Physics, _IsCritical), Color.black, Get_Size(_IsCritical),
            new Vector2(-0.2f, 0.2f), 1f);
        
    }

    public void Offset_ByEnergyDmg(Vector2 _TargetPos, float _Dmg, bool _IsCritical)
    {
        Set_Bold(_IsCritical);
        Start_DamageTxt(_TargetPos, string.Format("{0:F1}", _Dmg),
            Get_Color(eDamageType.Energy, _IsCritical), Color.black, Get_Size(_IsCritical),
            new Vector2(-0.2f, 0.2f), 1f);
        
    }

    public void Offset_ByStateDischarge(Vector2 _TargetPos)
    {
        Set_Bold(false);
        Start_DamageTxt(_TargetPos, "DISCHARGE",
            Get_Color("DISCHARGE"), Color.black, Get_DefaultSize(),
            new Vector2(0f, 0.2f), 1f);
    }

    #endregion

}
