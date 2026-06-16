using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyShopUIController : ShopUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Ally Shop")]

    [Space(10)]
    [Header("=== Profile List EUI")]
    [SerializeField] private Transform allyProfileParentTf;
    [SerializeField] private RectTransform allyProfileSelectSignRt;
    [SerializeField] private RectTransform allyProfilePickedSignRt;

    [Space(10)]
    [Header("=== Profile Detail EUI")]

    [Space(5)]
    [Header("-- Detail")]
    [SerializeField] private AllyProfileDetailEUIController profileDetailEuiPrefab;
    public AllyProfileDetailEUIController profileDetailEui { get; private set; }

    [Space(10)]
    [Header("=== Inner")]
    [SerializeField] private List<Image> extraMainClrImgList;
    [SerializeField] private List<TMP_Text> extraMainClrTxtList;

    [SerializeField] private List<Image> extraSubClrImgList;
    [SerializeField] private List<TMP_Text> extraSubClrTxtList;


    protected bool isTweening = false;
    // Profile List
    [HideInInspector] private List<AllyProfileEUIController> allAllyProfileEuiList;
    [HideInInspector] private List<AllyProfileEUIController> currentActiveProfileEuiList = new List<AllyProfileEUIController>();

    [HideInInspector] private AllyProfileEUIController currentSelectProfileEui = null;
    [HideInInspector] protected AllyProfileEUIController currentPickedProfileEui = null;

    [HideInInspector] private static readonly float allyProfileIntervalY = 160;
    [HideInInspector] private static readonly float allyProfilePanelMinHeight = 800;

    // Picked
    [HideInInspector] protected AllyController currentPickedAlly = null;
    [HideInInspector] private CanvasGroup allyProfilePickedSignCg;


    #endregion

    public override IEnumerator InitAsync()
    {
        yield return base.InitAsync();
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        // Profile List
        allAllyProfileEuiList = new List<AllyProfileEUIController>();

        foreach (Transform TF in allyProfileParentTf)
        {
            if (TF.TryGetComponent(out AllyProfileEUIController profile))
            {
                profile.ownerUIController = this;
                profile.Offset();
                allAllyProfileEuiList.Add(profile);
            }
        }

        allyProfilePickedSignCg = DevTool.Get_ComponentTType(allyProfilePickedSignRt.gameObject, out CanvasGroup cg) ? cg : null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Profile List</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;



#if UNITY_EDITOR
        sw.Restart();
#endif
        profileDetailEui = Instantiate(profileDetailEuiPrefab, transform);
        profileDetailEui.Offset();
        profileDetailEuiPrefab = null;
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Profile Detail -> Gen</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;

        yield return profileDetailEui.InitAsync(this);

        SetColor();
        yield return null;
    }

    public void SetColor()
    {
        mainColorCompList = new List<Component>();
        subColorCompList = new List<Component>();

        mainColorCompList.AddRange(extraMainClrImgList);
        mainColorCompList.AddRange(extraMainClrTxtList);
        subColorCompList.AddRange(extraSubClrImgList);
        subColorCompList.AddRange(extraSubClrTxtList);

        // Set Color
        Color mainClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, mainColorCompList);
        mainColorCompList.Clear();
        mainColorCompList = null;

        Color subClr = PlayerManager.instance.playerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, subColorCompList);
        subColorCompList.Clear();
        subColorCompList = null;

        profileDetailEui.SetColor();
    }

    #region Reset

    // 모든 데이터 초기화
    private void Reset_AllData()
    {
        currentActiveProfileEuiList.Clear();
        currentSelectProfileEui = null;
        currentPickedProfileEui = null;

        allyProfilePickedSignRt.gameObject.SetActive(false);
        isTweening = false;

        profileDetailEui.ResetData();
    }

    // 프로필 리스트 리셋
    private void Reset_AllyProfileListPanel()
    {
        List<AllyController> allAlly = AllyManager.instance.allAlly;

        Set_AllAllyProfileListOff();
        Set_AllyProfileListOn(allAlly);
        Set_AllyProfileListPanelY(allAlly.Count);
        Select_DefaultAllyProfile();
    }
    
    // 프로필 디테일 리셋
    private void Reset_AllyProfileDetailPanel()
    {
        profileDetailEui.SetOff_Panel();

    }

    #endregion

    #region Set (Panel)

    public override void SetOnThisPanel()
    {
        base.SetOnThisPanel();

        Reset_AllData();
        Reset_AllyProfileListPanel();
        Reset_AllyProfileDetailPanel();

        profileDetailEui.Play_ProfileExtraY(0.3f);
    }

    #endregion

    #region Profile List

    // 모든 프로필 끄기
    private void Set_AllAllyProfileListOff()
    {
        for (int i = 0; i < allAllyProfileEuiList.Count; i++)
        {
            allAllyProfileEuiList[i].gameObject.SetActive(false);
            allAllyProfileEuiList[i].Reset_Profile();
        }
    }

    // 프로필 키기 (현재 Ally)
    private void Set_AllyProfileListOn(List<AllyController> allAlly)
    {

        for (int i = 0; i < allAlly.Count; i++)
        {
            allAllyProfileEuiList[i].gameObject.SetActive(true);
            allAllyProfileEuiList[i].Set_Profile(allAlly[i], -(i * allyProfileIntervalY));
            currentActiveProfileEuiList.Add(allAllyProfileEuiList[i]);
        }
    }

    // 프로필 항목 패널의 사이즈 조절 (Scroll을 위함)
    private void Set_AllyProfileListPanelY(int amount)
    {
        float y = Mathf.Max(amount * allyProfileIntervalY, allyProfilePanelMinHeight);
        panelTabList[0].Set_ScrollHeight(y);
    }

    #endregion

    #region Profile Select

    // 처음 시작 시, 첫번째 Ally 선택 (없다면 NULL)
    private void Select_DefaultAllyProfile()
    {
        if (currentActiveProfileEuiList.Count <= 0) // 없다면
        {
            Select_AllyProfile(null);
            allyProfileSelectSignRt.gameObject.SetActive(false);
        }
        else
        {
            Select_AllyProfile(currentActiveProfileEuiList[0]);
            allyProfileSelectSignRt.gameObject.SetActive(true);
        }
    }

    public void Select_AllyProfile(AllyProfileEUIController eui)
    {
        if (currentSelectProfileEui == eui) return;

        currentSelectProfileEui = eui;

        allyProfileSelectSignRt.SetParent(currentSelectProfileEui.transform);
        allyProfileSelectSignRt.SetAsLastSibling();

        DevTool.SetKillTween(allyProfileSelectSignRt);
        allyProfileSelectSignRt.DOAnchorPos(Vector2.zero, 0.05f);
    }

    #endregion

    #region Profile Pick

    // 실제로 인풋으로 클릭이나 지정하는 것
    // 인풋을 통해, Ally를 픽 (UI 변경 필요)
    protected virtual void Pick_AllyProfile(AllyProfileEUIController eui)
    {
        if (isTweening) return;

        currentPickedProfileEui = eui;
        currentPickedAlly = currentPickedProfileEui.Get_ThisAlly();

        allyProfilePickedSignRt.gameObject.SetActive(true);
        allyProfilePickedSignRt.SetParent(currentSelectProfileEui.transform);
        allyProfilePickedSignRt.SetAsLastSibling();
        allyProfilePickedSignRt.anchoredPosition = Vector2.zero;

        DevTool.SetKillTween(allyProfilePickedSignRt);
        allyProfilePickedSignRt.transform.localScale = Vector2.one * 1.2f;
        allyProfilePickedSignRt.transform.DOScale(Vector2.one, 0.15f);

        DevTool.SetKillTween(allyProfilePickedSignCg);
        allyProfilePickedSignCg.alpha = 0f;
        allyProfilePickedSignCg.DOFade(1f, 0.2f);

        // Data UI Set
        Set_AllyState(currentPickedAlly);
        Set_AllyTuner(currentPickedAlly);
        Set_AllySync(currentPickedAlly);

        // Detail Panel
        profileDetailEui.SetOn_Panel(currentPickedAlly);

        // Extra Panel
        profileDetailEui.Play_ProfileExtraY_CloseAndOpen(0, currentPickedProfileEui);

        // Sound
        SoundManager.instance.Play_2D_SFX_UI("Click_01");
    }

    #endregion

    // State
    protected void Set_AllyState(AllyController ally)
    {
        profileDetailEui.Set_AllyState(ally);
    }

    // Tuner
    protected void Set_AllyTuner(AllyController ally)
    {
        profileDetailEui.Set_AllyTuner(ally);
    }

    // Sync
    protected void Set_AllySync(AllyController ally)
    {
        profileDetailEui.Set_AllySync(ally);
    }

    // Interact
    public virtual bool Try_Interact()
    {
        if (Interact_ProfileList()) return true;
        if (Interact_ProfileExtraBtn()) return true;

        return false;
    }


    private bool Interact_ProfileList()
    {
        if ((currentBtn == currentSelectProfileEui) &&
            (currentSelectProfileEui != currentPickedProfileEui))
        {
            Pick_AllyProfile(currentSelectProfileEui);

            return true;
        }

        return false;
    }

    private bool Interact_ProfileExtraBtn()
    {
        if (currentBtn is OwnCGBtnEUIController btn &&
            profileDetailEui.stateBtnList.Contains(btn))
        {
            profileDetailEui.Play_ProfileExtraY_CloseAndOpen(profileDetailEui.stateBtnList.IndexOf(btn), currentPickedProfileEui);

            return true;
        }

        return false;
    }

    // Tween
    protected void Play_UseTxt(TMP_Text txt, int pay, float upY, float durTime = 0.5f)
    {
        RectTransform rt = DevTool.Get_ComponentTType<RectTransform>(txt.gameObject);

        DevTool.SetKillTween(txt);
        DevTool.SetKillTween(rt);

        txt.text = $"-{pay}";
        DevTool.Set_AlphaColor(txt, 1f);
        rt.anchoredPosition = Vector2.zero;

        txt.DOFade(0f, durTime);
        rt.DOAnchorPosY(upY, durTime);
    }

    // Language
    public override void SetLanguageTxt()
    {
        // Profile List (Tab)
        tabBtnTxtList = new List<string>
        {
            ResourceManager.instance.Get_StaticWord(96)
        };

        profileDetailEui.SetLanguageTxt();

        base.SetLanguageTxt();
    }

}
