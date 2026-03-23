using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleLRSlidingItemEUIController : ElementUIController
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
    [SerializeField] public TitleOwnBtnEUIController leftBtn;
    [SerializeField] public TitleOwnBtnEUIController rightBtn;

    [Space(10)]
    [Header("=== Inner")]

    #endregion

    #region - Hide

    // Main
    [HideInInspector] private List<Transform> mainItemTfList;
    [HideInInspector] private int currentIndex = 0;

    #endregion

    #endregion

    #region Offset

    public void Set_OwnerUIController(TitleLobbyUIController ownerUIController)
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

    public List<Component> Get_InnerMainColorList()
    {
        return new List<Component>
        {
            DevTool.Get_ComponentTType<TMP_Text>(leftBtn.transform.GetChild(0).gameObject),
            DevTool.Get_ComponentTType<TMP_Text>(rightBtn.transform.GetChild(0).gameObject),
            headerTxt
        };
    }

    public int Get_CurrentIndex()
    {
        return currentIndex;
    }


    #endregion
}
