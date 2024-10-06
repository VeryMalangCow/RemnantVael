using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModifyEachInventorySlot : UIModule, IPointerEnterHandler, IPointerExitHandler
{
    #region Value

    [Header("=== Data")]
    [SerializeField] public ModifyEachInventoryItem ThisSlotItem = null;

    [Header("=== Selected Sign")]
    [SerializeField] public GameObject SelectedSign;
    [SerializeField] public float SignImgAnimDurTime = 0.1f;
    [SerializeField] public float SignImgAnimSize = 1.1f;

    [Header("=== RT")]
    [SerializeField] private Vector2 ThisSizeDelta = new Vector2(100, 100);

    [Header("=== Judg")]
    [SerializeField] public bool IsInventory = true;

    // Component
    private RectTransform ThisRT;
    private Image ThisImg;

    #endregion

    #region Offset

    public void SetData(Sprite _ThisIcon)
    {
        ThisImg.sprite = _ThisIcon;
    }

    public override void Offset()
    {
        if (TryGetComponent(out RectTransform rt))
        { ThisRT = rt; }

        if (TryGetComponent(out Image img))
        { ThisImg = img; }

        ThisRT.sizeDelta = ThisSizeDelta;

        if(SelectedSign.TryGetComponent(out Image ssimg))
        {
            Color clr = ssimg.color;
            clr.a = 0f;
            ssimg.color = clr;
        }
        SelectedSign.gameObject.SetActive(false);
    }

    #endregion

    #region Set Current

    public void OnPointerEnter(PointerEventData eventData)
    {
        InIt_SelectedItem();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OutIt_SelectedItem();
    }

    #endregion

    #region Select

    private void InIt_SelectedItem()
    {
        if (ThisSlotItem != null && ThisSlotItem.gameObject.activeSelf)
        {
            UIManager.Instance.ModuleUpgrade_UIController.CurrentSelectedMEIS = this;

            DOTween.Kill(gameObject.name); 

            Sequence seq = DOTween.Sequence();

            // img
            SelectedSign.TryGetComponent(out Image img);
            seq.Append(img.DOFade(1, SignImgAnimDurTime).SetEase(Ease.Linear));

            // rt
            SelectedSign.TryGetComponent(out RectTransform rt);
            Sequence _seq = DOTween.Sequence();
            _seq.Append(rt.DOScale(SignImgAnimSize, SignImgAnimDurTime / 2).SetEase(Ease.Linear));
            _seq.Append(rt.DOScale(1, SignImgAnimDurTime / 2).SetEase(Ease.Linear));
            seq.Join(_seq);

            seq.OnStart(() => { SelectedSign.gameObject.SetActive(true); })
                .SetId(gameObject.name);
        }
    }

    public void OutIt_SelectedItem()
    {
        if (UIManager.Instance.ModuleUpgrade_UIController.CurrentSelectedMEIS == this)
        {
            UIManager.Instance.ModuleUpgrade_UIController.CurrentSelectedMEIS = null;

            DOTween.Kill(gameObject.name); 

            Sequence seq = DOTween.Sequence();

            // img
            SelectedSign.TryGetComponent(out Image img);
            seq.Append(img.DOFade(0, SignImgAnimDurTime).SetEase(Ease.Linear));

            // rt
            SelectedSign.TryGetComponent(out RectTransform rt);
            seq.Join(rt.DOScale(1, SignImgAnimDurTime).SetEase(Ease.Linear));

            seq.OnComplete(() => { SelectedSign.gameObject.SetActive(false); })
                .SetId(gameObject.name);
        }
    }


    #endregion
}
