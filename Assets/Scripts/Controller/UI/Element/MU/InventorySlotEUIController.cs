using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotEUIController : ElementUIController, IPointerEnterHandler, IPointerExitHandler
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Slot")]

    [Space(10)]
    [Header("=== Partner")]
    [SerializeField] public InventoryItemEUIController ThisItem = null;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private Vector2 ThisSizeDelta = new Vector2(100, 100);
    [SerializeField] private bool IsCanSelect = true;

    [Space(10)]
    [Header("=== Selected Sign")]
    [SerializeField] private RectTransform SignRT;

    // This
    [HideInInspector] public Image ThisImg;

    // Sign
    [HideInInspector] private Image SignImg;
    [HideInInspector] private static readonly float SignImgAnimDurTime = 0.1f;
    [HideInInspector] private static readonly float SignImgAnimSize = 1.2f;

    // Seq
    [HideInInspector] private Sequence SignSeq;

    #endregion

    #region Offset

    public override void Offset()
    {
        if (DevTool.Get_ComponentTType(gameObject, out RectTransform rt))
            rt.sizeDelta = ThisSizeDelta;

        ThisImg = DevTool.Get_ComponentTType(gameObject, out Image img) ? img : null;

        SignImg = DevTool.Get_ComponentTType(SignRT.gameObject, out Image signImg) ? signImg : null;

        SignImg.color = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_AlphaColor(SignImg, 0f);
    }

    #endregion

    #region OnPointer

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsCanSelect)
        { return; }

        Play_Selected(_TargetAlpha: 1f, _TargetScale: SignImgAnimSize, SignImgAnimDurTime);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsCanSelect)
        { return; }

        Play_Selected(_TargetAlpha: 0f, _TargetScale: 1f, SignImgAnimDurTime);
    }

    #endregion

    #region Select

    private void Play_Selected(float _TargetAlpha, float _TargetScale, float _DurTime)
    {
        DevTool.Set_KillTween(SignSeq);
        SignSeq = DOTween.Sequence();

        SignSeq.Append(SignImg.DOFade(_TargetAlpha, _DurTime).SetEase(Ease.Linear));
        SignSeq.Join(SignRT.DOScale(_TargetScale, _DurTime).SetEase(Ease.Linear));
    }

    #endregion
}
