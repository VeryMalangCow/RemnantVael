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
    [SerializeField] private GameObject onPanelGo;
    [SerializeField] private Image faceImg;
    [SerializeField] private TMP_Text nameTxt;

    [Space(10)]
    [Header("=== Off Panel")]
    [SerializeField] private GameObject offPanelGo;

    #endregion

    #region Offset

    public override void Offset()
    {
        SetOff_Panel();
    }

    #endregion

    #region Set (Panel)

    public void SetOn_Panel(AllyController ally)
    {
        Set_Panel(true);
        nameTxt.text = $"-[ {ally.Get_Name()} ]-";
        faceImg.sprite = ally.Get_FrontFaceImg();
    }

    public void SetOff_Panel()
    {
        Set_Panel(false);
        nameTxt.text = "";

    }

    private void Set_Panel(bool onOff)
    {
        onPanelGo.SetActive(onOff);
        offPanelGo.SetActive(!onOff);
    }

    #endregion
}
