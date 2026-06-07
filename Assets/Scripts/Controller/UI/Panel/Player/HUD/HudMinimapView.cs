using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudMinimapView : MonoBehaviour
{
    [Space(10)]
    [Header("=== Stage Txt")]
    [SerializeField] private MinimapEUIController minimapEui;
    [SerializeField] private TMP_Text stageNameTxt;
    [SerializeField] private TMP_Text stageDescTxt;
    [SerializeField] private Image stageIcon;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private Image[] mainClrImgs;
    [SerializeField] private TMP_Text[] mainClrTmps;
    [SerializeField] private Image[] subClrImgs;

    private Sequence tween;

    // Init
    public void Init(Color mainClr, Color subClr)
    {
        minimapEui.Offset();

        DevTool.Set_AlphaColor(stageNameTxt, 1);
        DevTool.Set_AlphaColor(stageDescTxt, 0);

        ColorInit(mainClr, subClr);

        gameObject.SetActive(true);
    }

    private void ColorInit(Color mainClr, Color subClr)
    {
        DevTool.SetColorImgs(mainClr, mainClrImgs);
        mainClrImgs = null;
        DevTool.SetColorTmps(mainClr, mainClrTmps);
        mainClrTmps = null;

        DevTool.SetColorImgs(subClr, subClrImgs);
        subClrImgs = null;
    }
    public void TabOn(float durTime)
    {
        DevTool.SetKillTween(tween);
        
        tween.Join(stageNameTxt.DOFade(0f, durTime));
        tween.Join(stageDescTxt.DOFade(1f, durTime)); 
        minimapEui.SetOn_TabInteract(durTime);
    }

    public void TabOff(float durTime)
    {
        DevTool.SetKillTween(tween);

        tween.Join(stageNameTxt.DOFade(1f, durTime));
        tween.Join(stageDescTxt.DOFade(0f, durTime));
        minimapEui.SetOff_TabInteract(durTime);
    }

    public void SetStageDescription()
    {
        stageNameTxt.DOText(
            ResourceManager.instance.Get_MapName(
                StageManager.instance.Get_CurrentStageData().infoData.stageId), 0.5f)
            .OnPlay(() => { stageNameTxt.text = ""; });

        stageDescTxt.DOText(
            ResourceManager.instance.Get_MapDesc(
                StageManager.instance.Get_CurrentStageData().infoData.stageId), 0.5f)
            .OnPlay(() => { stageDescTxt.text = ""; });
    }

    public void SetOnMapIcon(int stageId)
    {
        stageIcon.gameObject.SetActive(true);
        stageIcon.sprite = ResourceManager.instance.Get_StageIcon(stageId);
    }

    public void SetOffMapIcon()
    {
        stageIcon.gameObject.SetActive(false);
    }

    public void AllRemoveMinimapCell()
    {
        minimapEui.Remove_AllMinimapCell();
    }

    public void Set_State()
    {
        minimapEui.Set_State();
    }
    public void Play_Effect()
    {
        minimapEui.Play_Effect();
    }
    public void Gen_Minimap()
    {
        minimapEui.Gen_Minimap();
    }
    public void Reset_BookRoom()
    {
        minimapEui.Reset_BookRoom();
    }
}
