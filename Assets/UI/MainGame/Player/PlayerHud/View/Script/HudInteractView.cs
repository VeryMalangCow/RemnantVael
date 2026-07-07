using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudInteractView : MonoBehaviour
{

    [Space(10)]
    [Header("=== Interact")]
    [SerializeField] private TMP_Text interactOnOffTxt;
    [SerializeField] private TMP_Text interactDescTxt;

    [SerializeField] private Image innerImg;
    [SerializeField] private Image usingInnerImg;
    [SerializeField] private Color uninteractableClr;
    [SerializeField] private Color interactableClr;

    // Interact
    private bool isActingInteractUi = false;
    private static string interactEnableString;
    private static string interactDisableString;
    private static string interacInoperableString;
    private static string interactNoneString;

    [Space(10)]
    [Header("=== Visual")]
    [SerializeField] private TMP_Text[] mainClrTmps;
    [SerializeField] private Image[] subClrImgs;

    private Tween onOffTxtTween;
    private Tween descTxtTween;

    // Init
    public void Init(Color mainClr, Color subClr)
    {
        ColorInit(mainClr, subClr);

        gameObject.SetActive(true);
    }

    private void ColorInit(Color mainClr, Color subClr)
    {
        DevTool.SetColorTmps(mainClr, mainClrTmps);
        mainClrTmps = null;

        DevTool.SetColorImgs(subClr, subClrImgs);
        subClrImgs = null;
    }

    // 상호작용 시 발생
    public void UseInteractUI(IInteract interactable)
    {
        isActingInteractUi = true;

        DevTool.SetKillTween(usingInnerImg);

        usingInnerImg.DOFade(1f, 0.2f)
            .OnComplete(() =>
            {
                usingInnerImg.DOFade(0.25f, 0.2f)
                .OnComplete(() =>
                {
                    isActingInteractUi = false;
                    SetInteractableUI(interactable);
                });
            });
    }

    public void SetInteractableUI(IInteract interactable)
    {
        if (isActingInteractUi) return;

        string txt = DevTool.Get_InteractingAnnoTxt(interactable, out bool canInteract);

        if (interactable != null && txt != "")
        {
            if (canInteract)
            {
                SetTxt(interactEnableString, txt, interactableClr);
                SetFade(1f, 0.5f);
            }
            else
            {
                SetTxt(interacInoperableString, txt, uninteractableClr);
                SetFade(1f, 0.5f);
            }
        }
        else
        {
            SetTxt(interactDisableString, interactNoneString, new Color(1, 1, 1, interactOnOffTxt.color.a));
            SetFade(0.25f, 0.5f);
        }
    }


    private void SetTxt(string onOffTxt, string interactableTxt, Color onOffTxtColor)
    {
        interactOnOffTxt.text = $"-{onOffTxt}-";
        interactOnOffTxt.color = onOffTxtColor;
        interactDescTxt.text = $"< {interactableTxt} >";
    }

    private void SetFade(float alpha, float durTime)
    {
        DevTool.SetKillTween(onOffTxtTween);
        DevTool.SetKillTween(descTxtTween);

        onOffTxtTween = interactOnOffTxt.DOFade(alpha, durTime);
        descTxtTween = interactDescTxt.DOFade(alpha, durTime);
    }

    public void SetLanguage()
    {
        var words = StaticResourceManager.instance.staticWords;
        interactEnableString = words.GetLanguage(4);
        interactDisableString = words.GetLanguage(5);
        interacInoperableString = words.GetLanguage(6);
        interactNoneString = words.GetLanguage(7);
        SetInteractableUI(null);
    }
}
