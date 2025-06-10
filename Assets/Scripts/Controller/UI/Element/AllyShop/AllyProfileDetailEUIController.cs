using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyProfileDetailEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Profile Detail")]

    [Space(10)]
    [Header("=== On Panel")]
    [SerializeField] private GameObject OnPanelGO;
    [SerializeField] private Image FaceImg;
    [SerializeField] private TMP_Text NameTxt;

    [Space(10)]
    [Header("=== Off Panel")]
    [SerializeField] private GameObject OffPanelGO;

    #endregion

    #region Offset

    public override void Offset()
    {
        SetOff_Panel();
    }

    #endregion

    #region Set (Panel)

    public void SetOn_Panel(AllyController _Ally)
    {
        Set_Panel(true);
        NameTxt.text = $"-[ {_Ally.Get_Name()} ]-";
        FaceImg.sprite = _Ally.Get_FrontFaceImg();
    }

    public void SetOff_Panel()
    {
        Set_Panel(false);
        NameTxt.text = "";

    }

    private void Set_Panel(bool _OnOff)
    {
        OnPanelGO.SetActive(_OnOff);
        OffPanelGO.SetActive(!_OnOff);
    }

    #endregion
}
