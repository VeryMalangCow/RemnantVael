using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllyBaseUpgradeUIController : AllyShopUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Base Upgrade Shop")]

    [Space(10)]
    [Header("=== EUI")]
    [SerializeField] private List<TunerEUIController> AllTunerEUI;

    [Space(10)]
    [Header("=== Txt")]
    [SerializeField] private TMP_Text TunerDetailTxt;
    [SerializeField] private TMP_Text TunerListTxt;


    #endregion

    #region - Hide

    // Tuner
    [SerializeField] private AllyBaseUpradeTunerSet AllyTunerSet;
    [HideInInspector] private static readonly int TunerAmount = 5;
    [HideInInspector] private static int NeedOverrider = 1;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_TunerSet();
        Set_LanguageTxt();
    }

    private void Offset_TunerSet()
    {
        // Data Set (Must set First)
        AllyTunerSet = new AllyBaseUpradeTunerSet();
        AllyTunerSet.Offset(TunerAmount, AllyManager.TunerTypeList, AllyManager.TunerTypePercent);

        // UI Set
        for (int i = 0; i < AllTunerEUI.Count; i++)
        {
            AllTunerEUI[i].OwnerUIController = this;
            AllTunerEUI[i].RerollBtnEUI.OwnerUIController = this;
            AllTunerEUI[i].Offset();
            Set_TunerUI(i);
        }
    }

    #endregion

    #region Set (Tuner Right)

    private void Set_TunerData(int _Index)
    {
        AllyTunerSet.AllyTunerDataList[_Index].Set_Data(AllyManager.TunerTypeList, AllyManager.TunerTypePercent);
    }

    private void Set_TunerUI(int _Index)
    {
        AllTunerEUI[_Index].Set_UI(AllyTunerSet.AllyTunerDataList[_Index], NeedOverrider);
    }

    #endregion

    #region Interact

    public override bool Try_Interact()
    {
        if (base.Try_Interact()) return true;
        if (Is_Interact_CloseBtn()) return true;

        if (Try_Interact_Reroll()) return true;

        return false;
    }

    private bool Is_Interact_CloseBtn()
    {
        if (CurrentBtn == CloseBtn)
        {
            MainGameUIManager.Instance.AllyBaseUpgrade_UIController.SetOff_ThisPanel();
            return true;
        }
        return false;
    }
    #endregion

    #region Interact (Tuner Right)

    private bool Try_Interact_Reroll()
    {
        for (int i = 0; i < AllTunerEUI.Count; i++)
        {
            if (AllTunerEUI[i].RerollBtnEUI == CurrentBtn &&
                PlayerManager.Instance.PlayerController.CurrentOverrider.Value >= NeedOverrider)
            {
                Set_TunerData(i);
                Set_TunerUI(i);
                PlayerManager.Instance.PlayerController.Add_CurrentOverrider(-NeedOverrider);

                Sequence seq = DOTween.Sequence();
                seq.Append(AllTunerEUI[i].Play_Scale(0.95f));
                seq.Append(AllTunerEUI[i].Play_Scale(1f));

                Sequence seq2 = DOTween.Sequence();
                seq2.Append(AllTunerEUI[i].Play_ScaleElements(0.5f));
                seq2.Append(AllTunerEUI[i].Play_ScaleElements(1f));
            }
        }

        return false;
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        // Label
        LabelName = ResourceManager.Instance.Get_StaticWord(95) + " " + ResourceManager.Instance.Get_StaticWord(26) + " " + ResourceManager.Instance.Get_StaticWord(2);
        LabelTxt.text = LabelName;

        // Tuner
        TunerDetailTxt.text = ResourceManager.Instance.Get_StaticWord(103);
        TunerListTxt.text = ResourceManager.Instance.Get_StaticWord(104);

        for (int i = 0; i < AllTunerEUI.Count; i++)
            AllTunerEUI[i].Set_Language();


        base.Set_LanguageTxt();
    }

    #endregion

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        // Dur
        ThisDurEUI.Set_Dur(AllyBaseUpgradeController.UsingShop.CurrentDur);

    }

    #endregion
}
