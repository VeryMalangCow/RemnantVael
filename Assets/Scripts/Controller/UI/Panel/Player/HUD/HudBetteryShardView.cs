using System.Diagnostics;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HudBetteryShardView : MonoBehaviour
{
    [Space(10)]
    [Header("=== Image")]
    [SerializeField] public ChargeSpriteEUIController currentEmptyBc;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private Image[] subClrImgs;


    // Init
    public IEnumerator Init(Color subClr)
    {
        Stopwatch sw = Stopwatch.StartNew();

        currentEmptyBc.Offset();

        ColorInit(subClr);

        sw.Stop();
        UnityEngine.Debug.Log($"Player HUD : <color=orange>Bettery Shard View</color> : <color=red>{sw.Elapsed.TotalMilliseconds:F2}</color> ms");
        yield return null;
    }

    private void ColorInit(Color subClr)
    {
        DevTool.SetColorImgs(subClr, subClrImgs);
        subClrImgs = null;
    }

    public void SetBetteryShard(int amount)
    {
        currentEmptyBc.ChangeSprite(amount);
    }

}
