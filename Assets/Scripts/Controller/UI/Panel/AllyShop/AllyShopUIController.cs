using System.Collections.Generic;
using UnityEngine;

public class AllyShopUIController : ShopUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Shop")]

    [Space(10)]
    [Header("=== EUI")]
    [SerializeField] private Transform AllyProfileParentTF;

    #endregion

    #region - Hide

    // Profile
    [HideInInspector] private List<AllyProfileEUIController> AllAllyProfileEUIList;
    [HideInInspector] private static readonly float AllyProfileIntervalY = 160;
    [HideInInspector] private static readonly float AllyProfileEachHeight = 140;
    [HideInInspector] private static readonly float AllyProfilePanelMinHeight = 800;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_Comp();
    }

    private void Offset_Comp()
    {
        AllAllyProfileEUIList = new List<AllyProfileEUIController>();

        foreach (Transform TF in AllyProfileParentTF)
        {
            if (TF.TryGetComponent(out AllyProfileEUIController profile))
            {
                profile.Offset();
                AllAllyProfileEUIList.Add(profile);
            }
        }
    }

    #endregion

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        Set_AllyProfilePanel();
    }

    #endregion

    #region Set (Profile List)

    private void Set_AllyProfilePanel()
    {
        List<AllyController> AllAlly = AllyManager.Instance.AllAllies;

        Set_AllAllyProfileOff();
        Set_AllyProfileOn(AllAlly);
        Set_AllyProfilePanelY(AllAlly.Count);
    }

    private void Set_AllAllyProfileOff()
    {
        for (int i = 0; i < AllAllyProfileEUIList.Count; i++)
            AllAllyProfileEUIList[i].gameObject.SetActive(false);
    }

    private void Set_AllyProfileOn(List<AllyController> _AllAlly)
    {
        for (int i = 0; i < _AllAlly.Count; i++)
        {
            AllAllyProfileEUIList[i].gameObject.SetActive(true);
            AllAllyProfileEUIList[i].Set_Profile(_AllAlly[i].Get_FaceImg(), _AllAlly[i].Get_Name(), -(i * AllyProfileIntervalY));
        }
    }

    private void Set_AllyProfilePanelY(int _Amount)
    {
        float y = Mathf.Max(((_Amount - 1) * AllyProfileIntervalY) + AllyProfileEachHeight, AllyProfilePanelMinHeight);
        ThisPanelTabList[0].Set_ScrollPanel(y);
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        // Tab
        TabBtnTxtList = new List<string>
        {
            ResourceManager.Instance.Get_StaticWord(96)
        };

        base.Set_LanguageTxt();
    }

    #endregion

}
