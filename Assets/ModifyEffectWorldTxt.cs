using DG.Tweening;
using LeTai.TrueShadow;
using TMPro;
using UnityEngine;

public class ModifyEffectWorldTxt : UIModule
{
    #region Value

    [SerializeField] private TMP_Text ThisTxt;
    [HideInInspector] private TrueShadow ThisTS;

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisTxt.color = new Color(1f, 1f, 1f, 0f);
        ThisTxt.transform.localScale = Vector3.zero;
        if (ThisTS == null && ThisTxt.gameObject.TryGetComponent(out TrueShadow ts))
        { ThisTS = ts; }
    }

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


    #endregion

}
