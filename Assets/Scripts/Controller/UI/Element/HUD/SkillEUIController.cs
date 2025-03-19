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
    [SerializeField] private Image SkillShadowImg;
    [SerializeField] public Image SkillInnerImg;

    [Space(10)]
    [Header("=== Txt")]
    [SerializeField] public TMP_Text SkillCostTxt;
    [SerializeField] public TMP_Text SkillErrorTxt;
    private IEnumerator ThisEff = null;

    #endregion

    #region Offset

    public override void Offset()
    {
        SkillErrorTxt.gameObject.SetActive(false);
        SkillCostTxt.gameObject.SetActive(true);
    }

    #endregion

    #region Set

    public void Set_CostText(float _Cost)
    {
        SkillCostTxt.text = "<size=60%>'</size>" + _Cost + "<size=60%>'</size>";
    }

    public void Set_ShadowFillAmount(float _FillAmount)
    {
        SkillShadowImg.fillAmount = _FillAmount;
    }

    public void Set_StartUI()
    {
        if (DOTween.IsTweening(SkillInnerImg))
        { DOTween.Kill(SkillInnerImg); }
        SkillInnerImg.DOFade(1f, 0.2f);
    }

    public void Set_EndUI()
    {
        if (DOTween.IsTweening(SkillInnerImg))
        { DOTween.Kill(SkillInnerImg); }
        SkillInnerImg.DOFade(0.25f, 0.2f);
    }

    public void Play_ErrorUI()
    {
        if (ThisEff == null)
        {
            ThisEff = Play_NotEnoughEP_Cor();
            StartCoroutine(ThisEff); 
        }
        else
        {
            StopCoroutine(ThisEff);
            StartCoroutine(ThisEff);
        }
    }

    private IEnumerator Play_NotEnoughEP_Cor()
    {
        SkillErrorTxt.gameObject.SetActive(true);
        SkillCostTxt.gameObject.SetActive(false);

        Color clr = SkillErrorTxt.color;

        for (int i = 0; i < 3; i++)
        {
            clr.a = 1;
            SkillErrorTxt.color = clr;

            yield return new WaitForSeconds(0.09f);

            clr.a = 0;
            SkillErrorTxt.color = clr;

            if (i < 2)
            { yield return new WaitForSeconds(0.09f); }
        }

        SkillErrorTxt.gameObject.SetActive(false);
        SkillCostTxt.gameObject.SetActive(true);

        ThisEff = null;
    }

    #endregion
}
