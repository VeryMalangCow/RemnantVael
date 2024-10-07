using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ModifyPanelBtn : UIModule
{
    #region Value

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private Button InteractOnBtn;
    [SerializeField] private Button InteractOffBtn;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float DurTime = 0.5f;

    [Header("-- Move")]
    [SerializeField] private Vector2 OriginalPos;
    [SerializeField] private Vector2 TargetPos;
    [SerializeField] private bool IsMove = false;

    [Header("-- Fade")]
    [SerializeField] private float OriginalFade;
    [SerializeField] private float TargetFade;
    [SerializeField] private bool IsFade = false;

    #endregion

    #region Offset

    public override void Offset()
    {
        InteractOnBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                MovePanelRT(OriginalPos, TargetPos, DurTime);
                FadePanelRT(OriginalFade, TargetFade, DurTime);
            });

        InteractOffBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                MovePanelRT(TargetPos, OriginalPos, DurTime);
                FadePanelRT(TargetFade, OriginalFade, DurTime);
            });
    }

    #endregion

    #region Move

    private void MovePanelRT(Vector2 _StartPos, Vector2 _TargetPos, float _DurTime)
    {
        if (!IsMove) 
        { return; }

        if(this.gameObject.TryGetComponent(out RectTransform ThisRT))
        {
            if (DOTween.IsTweening(ThisRT))
            { return; }

            ThisRT.anchoredPosition = _StartPos;
            ThisRT.DOAnchorPos(_TargetPos, _DurTime)
                .SetEase(Ease.OutCubic);
        }
    }

    private void FadePanelRT(float _StartAlpha, float _TargetAlpha, float _DurTime)
    {
        if (!IsFade)
        { return; }

        if (this.gameObject.TryGetComponent(out CanvasGroup ThisCG))
        {
            if (DOTween.IsTweening(ThisCG))
            { return; }

            ThisCG.alpha = _StartAlpha;
            ThisCG.DOFade(_TargetAlpha, _DurTime)
                .SetEase(Ease.OutCubic);
        }
    }

    #endregion
}
