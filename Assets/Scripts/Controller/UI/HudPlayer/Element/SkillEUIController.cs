using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Img")]
    [SerializeField] private Image skillShadowImg;
    [SerializeField] public Image skillInnerImg;

    [Space(10)]
    [Header("=== Txt")]
    [SerializeField] public TMP_Text skillCostTxt;
    [SerializeField] public TMP_Text skillErrorTxt;

    [HideInInspector] private IEnumerator eff = null;

    #endregion

    #region Offset

    public override void Offset()
    {
        SetOn_CostTxt(true);
    }

    #endregion

    #region Set

    public void Set_CostText(float cost)
    {
        skillCostTxt.text = "<size=60%>'</size>" + cost + "<size=60%>'</size>";
    }

    public void Set_ShadowFillAmount(float fillAmount)
    {
        skillShadowImg.fillAmount = fillAmount;
    }

    private void SetOn_CostTxt(bool onOff)
    {
        skillCostTxt.gameObject.SetActive(onOff);
        skillErrorTxt.gameObject.SetActive(!onOff);
    }

    #endregion

    #region Tween

    public void Play_StartInnerUI()
    {
        DevTool.SetKillTween(skillInnerImg);

        skillInnerImg.DOFade(1f, 0.2f);
    }

    public void Play_EndInnerUI()
    {
        DevTool.SetKillTween(skillInnerImg);

        skillInnerImg.DOFade(0.25f, 0.2f);
    }

    public void Play_ErrorUI()
    {
        if (eff != null) return;

        eff = Play_Error_Cor();
        StartCoroutine(eff);
    }

    private IEnumerator Play_Error_Cor()
    {
        SetOn_CostTxt(false);

        Color clr = skillErrorTxt.color;

        for (int i = 0; i < 3; i++)
        {
            clr.a = 1;
            skillErrorTxt.color = clr;

            yield return new WaitForSeconds(0.09f);

            clr.a = 0;
            skillErrorTxt.color = clr;

            if (i < 2)
            { yield return new WaitForSeconds(0.09f); }
        }

        SetOn_CostTxt(true);

        eff = null;
    }

    #endregion
}
