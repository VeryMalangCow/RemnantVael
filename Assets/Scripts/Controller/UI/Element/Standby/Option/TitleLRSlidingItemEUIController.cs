using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleLRSlidingItemEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(10)]
    [Header("=== Header")]
    [SerializeField] public TMP_Text HeaderTxt;

    [Space(10)]
    [Header("=== Main")]
    [SerializeField] private Transform MainItemParentTF;

    [Space(10)]
    [Header("=== LR")]
    [SerializeField] public TitleOwnBtnEUIController LeftBtn;
    [SerializeField] public TitleOwnBtnEUIController RightBtn;

    [Space(10)]
    [Header("=== Inner")]

    #endregion

    #region - Hide

    // Main
    [HideInInspector] private List<Transform> MainItemTFList;
    [HideInInspector] private int CurrentIndex = 0;

    #endregion

    #endregion

    #region Offset

    public void Set_OwnerUIController(TitleLobbyUIController _OwnerUIController)
    {
        LeftBtn.OwnerUIController = _OwnerUIController;
        RightBtn.OwnerUIController = _OwnerUIController;
    }

    public override void Offset()
    {
        LeftBtn.Offset();
        RightBtn.Offset();

        MainItemTFList = DevTool.Get_ChildList<Transform>(MainItemParentTF);
        Set_Item(CurrentIndex);
    }

    #endregion

    #region Change

    public void Change_Left(Dele _SetFunc = null)
    {
        CurrentIndex--;
        if (CurrentIndex < 0)
            CurrentIndex = MainItemTFList.Count - 1;

        Set_Item(CurrentIndex);

        if (_SetFunc != null)
            _SetFunc();
    }

    public void Change_Right(Dele _SetFunc = null)
    {
        CurrentIndex++;
        if (CurrentIndex >= MainItemTFList.Count)
            CurrentIndex = 0;

        Set_Item(CurrentIndex);

        if (_SetFunc != null)
            _SetFunc();
    }

    #endregion

    #region Set

    public void Set_Item(int _Index)
    {
        CurrentIndex = _Index;

        for (int i = 0; i < MainItemTFList.Count; i++)
            MainItemTFList[i].gameObject.SetActive(false);

        MainItemTFList[_Index].gameObject.SetActive(true);
    }

    #endregion

    #region Get

    public List<Component> Get_InnerMainColorList()
    {
        return new List<Component>
        {
            DevTool.Get_ComponentTType<TMP_Text>(LeftBtn.transform.GetChild(0).gameObject),
            DevTool.Get_ComponentTType<TMP_Text>(RightBtn.transform.GetChild(0).gameObject),
            HeaderTxt
        };
    }

    public int Get_CurrentIndex()
    {
        return CurrentIndex;
    }


    #endregion
}
