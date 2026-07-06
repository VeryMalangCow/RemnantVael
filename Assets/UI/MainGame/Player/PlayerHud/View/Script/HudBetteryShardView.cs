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
    public void Init(Color subClr)
    {
        currentEmptyBc.Offset();

        ColorInit(subClr);

        gameObject.SetActive(true);
    }

    private void ColorInit(Color subClr)
    {
        DevTool.SetColorImgs(subClr, subClrImgs);
        subClrImgs = null;
    }

    public void SetBetteryShardUI(int amount)
    {
        currentEmptyBc.ChangeSprite(amount);
    }

}
