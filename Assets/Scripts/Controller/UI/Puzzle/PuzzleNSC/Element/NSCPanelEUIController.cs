using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NSCPanelEUIController : ElementUIController
{
    #region Value 

    [Space(20)]
    [Header("<><><><><> NSC Panel")]

    [Space(10)]
    [Header("=== EUI")]
    [SerializeField] private NSCRollImgCellEUIController shapeRollEui;
    [SerializeField] private NSCRollColorCellEUIController colorRollEui;
    [SerializeField] private NSCRollImgCellEUIController numRollEui;

    [Space(10)]
    [Header("=== Answer")]
    [SerializeField] private Image answerImg;

    [Space(10)]
    [Header("=== TF")]
    [SerializeField] private Transform innerParentTf;

    // Owner
    [HideInInspector] public NumShapeColorPasswordUIController ownerUIController;

    // EUI
    [HideInInspector] public List<NSCRollCellEUIController> allRollEui;

    // Comp
    [HideInInspector] public RectTransform rt;
    [HideInInspector] private List<Image> allInnerList;

    #endregion

    #region Init

    public void Init(NumShapeColorPasswordUIController ui, Vector2 pos)
    {
        ownerUIController = ui;

        rt.anchoredPosition = pos;

        for (int i = 0; i < allRollEui.Count; i++)
            allRollEui[i].Init(ownerUIController, this);
    }

    #endregion

    #region Offset

    public override void Offset()
    {
        rt = DevTool.Get_ComponentTType(gameObject, out RectTransform _rt) ? _rt : null;

        allRollEui = new List<NSCRollCellEUIController>{ shapeRollEui, colorRollEui, numRollEui };

        allInnerList = DevTool.Get_ChildList<Image>(innerParentTf);

        for (int i = 0; i < allRollEui.Count; i++)
        {
            allRollEui[i].Offset();
        }
    }

    #endregion

    #region Set (Inner)

    public void Set_InnerColor(Color clr)
    {
        for (int i = 0; i < allInnerList.Count; i++)
            allInnerList[i].color = clr;
    }

    #endregion

    #region Set (Value)

    public void Set_RollValueRandom()
    {
        for (int i = 0; i < allRollEui.Count; i++)
        {
            allRollEui[i].Set_ImgByIndexRandom();
        }
    }

    public void Set_RandomAnswer()
    {
        shapeRollEui.answerIndex = Random.Range(0, shapeRollEui.Get_IndexAmount());
        colorRollEui.answerIndex = Random.Range(0, colorRollEui.Get_IndexAmount());
        numRollEui.answerIndex = Random.Range(0, numRollEui.Get_IndexAmount());

        var prefab = StaticResourceManager.instance.BuildPrefab.prisonPrefab;

        answerImg.sprite = prefab.allNscAnswerSprites[shapeRollEui.answerIndex].array[numRollEui.answerIndex];
        answerImg.color = prefab.nscClrArr[colorRollEui.answerIndex];

        for (int i = 0; i < allRollEui.Count; i++)
            allRollEui[i].Set_NoLock();
    }

    #endregion

    #region Can

    public bool Is_Answer()
    {
        if (shapeRollEui.Is_AnswerIndex() && colorRollEui.Is_AnswerIndex() && numRollEui.Is_AnswerIndex())
            return true;

        return false;
    }

    #endregion
}
