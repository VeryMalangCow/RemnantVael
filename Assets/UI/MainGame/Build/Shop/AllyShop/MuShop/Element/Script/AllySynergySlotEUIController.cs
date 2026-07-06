using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AllySynergySlotEUIController : OwnBtnEUIController
{
    #region Value

    #region - Inspector

    [Space(10)]
    [Header("=== Ally Synergy")]
    [SerializeField] private RectTransform frameRt;
    [SerializeField] private Image selectImg;
    [SerializeField] private Image lockImg;
    [SerializeField] private TMP_Text txt;
    [SerializeField] private TMP_Text playerSynergyStackTxt;

    #endregion

    #region - Hide

    [HideInInspector] private bool isOn = false;
    [HideInInspector] private Image img;
    [HideInInspector] private int id = -1;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        img = DevTool.Get_ComponentTType(gameObject, out Image _img) ? _img : null;
        Set_Select(false);
    }

    #endregion

    #region Get

    public bool Get_IsOn()
    {
        return isOn;
    }

    public int Get_ID()
    {
        return id;
    }

    #endregion

    #region Set

    public void Set_SelectChange()
    {
        Set_Select(!isOn);
    }

    public void Set_Select(bool onOff)
    {
        isOn = onOff;
        selectImg.gameObject.SetActive(onOff);
    }

    public void Set_SynergySlot(int id, Sprite icon, string desc)
    {
        this.gameObject.SetActive(true);

        this.id = id;
        img.sprite = icon;
        txt.text = desc;
    }

    public void Set_PlayerSynergyTxt(int amount)
    {
        if (amount == -1)
        {
            playerSynergyStackTxt.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            playerSynergyStackTxt.text = $"( {ResourceManager.instance.Get_StaticDesc(38)} )";
        }
        else
        {
            playerSynergyStackTxt.color = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
            playerSynergyStackTxt.text = $"( {ResourceManager.instance.Get_StaticDesc(39)}: <size=150%>{amount}</size> )";
        }
        
    }

    public void Set_Lock(bool isOn)
    {
        lockImg.gameObject.SetActive(isOn);
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!isCanSelect || btn == null || !btn.interactable) return;

        base.OnPointerEnter(eventData);

        Play_Scale(1.05f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!isCanSelect || btn == null || !btn.interactable) return;

        base.OnPointerExit(eventData);

        Play_Scale(1f);
    }

    #endregion

    #region Play

    public Sequence Play_Scale(float size, float durTime = 0.05f)
    {
        DevTool.SetKillTween(rt);

        Sequence seq = DOTween.Sequence();

        seq.Append(frameRt.DOScale(size, durTime));

        return seq;
    }

    #endregion
}
