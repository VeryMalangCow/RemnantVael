using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private AllyProfileDetailEUIController ProfileDetailEUI;

    #endregion

    #region - Hide

    // Profile
    [HideInInspector] private List<AllyProfileEUIController> AllAllyProfileEUIList;
    [HideInInspector] private List<AllyProfileEUIController> CurrentActiveProfileEUIList = new List<AllyProfileEUIController>();

    [HideInInspector] private AllyProfileEUIController CurrentSelectProfileEUI = null;
    [HideInInspector] private AllyProfileEUIController CurrentPickedProfileEUI = null;

    [HideInInspector] private static readonly float AllyProfileIntervalY = 160;
    [HideInInspector] private static readonly float AllyProfileEachHeight = 140;
    [HideInInspector] private static readonly float AllyProfilePanelMinHeight = 800;

    // Picked
    [HideInInspector] private CanvasGroup AllyProfilePickedSignCG;

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
    }

    #endregion

    #region Set (Profile List)

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

    #region Set (Profile Select)

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

    #region Set (Profile Pick)

    // 실제로 인풋으로 클릭이나 지정하는 것
    // 인풋을 통해, Ally를 픽 (UI 변경 필요)
    private void Pick_AllyProfile(AllyProfileEUIController _EUI)
    {
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
    }

    #endregion

    #region Interact

    public virtual bool Try_Interact()
    {
        if (Interact_Profile()) return true;

        return false;
    }
    private bool Interact_Profile()
    {
        if ((CurrentBtn == CurrentSelectProfileEUI) &&
            (CurrentSelectProfileEUI != CurrentPickedProfileEUI))
        {
            Pick_AllyProfile(CurrentSelectProfileEUI);

            return true;
        }

        return false;
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
