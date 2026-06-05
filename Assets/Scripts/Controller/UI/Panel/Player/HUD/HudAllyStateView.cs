using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class HudAllyStateView : MonoBehaviour
{
    // Value
    [Space(10)]
    [Header("=== ")]

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private Image[] mainClrImgs;
    [SerializeField] private Image[] subClrImgs;


    // Init
    public IEnumerator Init(Color mainClr, Color subClr)
    {
        Stopwatch sw = Stopwatch.StartNew();


        ColorInit(mainClr, subClr);

        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=orange>AllyState View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
        yield return null;
    }

    private void ColorInit(Color mainClr, Color subClr)
    {
        DevTool.SetColorImgs(mainClr, mainClrImgs);
        mainClrImgs = null;

        DevTool.SetColorImgs(subClr, subClrImgs);
        subClrImgs = null;
    }
}
