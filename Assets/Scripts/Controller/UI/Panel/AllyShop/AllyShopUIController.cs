using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyShopUIController : ShopUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Shop")]

    [Space(10)]
    [Header("=== Profile List EUI")]
    [SerializeField] private Transform AllyProfileParentTF;
    [SerializeField] private RectTransform AllyProfileSelectSignRT;
    [SerializeField] private RectTransform AllyProfilePickedSignRT;

    [Space(10)]
    [Header("=== Profile Detail EUI")]

    [Space(5)]
    [Header("-- Detail")]
    [SerializeField] private AllyProfileDetailEUIController ProfileDetailEUI;
    [SerializeField] private TMP_Text ProfileDetailTxt;
    [SerializeField] private RectTransform ProfileDetailExtraRT;

    [Space(5)]
    [Header("-- Panel")]
    [SerializeField] private List<RectTransform> StatePanelRTList;
    [SerializeField] private CanvasGroup StatePanelCG;

    [Space(5)]
    [Header("-- Btn")]
    [SerializeField] private List<OwnCGBtnEUIController> StateBtnList;


    [Space(10)]
    [Header("=== Inner")]
    [SerializeField] private List<Image> ExtraMainColorImgList;
    [SerializeField] private List<TMP_Text> ExtraMainColorTxtList;

    [SerializeField] private List<Image> ExtraSubColorImgList;
    [SerializeField] private List<TMP_Text> ExtraSubColorTxtList;

    #endregion

    #region - Hide

    // Profile List
    [HideInInspector] private List<AllyProfileEUIController> AllAllyProfileEUIList;
    [HideInInspector] private List<AllyProfileEUIController> CurrentActiveProfileEUIList = new List<AllyProfileEUIController>();

    [HideInInspector] private AllyProfileEUIController CurrentSelectProfileEUI = null;
    [HideInInspector] private AllyProfileEUIController CurrentPickedProfileEUI = null;

    [HideInInspector] private static readonly float AllyProfileIntervalY = 160;
    [HideInInspector] private static readonly float AllyProfileEachHeight = 140;
    [HideInInspector] private static readonly float AllyProfilePanelMinHeight = 800;

    // Picked
    [HideInInspector] private CanvasGroup AllyProfilePickedSignCG;

    // Profile Detail
    [HideInInspector] private bool ProfileDetailExtraIsOpen = false;
    [HideInInspector] private bool IsTweening = false;
    [HideInInspector] private static readonly float ProfileDetailExtraRT_OpenHeight = 650f;
    [HideInInspector] private static readonly float ProfileDetailExtraRT_CloseHeight = 80f;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_Comp();
        Offset_ColorComp();
    }

    private void Offset_Comp()
    {
        // Profile List
        AllAllyProfileEUIList = new List<AllyProfileEUIController>();

        foreach (Transform TF in AllyProfileParentTF)
        {
            if (TF.TryGetComponent(out AllyProfileEUIController profile))
            {
                profile.OwnerUIController = this;
                profile.Offset();
                AllAllyProfileEUIList.Add(profile);
            }
        }

        AllyProfilePickedSignCG = DevTool.Get_ComponentTType(AllyProfilePickedSignRT.gameObject, out CanvasGroup cg) ? cg : null;


        // Profile Detail
        ProfileDetailEUI.Offset();
        
        for (int i = 0; i < StateBtnList.Count; i++)
        {
            StateBtnList[i].OwnerUIController = this;
            StateBtnList[i].Offset();
        }
    }

    private void Offset_ColorComp()
    {
        MainColorCompList = new List<Component>();
        SubColorCompList = new List<Component>();

        MainColorCompList.AddRange(ExtraMainColorImgList);
        MainColorCompList.AddRange(ExtraMainColorTxtList);
        SubColorCompList.AddRange(ExtraSubColorImgList);
        SubColorCompList.AddRange(ExtraSubColorTxtList);

        // Set Color
        Color mainClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false);
        Debug.Log(mainClr);
        DevTool.Set_Color(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, true);
        Debug.Log(subClr);
        DevTool.Set_Color(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;
    }

    #endregion

    #region Reset

    // 모든 데이터 초기화
    private void Reset_AllData()
    {
        CurrentActiveProfileEUIList.Clear();
        CurrentSelectProfileEUI = null;
        CurrentPickedProfileEUI = null;
        AllyProfilePickedSignRT.gameObject.SetActive(false);

        ProfileDetailExtraIsOpen = false;
        IsTweening = false;
        StatePanelCG.alpha = 0;
    }

    // 프로필 리스트 리셋
    private void Reset_AllyProfileListPanel()
    {
        List<AllyController> AllAlly = AllyManager.Instance.AllAllies;

        Set_AllAllyProfileListOff();
        Set_AllyProfileListOn(AllAlly);
        Set_AllyProfileListPanelY(AllAlly.Count);
        Select_DefaultAllyProfile();
    }
    
    // 프로필 디테일 리셋
    private void Reset_AllyProfileDetailPanel()
    {
        ProfileDetailEUI.SetOff_Panel();

    }

    #endregion

    #region Set (Panel)

    public override void SetOn_ThisPanel()
    {
        base.SetOn_ThisPanel();

        Reset_AllData();
        Reset_AllyProfileListPanel();
        Reset_AllyProfileDetailPanel();

        Play_ProfileExtraY(ProfileDetailExtraRT_CloseHeight, 0.3f);

    }

    #endregion

    #region Profile List

    // 모든 프로필 끄기
    private void Set_AllAllyProfileListOff()
    {
        for (int i = 0; i < AllAllyProfileEUIList.Count; i++)
        {
            AllAllyProfileEUIList[i].gameObject.SetActive(false);
            AllAllyProfileEUIList[i].Reset_Profile();
        }
    }

    // 프로필 키기 (현재 Ally)
    private void Set_AllyProfileListOn(List<AllyController> _AllAlly)
    {

        for (int i = 0; i < _AllAlly.Count; i++)
        {
            AllAllyProfileEUIList[i].gameObject.SetActive(true);
            AllAllyProfileEUIList[i].Set_Profile(_AllAlly[i], -(i * AllyProfileIntervalY));
            CurrentActiveProfileEUIList.Add(AllAllyProfileEUIList[i]);
        }
    }

    // 프로필 패널의 사이즈 조절 (Scroll을 위함)
    private void Set_AllyProfileListPanelY(int _Amount)
    {
        float y = Mathf.Max(((_Amount - 1) * AllyProfileIntervalY) + AllyProfileEachHeight, AllyProfilePanelMinHeight);
        ThisPanelTabList[0].Set_ScrollPanel(y);
    }

    #endregion

    #region Profile Select

    // 처음 시작 시, 첫번째 Ally 선택 (없다면 NULL)
    private void Select_DefaultAllyProfile()
    {
        if (CurrentActiveProfileEUIList.Count <= 0) // 없다면
        {
            Select_AllyProfile(null);
            AllyProfileSelectSignRT.gameObject.SetActive(false);
        }
        else
        {
            Select_AllyProfile(CurrentActiveProfileEUIList[0]);
            AllyProfileSelectSignRT.gameObject.SetActive(true);
        }
    }

    public void Select_AllyProfile(AllyProfileEUIController _EUI)
    {
        if (CurrentSelectProfileEUI == _EUI) return;

        CurrentSelectProfileEUI = _EUI;

        AllyProfileSelectSignRT.SetParent(CurrentSelectProfileEUI.transform);
        AllyProfileSelectSignRT.SetAsLastSibling();

        DevTool.Set_KillTween(AllyProfileSelectSignRT);
        AllyProfileSelectSignRT.DOAnchorPos(Vector2.zero, 0.05f);
    }

    #endregion

    #region Profile Pick

    // 실제로 인풋으로 클릭이나 지정하는 것
    // 인풋을 통해, Ally를 픽 (UI 변경 필요)
    private void Pick_AllyProfile(AllyProfileEUIController _EUI)
    {
        if (IsTweening) return;

        CurrentPickedProfileEUI = _EUI;

        AllyProfilePickedSignRT.gameObject.SetActive(true);
        AllyProfilePickedSignRT.SetParent(CurrentSelectProfileEUI.transform);
        AllyProfilePickedSignRT.SetAsLastSibling();
        AllyProfilePickedSignRT.anchoredPosition = Vector2.zero;

        DevTool.Set_KillTween(AllyProfilePickedSignRT);
        AllyProfilePickedSignRT.transform.localScale = Vector2.one * 1.2f;
        AllyProfilePickedSignRT.transform.DOScale(Vector2.one, 0.15f);

        DevTool.Set_KillTween(AllyProfilePickedSignCG);
        AllyProfilePickedSignCG.alpha = 0f;
        AllyProfilePickedSignCG.DOFade(1f, 0.2f);

        // Detail Panel
        ProfileDetailEUI.SetOn_Panel(CurrentPickedProfileEUI.Get_ThisAlly());

        // Extra Panel
        Play_ProfileExtraY_CloseAndOpen(0);
    }

    #endregion

    #region Profile Detail

    private Sequence Play_ProfileDetailExtraCG(int _ExtraIndex, float _DurTime)
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(StatePanelCG.DOFade(0f, _DurTime * 0.5f).OnComplete(() =>
        {
            for (int i = 0; i < StateBtnList.Count; i++)
            {
                int index = i;

                if (index == _ExtraIndex)
                    StatePanelRTList[index].gameObject.SetActive(true);
                else
                    StatePanelRTList[index].gameObject.SetActive(false);
            }
        }));
        seq.Append(StatePanelCG.DOFade(1f, _DurTime * 0.5f));

        return seq;
    }

    private Sequence Play_ProfileDetailExtraBtn(int _ExtraIndex, float _DurTime)
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < StateBtnList.Count; i++)
        {
            int index = i;

            if (index == _ExtraIndex)
                seq.Join(StateBtnList[index].ThisCG.DOFade(1f, _DurTime));
            else
                seq.Join(StateBtnList[index].ThisCG.DOFade(0.5f, _DurTime));
        }

        return seq;
    }

    private Sequence Play_ProfileExtraY_CloseAndOpen(int _ExtraIndex)
    {
        if (IsTweening || CurrentPickedProfileEUI == null) return null;
        IsTweening = true;

        Sequence seq = DOTween.Sequence();

        float durTime = ProfileDetailExtraIsOpen ? 0.24f : 0.12f;

        if (ProfileDetailExtraIsOpen)
        {
            seq.Append(Play_ProfileExtraY(ProfileDetailExtraRT_CloseHeight, 0.12f));
            seq.Append(Play_ProfileExtraY(ProfileDetailExtraRT_OpenHeight, 0.12f));
        }
        else
        {
            seq.Append(Play_ProfileExtraY(ProfileDetailExtraRT_OpenHeight, 0.12f));
        }

        Play_ProfileDetailExtraBtn(_ExtraIndex, durTime);
        Play_ProfileDetailExtraCG(_ExtraIndex, durTime);

        ProfileDetailExtraIsOpen = true;

        seq.OnComplete(() => { IsTweening = false; });

        return seq;
    }


    private Sequence Play_ProfileExtraY(float _Y, float _DurTime = 0f)
    {
        DevTool.Set_KillTween(ProfileDetailExtraRT);

        Sequence seq = DOTween.Sequence();

        seq.Append(ProfileDetailExtraRT.DOSizeDelta(new Vector2(ProfileDetailExtraRT.rect.width, _Y), _DurTime));

        return seq;
    }


    #endregion

    #region Interact

    public virtual bool Try_Interact()
    {
        if (Interact_ProfileList()) return true;
        if (Interact_ProfileExtraBtn()) return true;

        return false;
    }

    private bool Interact_ProfileList()
    {
        if ((CurrentBtn == CurrentSelectProfileEUI) &&
            (CurrentSelectProfileEUI != CurrentPickedProfileEUI))
        {
            Pick_AllyProfile(CurrentSelectProfileEUI);

            return true;
        }

        return false;
    }

    private bool Interact_ProfileExtraBtn()
    {
        if (CurrentBtn is OwnCGBtnEUIController btn &&
            StateBtnList.Contains(btn))
        {
            Play_ProfileExtraY_CloseAndOpen(StateBtnList.IndexOf(btn));

            return true;
        }

        return false;
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        // Profile List (Tab)
        TabBtnTxtList = new List<string>
        {
            ResourceManager.Instance.Get_StaticWord(96)
        };

        // Profile Detail
        ProfileDetailTxt.text = ResourceManager.Instance.Get_StaticWord(101);

        base.Set_LanguageTxt();
    }

    #endregion

}
