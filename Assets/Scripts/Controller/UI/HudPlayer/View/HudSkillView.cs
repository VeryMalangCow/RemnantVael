using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudSkillView : MonoBehaviour
{
    [Space(10)]
    [Header("=== Skill State")]
    [SerializeField] private RectTransform skillStatesParentRt;
    [SerializeField] private List<TMP_Text> skillStatesTxtList;
    [SerializeField] public List<SkillEUIController> skillList;

    private List<Image> skillImgList = new List<Image>();

    private float defaultSkillStatesRectY;
    private List<string> skillStatesStringList = new List<string>(2);

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private TMP_Text[] mainClrTmps;
    [SerializeField] private TMP_Text[] subClrTmps;
    [SerializeField] private Image[] mainClrImgs;

    private Tween tabTween = null;

    // Init
    public void Init(SkillWeaponController skillWeapon, Color mainClr, Color subClr)
    {
        for (int i = 0; i < skillList.Count; i++) 
            skillList[i].Offset();

        for (int i = 0; i < DevTool.skillAmount; i++)
        {
            skillImgList.Add(DevTool.Get_ComponentTType<Image>(skillList[i].gameObject));
            skillImgList[i].sprite = skillWeapon.skillList[i].icon;
        }

        defaultSkillStatesRectY = DevTool.Get_ComponentTType(
            skillStatesParentRt.gameObject, out RectTransform ss_Rt) ?
                ss_Rt.anchoredPosition.y : 0f;

        ColorInit(mainClr, subClr);

        gameObject.SetActive(true);
    }

    private void ColorInit(Color mainClr, Color subClr)
    {
        DevTool.SetColorTmps(mainClr, mainClrTmps);
        mainClrTmps = null;

        DevTool.SetColorTmps(subClr, subClrTmps);
        subClrTmps = null;
        DevTool.SetColorImgs(subClr, mainClrImgs);
        mainClrImgs = null;
    }

    public void ResetTab(SkillWeaponController skillWeapon)
    {
        
        for (int i = 0; i < DevTool.skillAmount; i++)
            skillStatesTxtList[i].text = Get_SkillStateTxt(skillWeapon.skillList[i]);
    }
    public void TabOn(float durTime)
    {
        DevTool.SetKillTween(tabTween);
        skillStatesParentRt.DOAnchorPosY(0f, durTime);
    }

    public void TabOff(float durTime)
    {
        DevTool.SetKillTween(tabTween);
        skillStatesParentRt.DOAnchorPosY(defaultSkillStatesRectY, durTime);
    }

    // Tab ½ºÅ³ ½ºÅÈ Txt
    private string Get_SkillStateTxt(ActiveSkillController skill)
    {
        string result = "";
        List<string> strings = Get_SkillStateStrings(skill);

        for (int i = 0; i < skillStatesStringList.Count; i++)
        {
            result += "<size=70%>" + skillStatesStringList[i] + ": </size>\n";
            result += "<b>" + strings[i] + "</b>\n";
        }
        return result;
    }
    private List<string> Get_SkillStateStrings(ActiveSkillController skill)
    {
        return new List<string>()
        {
            skill.tier.actualState.ToString(),
            skill.power.actualState.ToString()
        };
    }

    public void SetLanguage()
    {
        skillStatesStringList.Clear();
        for (int i = 17; i <= 18; i++)
            skillStatesStringList.Add(ResourceManager.instance.Get_StaticWord(i));
    }
}
