using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyShopUIController : ShopUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Ally Shop")]

    [Space(10)]
    [Header("=== Profile EUI")]
    [SerializeField] private AllyProfileListView profileListEuiPrefab;
    public AllyProfileListView profileListEui { get; private set; }

    [SerializeField] private AllyProfileDetailView profileDetailEuiPrefab;
    public AllyProfileDetailView profileDetailEui { get; private set; }
    [SerializeField] private Transform profileParentTf;

    [Space(10)]
    [Header("=== Inner")]
    [SerializeField] private List<Image> extraMainClrImgList;
    [SerializeField] private List<TMP_Text> extraMainClrTxtList;
    [SerializeField] private List<Image> extraSubClrImgList;

    protected bool isTweening = false;

    #endregion

    public override IEnumerator InitAsync(Color mainClr, Color subClr)
    {
        yield return base.InitAsync(mainClr, subClr);
#if UNITY_EDITOR
        Stopwatch sw = Stopwatch.StartNew();
#endif
        profileListEui = Instantiate(profileListEuiPrefab, profileParentTf);
        profileListEuiPrefab = null;
        profileListEui.Init(this);
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Profile List</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;

#if UNITY_EDITOR
        sw.Restart();
#endif
        profileDetailEui = Instantiate(profileDetailEuiPrefab, profileParentTf);
        profileDetailEuiPrefab = null;
        profileDetailEui.Init(this);
#if UNITY_EDITOR
        sw.Stop();
        UnityEngine.Debug.Log($"<color=yellow>Profile Detail</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
#endif
        yield return null;
        panelTabList = new List<TabEUIController>() { profileListEui.thisTabEui };
        SetColor(mainClr, subClr);
        yield return null;
    }

    public void SetColor(Color mainClr, Color subClr)
    {
        DevTool.SetColorImgs(mainClr, extraMainClrImgList);
        DevTool.SetColorTmps(mainClr, extraMainClrTxtList);
        DevTool.SetColorImgs(subClr, extraSubClrImgList);

        profileListEui.SetColor(mainClr, subClr);
        profileDetailEui.SetColor(mainClr, subClr);
    }

    #region Set (Panel)

    public override void SetOnThisPanel()
    {
        base.SetOnThisPanel();

        // 모든 데이터 초기화
        isTweening = false;
        profileListEui.ResetData();
        profileDetailEui.ResetData();

        // 프로필 리스트 리셋
        List<AllyController> allAlly = AllyManager.instance.allAlly;
        profileListEui.SetOnProfileListOn(allAlly);

        // 프로필 디테일 리셋
        profileDetailEui.SetOff_Panel();
        profileDetailEui.Play_ProfileExtraY(0.3f);
    }

    #endregion

    // 실제로 인풋으로 클릭이나 지정하는 것
    // 인풋을 통해, Ally를 픽 (UI 변경 필요)
    protected virtual void Pick_AllyProfile(AllyProfileEUIController eui)
    {
        if (isTweening) return;

        AllyProfileEUIController allyProfileEui = profileListEui.currentPickedProfileEui;
        profileListEui.Pick_AllyProfile(eui);

        AllyController ally = profileListEui.currentPickedAlly;
        profileDetailEui.Set_AllyState(ally);
        profileDetailEui.Set_AllyTuner(ally);
        profileDetailEui.Set_AllySync(ally);
        profileDetailEui.SetOn_Panel(ally);
        profileDetailEui.Play_ProfileExtraY_CloseAndOpen(0, allyProfileEui);

        SoundManager.instance.PlayUiSfx("Click01");
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
        if ((currentBtn == profileListEui.currentSelectProfileEui) &&
            (profileListEui.currentSelectProfileEui != profileListEui.currentPickedProfileEui))
        {
            Pick_AllyProfile(profileListEui.currentSelectProfileEui);

            return true;
        }

        return false;
    }

    private bool Interact_ProfileExtraBtn()
    {
        if (currentBtn is OwnCGBtnEUIController btn &&
            profileDetailEui.stateBtnList.Contains(btn))
        {
            profileDetailEui.Play_ProfileExtraY_CloseAndOpen(profileDetailEui.stateBtnList.IndexOf(btn), profileListEui.currentPickedProfileEui);

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
            StaticResourceManager.instance.staticWords.GetLanguage(96)
        };

        profileDetailEui.SetLanguageTxt();

        base.SetLanguageTxt();
    }

}
