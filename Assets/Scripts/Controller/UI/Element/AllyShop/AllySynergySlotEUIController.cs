using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllySynergySlotEUIController : OwnBtnEUIController
{
    #region Value

    #region - Inspector

    [Space(10)]
    [Header("=== Ally Synergy")]
    [SerializeField] private TMP_Text ThisTxt;

    #endregion

    #region - Hide

    [HideInInspector] private Image ThisImg;
    [HideInInspector] private int ID = -1;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        ThisImg = DevTool.Get_ComponentTType(gameObject, out Image img) ? img : null;
    }

    #endregion

    #region Set

    #region Set

    public void Set_SynergySlot(int _ID, Sprite _Icon, string _Desc)
    {
        this.gameObject.SetActive(true);

        ID = _ID;
        ThisImg.sprite = _Icon;
        ThisTxt.text = _Desc;
    }

    #endregion

    #endregion
}
