using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DurablityEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Durablity")]

    [Space(10)]
    [Header("=== Intact")]
    [SerializeField] private GameObject IntactGO;
    [SerializeField] private TMP_Text DurablityTxt;
    [SerializeField] private TMP_Text DurablityStateTxt;
    [SerializeField] private Transform FillImgListParentTF;

    [Space(10)]
    [Header("=== Broken")]
    [SerializeField] private GameObject BrokenGO;
    [SerializeField] private TMP_Text BrokenTxt;
    [SerializeField] private Color BrokenTxtColor;

    #endregion

    #region - Hide

    [HideInInspector] private List<Image> FillImgList;

    #endregion

    #endregion

    #region Set (Language)

    public void Set_LanguageTxt()
    {
        DurablityTxt.text = ResourceManager.instance.Get_StaticWord(23) + " :";
        BrokenTxt.text = ResourceManager.instance.Get_StaticWord(24) + ": " + ResourceManager.instance.Get_StaticDesc(16);
    }

    #endregion

    #region Offset

    public override void Offset()
    {
        Set_LanguageTxt();

        FillImgList = new List<Image>();
        for (int i = 0; i < FillImgListParentTF.childCount; i++)
        {
            DevTool.Get_ComponentTType(FillImgListParentTF.GetChild(i).gameObject.transform.GetChild(0).gameObject, out Image img);
            FillImgList.Add(img);
        }

        DevTool.Set_Color(BrokenTxtColor, BrokenTxt);
    }

    #endregion

    #region Set

    public void Set_Dur(int _DurState)
    {
        if (_DurState > 0)
        {
            Set_Intact(true);
            DevTool.Set_Dur(_DurState, FillImgList, DurablityStateTxt);
        }
        else
        {
            Set_Intact(false);
        }
    }

    private void Set_Intact(bool _IsOn)
    {
        IntactGO.SetActive(_IsOn);
        BrokenGO.SetActive(!_IsOn);
    }

    #endregion
}
