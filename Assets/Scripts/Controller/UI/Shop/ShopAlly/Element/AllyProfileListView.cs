using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyProfileListView : MonoBehaviour
{
    [SerializeField] public TabEUIController thisTabEui;
    [SerializeField] private Transform allyProfileParentTf;
    [SerializeField] private RectTransform allyProfileSelectSignRt;
    [SerializeField] private RectTransform allyProfilePickedSignRt;
    private CanvasGroup allyProfilePickedSignCg;

    [Header("=== Visual")]
    [SerializeField] private TMP_Text[] mainTmps;
    [SerializeField] private Image[] subImgs;

    [HideInInspector] public List<AllyProfileEUIController> allAllyProfileEuiList;
    [HideInInspector] public List<AllyProfileEUIController> currentActiveProfileEuiList = new List<AllyProfileEUIController>();

    public AllyProfileEUIController currentSelectProfileEui { get; private set; } = null;
    public AllyProfileEUIController currentPickedProfileEui { get; private set; } = null;

    private static readonly float allyProfileIntervalY = 160;
    private static readonly float allyProfilePanelMinHeight = 800;

    public AllyController currentPickedAlly { get; private set; } = null;

    public void Init(AllyShopUIController ui)
    {
        // Profile List
        allAllyProfileEuiList = new List<AllyProfileEUIController>();

        foreach (Transform TF in allyProfileParentTf)
        {
            if (TF.TryGetComponent(out AllyProfileEUIController profile))
            {
                profile.ownerUIController = ui;
                profile.Offset();
                allAllyProfileEuiList.Add(profile);
            }
        }

        allyProfilePickedSignCg = DevTool.Get_ComponentTType(allyProfilePickedSignRt.gameObject, out CanvasGroup cg) ? cg : null;
    }

    public void ResetData()
    {
        currentActiveProfileEuiList.Clear();
        currentSelectProfileEui = null;
        currentPickedProfileEui = null;

        allyProfilePickedSignRt.gameObject.SetActive(false);
    }

    public void SetColor(Color mainClr, Color subClr)
    {
        DevTool.SetColorTmps(mainClr, mainTmps);
        DevTool.SetColorImgs(subClr, subImgs);
    }

    #region Profile List

    public void SetOnProfileListOn(List<AllyController> allAlly)
    {
        Set_AllAllyProfileListOff();
        Set_AllyProfileListOn(allAlly);
        Set_AllyProfileListPanelY(allAlly.Count);
        Select_DefaultAllyProfile();
    }

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
        thisTabEui.Set_ScrollHeight(y);
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
    public void Pick_AllyProfile(AllyProfileEUIController eui)
    {
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
    }


    #endregion
}
