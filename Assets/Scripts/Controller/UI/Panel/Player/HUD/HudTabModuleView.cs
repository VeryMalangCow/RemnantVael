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
    
    private Tween tabTween;

    // Init
    public IEnumerator Init()
    {
        Stopwatch sw = Stopwatch.StartNew();

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

        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=orange>Tab Module View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
        yield return null;
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
