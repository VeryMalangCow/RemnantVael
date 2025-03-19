using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DurablityEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(10)]
    [Header("=== Durablity")]
    [SerializeField] private TMP_Text DurablityTxt;
    [SerializeField] private TMP_Text DurablityStateTxt;
    [SerializeField] private Transform FillImgListParentTF;

    #endregion

    #region - Hide

    [HideInInspector] public static string DurablityStringTxt = "Durablity";
    [HideInInspector] private List<Image> FillImgList;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        DurablityTxt.text = DurablityStringTxt + " :";

        FillImgList = new List<Image>();
        for (int i = 0; i < FillImgListParentTF.childCount; i++)
        {
            FillImgListParentTF.GetChild(i).gameObject.transform.GetChild(0).gameObject.TryGetComponent(out Image EmptyImg);
            FillImgList.Add(EmptyImg);
        }
    }

    #endregion

    #region Set

    public void Set_Dur(int _DurState)
    {
        DevTool.Set_Dur(_DurState, FillImgList, DurablityStateTxt);
    }

    #endregion
}
