using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NSCPanelEUIController : ElementUIController
{
    #region Value 

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> NSC Panel")]

    [Space(10)]
    [Header("=== EUI")]
    [SerializeField] private NSCRollImgCellEUIController ShapeRollEUI;
    [SerializeField] private NSCRollColorCellEUIController ColorRollEUI;
    [SerializeField] private NSCRollImgCellEUIController NumRollEUI;

    [Space(10)]
    [Header("=== Answer")]
    [SerializeField] private Image AnswerImg;

    [Space(10)]
    [Header("=== TF")]
    [SerializeField] private Transform InnerParentTF;

    #endregion

    #region - Hide

    // Owner
    [HideInInspector] public NumShapeColorPasswordUIController OwnerUIController;

    // EUI
    [HideInInspector] public List<NSCRollCellEUIController> AllRollEUI;

    // Comp
    [HideInInspector] public RectTransform ThisRT;
    [HideInInspector] private List<Image> AllInnerList;

    #endregion

    #region - Hide

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisRT = DevTool.Get_ComponentTType(gameObject, out RectTransform rt) ? rt : null;

        AllRollEUI = new List<NSCRollCellEUIController>{ ShapeRollEUI, ColorRollEUI, NumRollEUI };

        AllInnerList = DevTool.Get_ChildList<Image>(InnerParentTF);

        for (int i = 0; i < AllRollEUI.Count; i++)
        {
            AllRollEUI[i].OwnerUIController = OwnerUIController;
            AllRollEUI[i].OwnerNSCUIController = OwnerUIController;
            AllRollEUI[i].OwnerNSCPanelEUIController = this;

            AllRollEUI[i].Offset();
        }
    }

    #endregion

    #region Set (Inner)

    public void Set_InnerColor(Color _Clr)
    {
        for (int i = 0; i < AllInnerList.Count; i++)
            AllInnerList[i].color = _Clr;
    }

    #endregion

    #region Set (Value)

    public void Set_RollValueRandom()
    {
        for (int i = 0; i < AllRollEUI.Count; i++)
        {
            AllRollEUI[i].Set_ImgByIndexRandom();
        }
    }

    public void Set_RandomAnswer()
    {
        ShapeRollEUI.AnswerIndex = Random.Range(0, ShapeRollEUI.Get_IndexAmount());
        ColorRollEUI.AnswerIndex = Random.Range(0, ColorRollEUI.Get_IndexAmount());
        NumRollEUI.AnswerIndex = Random.Range(0, NumRollEUI.Get_IndexAmount());

        AnswerImg.sprite = UnitManager.Instance.Get_NSCAnswerSprite(ShapeRollEUI.AnswerIndex, NumRollEUI.AnswerIndex);
        AnswerImg.color = UnitManager.Instance.NSC_ColorList[ColorRollEUI.AnswerIndex];

        for (int i = 0; i < AllRollEUI.Count; i++)
            AllRollEUI[i].Set_NoLock();
    }

    #endregion

    #region Can

    public bool Is_Answer()
    {
        if (ShapeRollEUI.Is_AnswerIndex() && ColorRollEUI.Is_AnswerIndex() && NumRollEUI.Is_AnswerIndex())
            return true;

        return false;
    }

    #endregion
}
