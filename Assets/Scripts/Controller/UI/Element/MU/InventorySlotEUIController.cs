using DG.Tweening;
using TMPro;
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
    [SerializeField] public InventoryItemEUIController item = null;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private Vector2 sizeDelta = new Vector2(100, 100);
    [SerializeField] private bool isCanSelect = true;

    [Space(10)]
    [Header("=== Selected Sign")]
    [SerializeField] private RectTransform signRt;
    [SerializeField] private TMP_Text equipedTxt;
    [SerializeField] private TMP_Text forgeSelectedTxt;

    // Only Inventory
    [HideInInspector] public int col = -1;
    [HideInInspector] public int row = -1;

    // This
    [HideInInspector] public Image thisImg;

    // Sign
    [HideInInspector] private Image signImg;
    [HideInInspector] private static readonly float signImgAnimDurTime = 0.1f;
    [HideInInspector] private static readonly float signImgAnimSize = 1.6f;

    // Owner
    [HideInInspector] public SinglePanelUIController ownerUIController = null;

    // Seq
    [HideInInspector] private Sequence signSeq;

    #endregion

    #region Offset

    public override void Offset()
    {
        if (DevTool.Get_ComponentTType(gameObject, out RectTransform _rt))
            _rt.sizeDelta = sizeDelta;

        thisImg = DevTool.Get_ComponentTType(gameObject, out Image img) ? img : null;

        signImg = DevTool.Get_ComponentTType(signRt.gameObject, out Image _signImg) ? _signImg : null;

        Color txtColor = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);

        this.signImg.color =
            PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
        equipedTxt.color = txtColor;
        forgeSelectedTxt.color = txtColor;

        DevTool.Set_AlphaColor(this.signImg, 0f);
    }

    #endregion

    #region OnPointer

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isCanSelect)
        { return; }

        Play_Selected(targetAlpha: 1f, targetScale: signImgAnimSize, signImgAnimDurTime);

        if (ownerUIController.gameObject.activeSelf)
            ownerUIController.currentSlotBtn = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isCanSelect)
        { return; }

        Play_Selected(targetAlpha: 0f, targetScale: 1f, signImgAnimDurTime);

        if (ownerUIController.gameObject.activeSelf)
            ownerUIController.currentSlotBtn = null;
    }

    #endregion

    #region Select

    public void Play_Selected(float targetAlpha, float targetScale, float durTime)
    {
        DevTool.Set_KillTween(signSeq);
        signSeq = DOTween.Sequence();

        signSeq.Append(signImg.DOFade(targetAlpha, durTime).SetEase(Ease.Linear));
        signSeq.Join(signRt.DOScale(targetScale, durTime).SetEase(Ease.Linear));
    }

    public void Set_SelectedOff()
    {
        DevTool.Set_KillTween(signSeq);

        DevTool.Get_AlphaColor(signImg, 0f);
        signRt.localScale = Vector2.one;
    }


    #endregion

    #region Equiped

    public void Set_EquipedTxt_NoneNum(bool isOn)
    {
        equipedTxt.gameObject.SetActive(isOn);

        if (isOn)
        {
            equipedTxt.text = "#";
            equipedTxt.transform.SetAsLastSibling();
        }
    }

    public void Set_EquipedTxt(bool isOn, int equipedSlotIndex = 0)
    {
        equipedTxt.gameObject.SetActive(isOn);

        if (isOn)
        {
            equipedTxt.text = $"#{equipedSlotIndex + 1}";
            equipedTxt.transform.SetAsLastSibling();
        }
    }

    public void Set_EquipedTxt(bool isOn, string equipedTxt)
    {
        this.equipedTxt.gameObject.SetActive(isOn);

        if (isOn)
        {
            this.equipedTxt.text = equipedTxt;
            this.equipedTxt.transform.SetAsLastSibling();
        }
    }

    #endregion

    #region Forge

    public void Set_ForgeSelectedTxt(bool isOn, int index = -1)
    {
        forgeSelectedTxt.gameObject.SetActive(isOn);

        string txt = index == -1 ? "<>" : $"<{index + 1}>";
        if (isOn)
        { 
            forgeSelectedTxt.text = txt;
            forgeSelectedTxt.transform.SetAsLastSibling();
        }
    }

    #endregion
}