using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DurablityEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Durablity")]

    [Space(10)]
    [Header("=== Intact")]
    [FormerlySerializedAs("IntactGO")][SerializeField] private GameObject intactGo;
    [FormerlySerializedAs("DurablityTxt")][SerializeField] private TMP_Text durablityTxt;
    [FormerlySerializedAs("DurablityStateTxt")][SerializeField] private TMP_Text durablityStateTxt;
    [FormerlySerializedAs("FillImgListParentTF")][SerializeField] private Transform fillImgListParentTf;

    [Space(10)]
    [Header("=== Broken")]
    [FormerlySerializedAs("BrokenGO")][SerializeField] private GameObject brokenGo;
    [FormerlySerializedAs("BrokenTxt")][SerializeField] private TMP_Text brokenTxt;
    [FormerlySerializedAs("BrokenTxtColor")][SerializeField] private Color brokenTxtClr;

    #endregion

    #region - Hide

    [HideInInspector] private List<Image> fillImgList;

    #endregion

    #endregion

    #region Set (Language)

    public void Set_LanguageTxt()
    {
        durablityTxt.text = ResourceManager.instance.Get_StaticWord(23) + " :";
        brokenTxt.text = ResourceManager.instance.Get_StaticWord(24) + ": " + ResourceManager.instance.Get_StaticDesc(16);
    }

    #endregion

    #region Offset

    public override void Offset()
    {
        Set_LanguageTxt();

        fillImgList = new List<Image>();
        for (int i = 0; i < fillImgListParentTf.childCount; i++)
        {
            DevTool.Get_ComponentTType(fillImgListParentTf.GetChild(i).gameObject.transform.GetChild(0).gameObject, out Image img);
            fillImgList.Add(img);
        }

        DevTool.Set_Color(brokenTxtClr, brokenTxt);
    }

    #endregion

    #region Set

    public void Set_Dur(int durState)
    {
        if (durState > 0)
        {
            Set_Intact(true);
            DevTool.Set_Dur(durState, fillImgList, durablityStateTxt);
        }
        else
        {
            Set_Intact(false);
        }
    }

    private void Set_Intact(bool isOn)
    {
        intactGo.SetActive(isOn);
        brokenGo.SetActive(!isOn);
    }

    #endregion
}
