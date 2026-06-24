using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class HudTabModuleView : MonoBehaviour
{
    [Space(10)]
    [Header("=== Tab States")]
    [SerializeField] private RectTransform moduleListParentRt;

    private float defaultModuleRectX;
    [HideInInspector] public List<InventorySlotEUIController> moduleSlots = new List<InventorySlotEUIController>();
    
    private Tween tabTween = null;

    // Init
    public void Init()
    {
        defaultModuleRectX = DevTool.Get_ComponentTType(
            moduleListParentRt.gameObject, out RectTransform module_Rt) ?
                module_Rt.anchoredPosition.x : 0f;

        moduleSlots = DevTool.Get_ChildList<InventorySlotEUIController>(moduleListParentRt);
        for (int i = 0; i < moduleSlots.Count; i++)
        {
            moduleSlots[i].Offset();
            moduleSlots[i].item.Offset();
            moduleSlots[i].Set_EquipedTxt(true, i);
        }

        gameObject.SetActive(true);
    }

    public void TabOn(float durTime)
    {
        DevTool.SetKillTween(tabTween);
        moduleListParentRt.DOAnchorPosX(0, durTime);
    }

    public void TabOff(float durTime)
    {
        DevTool.SetKillTween(tabTween);
        moduleListParentRt.DOAnchorPosX(defaultModuleRectX, durTime);
    }

}
