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
    [SerializeField] private RectTransform ThisFrameRT;
    [SerializeField] private Image ThisSelectImg;
    [SerializeField] private TMP_Text ThisTxt;
    [SerializeField] private TMP_Text PlayerSynergyStackTxt;

    #endregion

    #region - Hide

    [HideInInspector] private bool isOn = false;
    [HideInInspector] private Image ThisImg;
    [HideInInspector] private int ID = -1;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        ThisImg = DevTool.Get_ComponentTType(gameObject, out Image img) ? img : null;
        Set_Select(false);
    }

    #endregion

    #region Set

    public void Set_SelectChange()
    {
        Set_Select(!isOn);
    }

    public void Set_Select(bool _OnOff)
    {
        isOn = _OnOff;
        ThisSelectImg.gameObject.SetActive(_OnOff);
    }

    public void Set_SynergySlot(int _ID, Sprite _Icon, string _Desc)
    {
        this.gameObject.SetActive(true);

        ID = _ID;
        ThisImg.sprite = _Icon;
        ThisTxt.text = _Desc;
    }

    public void Set_PlayerSynergyTxt(int _Amount)
    {
        if (_Amount == -1)
        {
            PlayerSynergyStackTxt.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            PlayerSynergyStackTxt.text = $"( {ResourceManager.Instance.Get_StaticDesc(38)} )";
        }
        else
        {
            PlayerSynergyStackTxt.color = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, true);
            PlayerSynergyStackTxt.text = $"( {ResourceManager.Instance.Get_StaticDesc(39)}: <size=150%>{_Amount}</size> )";
        }
        
    }

    #endregion

    #region Pointer

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        base.OnPointerEnter(eventData);

        Play_Scale(1.05f);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!IsCanSelect || ThisBtn == null || !ThisBtn.interactable) return;

        base.OnPointerExit(eventData);

        Play_Scale(1f);
    }

    #endregion

    #region Play

    public Sequence Play_Scale(float _Size, float _DurTime = 0.05f)
    {
        DevTool.Set_KillTween(ThisRT);

        Sequence seq = DOTween.Sequence();

        seq.Append(ThisFrameRT.DOScale(_Size, _DurTime));

        return seq;
    }

    #endregion
}
