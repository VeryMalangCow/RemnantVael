using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class HudAllyStateView : MonoBehaviour
{
    // Value
    [Space(10)]
    [Header("=== Ally")]
    [SerializeField] private RectTransform allyStateParentRt;

    [SerializeField] private List<RectTransform> allyHudList = new List<RectTransform>(16);
    private readonly static int maxCol = 6;
    private readonly static float colInterval = -120;
    private readonly static float rowInterval = 240;

    private float defaultAllyStateRectX;

    [Space(10)]
    [Header("=== Visual")]

    private Tween tabTween;

    // Init
    public IEnumerator Init()
    {
        Stopwatch sw = Stopwatch.StartNew();

        // Ally
        defaultAllyStateRectX = allyStateParentRt.anchoredPosition.x;

        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=orange>Ally State View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
        yield return null;
    }

    public void TabOn(float durTime)
    {
        DevTool.SetKillTween(tabTween);
        allyStateParentRt.DOAnchorPosX(700, durTime);
    }

    public void TabOff(float durTime)
    {
        DevTool.SetKillTween(tabTween);
        allyStateParentRt.DOAnchorPosX(defaultAllyStateRectX, durTime);
    }

    public void Add_AllyState(AllyHUDController hud)
    {
        if (hud.gameObject.TryGetComponent(out RectTransform rt))
        {
            rt.gameObject.transform.SetParent(allyStateParentRt);
            rt.gameObject.layer = LayerMask.NameToLayer("UI");

            rt.pivot = new Vector2(0, 1);
            rt.localScale = Vector3.one;

            int currentAmount = allyHudList.Count;
            float y = currentAmount == 0 ? 0 : (currentAmount % maxCol) * colInterval;
            float x = currentAmount == 0 ? 0 : (currentAmount / maxCol) * rowInterval;
            rt.anchoredPosition3D = new Vector3(x, y, 0);

            allyHudList.Add(rt);
        }
    }
}
