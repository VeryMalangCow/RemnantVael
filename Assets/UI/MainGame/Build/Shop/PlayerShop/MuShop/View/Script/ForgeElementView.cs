using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ForgeElementView : MonoBehaviour
{
    [Header("=== Visual")]
    [SerializeField] private TMP_Text[] mainTmps;

    [Space(10)]
    public RectTransform panelRT;
    public OwnBtnEUIController panelBtn;
    public TMP_Text panelBtnTxt;

    [HideInInspector] public CanvasGroup PanelBtnCG;

    [Space(10)]
    public OwnBtnEUIController roleBtn;
    public TMP_Text roleBtnTxt;
    public TMP_Text roleDescTxt;

    [HideInInspector] public RectTransform roleBtnTxtRT;
    [HideInInspector] public Tween rtTween = null;

    [Space(10)]
    public List<Image> innerImgs;

    public virtual void Offset(ModuleUpgradeUIController ui, OwnBtnEUIController _panelBtn)
    {
        panelBtn = _panelBtn;
        panelBtn.Offset();
        panelBtn.ownerUIController = ui;
        panelBtnTxt = panelBtn.gameObject.transform.GetChild(0).TryGetComponent(out TMP_Text tmp) ? tmp : null;

        roleBtn.Offset();
        roleBtn.ownerUIController = ui;

        PanelBtnCG = DevTool.Get_ComponentTType<CanvasGroup>(panelBtn.gameObject);

        roleBtnTxtRT = DevTool.Get_ComponentTType<RectTransform>(roleBtnTxt.gameObject);

        rtTween = roleBtnTxtRT.DOScale(1.15f, 1.0f)
                .OnPlay(() => { roleBtnTxtRT.localScale = Vector2.one; })
                .OnKill(() => { roleBtnTxtRT.localScale = Vector2.one; })
                .SetLoops(-1, LoopType.Yoyo);

        DOTween.Play(rtTween);
    }
    public void Set_LanguageTxt(string btnName, string btnDesc)
    {
        panelBtnTxt.text = btnName;
        roleBtnTxt.text = ">>  " + btnName + "  <<";
        roleDescTxt.text = btnDesc;
    }

    public abstract void ResetPanel();
    public void SetColor(Color mainClr)
    {
        DevTool.SetColorTmps(mainClr, mainTmps);
    }
}
