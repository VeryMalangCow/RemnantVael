using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PauseInfoView : MonoBehaviour
{
    [Header("=== Comp")]
    [SerializeField] public TMP_Text listTxt;
    [SerializeField] public TMP_Text detailTxt;

    [SerializeField] public RectTransform panelRt;
    [SerializeField] public OwnBtnEUIController backBtn;
    [SerializeField] private ScrollPanelEUIController listScrollPanelEui;

    [Header("=== Element")]
    [SerializeField] private Transform listElementParentTf;

    [HideInInspector] private InfoEUIController[] listEuiArr;

    [SerializeField] private Transform detailElementParentTf;

    [HideInInspector] private InfoDetailEUIController[] detailEuiArr;
    [HideInInspector] private InfoDetailEUIController currentDetailEui;

    public void Init(PauseUIController ownerUI)
    {
        backBtn.Offset();
        backBtn.ownerUIController = ownerUI;

        listScrollPanelEui.Offset();

        int amount = listElementParentTf.childCount;

        listEuiArr = new InfoEUIController[amount];
        detailEuiArr = new InfoDetailEUIController[amount];
        for (int i = 0; i < amount; i++)
        {
            listEuiArr[i] = listElementParentTf.GetChild(i).TryGetComponent(out InfoEUIController infoListEUI) ? infoListEUI : null;
            listEuiArr[i].ownerUIController = ownerUI;
            listEuiArr[i].Offset();

            detailEuiArr[i] = detailElementParentTf.GetChild(i).TryGetComponent(out InfoDetailEUIController infoDetailEUI) ? infoDetailEUI : null;
            detailEuiArr[i].Offset();
        }

        Set_LanguageTxt();
        Set_List();
    }

    public void Set_List()
    {
        List<EachInfoJsonData> data = SaveDataManager.instance.jsonData.infoData;

        List<InfoEUIController> visibleEUIs = new List<InfoEUIController>();
        for (int i = 0; i < listEuiArr.Length; i++)
        {
            if (data[i].canVisible)
            {
                listEuiArr[i].gameObject.SetActive(true);
                listEuiArr[i].rt.anchoredPosition = new Vector2(listEuiArr[i].rt.anchoredPosition.x, -20 + (-140 * (visibleEUIs.Count)));
                visibleEUIs.Add(listEuiArr[i]);
            }
            else
            {
                listEuiArr[i].gameObject.SetActive(false);
            }
        }

        float y = (140 * visibleEUIs.Count) + 20;
        listScrollPanelEui.Set_ScrollHeight(y);
    }

    public void Set_Panel(bool onOff)
    {
        panelRt.gameObject.SetActive(onOff);

        if (!onOff)
        {
            SetOff_DetailWindow();
        }
    }

    public bool Is_ListBtn(OwnBtnEUIController btn)
    {
        return listEuiArr.Contains((InfoEUIController)btn);
    }

    public void SetOn_DetailWindow(InfoEUIController listBtn)
    {
        SetOff_DetailWindow();

        int index = Array.IndexOf(listEuiArr, listBtn);
        currentDetailEui = detailEuiArr[index];
        currentDetailEui.gameObject.SetActive(true);
    }

    private void SetOff_DetailWindow()
    {
        if (currentDetailEui != null)
        {
            currentDetailEui.gameObject.SetActive(false);
            currentDetailEui = null;
        }
    }

    public void Set_LanguageTxt()
    {
        listTxt.text = ResourceManager.instance.Get_StaticWord(145);
        detailTxt.text = ResourceManager.instance.Get_StaticWord(144);

        for (int i = 0; i < listEuiArr.Length; i++)
            listEuiArr[i].Set_LanguageTxt();

        for (int i = 0; i < detailEuiArr.Length; i++)
            detailEuiArr[i].Set_LanguageTxt();
    }
}
