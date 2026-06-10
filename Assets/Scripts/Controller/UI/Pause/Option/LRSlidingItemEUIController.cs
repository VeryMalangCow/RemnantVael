using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LRSlidingItemEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(10)]
    [Header("=== Header")]
    [SerializeField] public TMP_Text headerTxt;

    [Space(10)]
    [Header("=== Main")]
    [SerializeField] private Transform mainItemParentTf;

    [Space(10)]
    [Header("=== LR")]
    [SerializeField] public OwnBtnEUIController leftBtn;
    [SerializeField] public OwnBtnEUIController rightBtn;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private TMP_Text[] mainClrTmps;
    public TMP_Text[] MainClrTmps { get { return mainClrTmps; } }

    #endregion

    #region - Hide

    // Main
    [HideInInspector] private List<Transform> mainItemTfList;
    [HideInInspector] private int currentIndex = 0;

    #endregion

    #endregion

    #region Offset

    public void Set_OwnerUIController(SinglePanelUIController ownerUIController)
    {
        leftBtn.ownerUIController = ownerUIController;
        rightBtn.ownerUIController = ownerUIController;
    }

    public override void Offset()
    {
        leftBtn.Offset();
        rightBtn.Offset();

        mainItemTfList = DevTool.Get_ChildList<Transform>(mainItemParentTf);
        Set_Item(currentIndex);
    }

    #endregion

    #region Change

    public void Change_Left(Dele setFunc = null)
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = mainItemTfList.Count - 1;

        Set_Item(currentIndex);

        if (setFunc != null)
            setFunc();
    }

    public void Change_Right(Dele setFunc = null)
    {
        currentIndex++;
        if (currentIndex >= mainItemTfList.Count)
            currentIndex = 0;

        Set_Item(currentIndex);

        if (setFunc != null)
            setFunc();
    }

    #endregion

    #region Set

    public void Set_Item(int index)
    {
        currentIndex = index;

        for (int i = 0; i < mainItemTfList.Count; i++)
            mainItemTfList[i].gameObject.SetActive(false);
        
        mainItemTfList[index].gameObject.SetActive(true);
    }

    #endregion

    #region Get

    public int Get_CurrentIndex()
    {
        return currentIndex;
    }


    #endregion
}
