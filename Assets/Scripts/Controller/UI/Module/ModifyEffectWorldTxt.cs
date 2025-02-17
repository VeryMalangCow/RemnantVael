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

    private void SetComponent(Vector2 _TargetPos, string _Txt, Color _TxtColor, Color _TSColor, float _FontSize)
    {
        ThisTxt.text = _Txt;
        ThisTxt.color = _TxtColor;
        ThisTS.Color = _TSColor; 
        this.transform.position = _TargetPos;
        ThisTxt.fontSize = _FontSize;
    }

    public void StartDamageTxt(Vector2 _TargetPos, string _Txt, Color _TxtColor, Color _TSColor, float _FontSize,
        Vector2 _Dir, float _DurTime)
    {
        Offset();
        SetComponent(_TargetPos, _Txt, _TxtColor, _TSColor, _FontSize);

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

    private int GetDefaultSize()
    {
        return 25;
    }

    private int GetSize(bool _IsCritical)
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

    private Color GetColor(eDamageType _DamageType, bool _IsCritical)
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

    private Color GetColor(string _StateName)
    {
        if (_StateName == "STUNED")
        { return Color.white; }
        return Color.white;
    }

    private void SetBold(bool _IsSet)
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

    public void OffsetByShieldDmg(Vector2 _TargetPos, float _Dmg, bool _IsCritical)
    {
        SetBold(_IsCritical);
        StartDamageTxt(_TargetPos, string.Format("{0:F1}", _Dmg),
            Color.white, Color.black, GetSize(_IsCritical),
            new Vector2(0.2f, 0.2f), 1f);
    }

    public void OffsetByPhysicDmg(Vector2 _TargetPos, float _Dmg, bool _IsCritical)
    {
        SetBold(_IsCritical);
        StartDamageTxt(_TargetPos, string.Format("{0:F1}", _Dmg),
            GetColor(eDamageType.Physics, _IsCritical), Color.black, GetSize(_IsCritical),
            new Vector2(-0.2f, 0.2f), 1f);
        
    }

    public void OffsetByEnergyDmg(Vector2 _TargetPos, float _Dmg, bool _IsCritical)
    {
        SetBold(_IsCritical);
        StartDamageTxt(_TargetPos, string.Format("{0:F1}", _Dmg),
            GetColor(eDamageType.Energy, _IsCritical), Color.black, GetSize(_IsCritical),
            new Vector2(-0.2f, 0.2f), 1f);
        
    }

    public void OffsetByStateStun(Vector2 _TargetPos)
    {
        SetBold(false);
        StartDamageTxt(_TargetPos, "STUNED",
            GetColor("STUNED"), Color.black, GetDefaultSize(),
            new Vector2(0f, 0.2f), 1f);
    }

    #endregion

}
