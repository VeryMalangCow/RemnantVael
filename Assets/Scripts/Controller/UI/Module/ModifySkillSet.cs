using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifySkillSet : UIModule
{
    #region Value

    [Space(10)]
    [Header("=== Img")]
    [SerializeField] private Image SkillShadowImg;
    [SerializeField] private Image SkillInnerImg;

    [Space(10)]
    [Header("=== Txt")]
    [SerializeField] private TMP_Text SkillCostTxt;
    [SerializeField] private TMP_Text SkillErrorTxt;
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

    public void SetCostText(float _Cost)
    {
        SkillCostTxt.text = "<size=60%>'</size>" + _Cost + "<size=60%>'</size>";
    }

    public void SetShadowFillAmount(float _FillAmount)
    {
        SkillShadowImg.fillAmount = _FillAmount;
    }

    public void SetStartUI()
    {
        if (DOTween.IsTweening(SkillInnerImg))
        { DOTween.Kill(SkillInnerImg); }
        SkillInnerImg.DOFade(1f, 0.2f);
    }

    public void SetEndUI()
    {
        if (DOTween.IsTweening(SkillInnerImg))
        { DOTween.Kill(SkillInnerImg); }
        SkillInnerImg.DOFade(0.25f, 0.2f);
    }

    public void StartNotEnoughEP()
    {
        if (ThisEff == null)
        {
            ThisEff = NotEnoughEP();
            StartCoroutine(ThisEff); 
        }
        else
        {
            StopCoroutine(ThisEff);
            StartCoroutine(ThisEff);
        }
    }

    private IEnumerator NotEnoughEP()
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
