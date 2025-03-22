using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutMainGameUIController : PanelUIController
{
    #region Value
    [Space(20)]
    [Header("<><><><><> Out Main Game UI")]

    [Space(10)]
    [Header("=== Inner")]
    [SerializeField] private Transform InnerParentTF;
    [SerializeField] private Image ResumeInnerImg;
    [SerializeField] private Image OptionInnerImg;
    [SerializeField] private Image QuitInnerImg;

    [Space(10)]
    [Header("=== Btns")]
    [SerializeField] private OwnBtnEUIController ResumeBtn;
    [SerializeField] private OwnBtnEUIController OptionBtn;
    [SerializeField] private OwnBtnEUIController QuitBtn;


    [HideInInspector] private List<Image> InnerImgs;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_Btn();
        Offset_ColorComp();
    }

    private void Offset_Btn()
    {
        ResumeBtn.Offset();
        ResumeBtn.OwnerUIController = this;
        OptionBtn.Offset();
        OptionBtn.OwnerUIController = this;
        QuitBtn.Offset();
        QuitBtn.OwnerUIController = this;

        InnerImgs = DevTool.Get_ChildList<Image>(InnerParentTF);
    }

    private void Offset_ColorComp()
    {
        MainColorCompList.AddRange(Get_AllTabBtn_Txt());
        SubColorCompList.AddRange(Get_AllTabBtn_Img());

        SubColorCompList.AddRange(InnerImgs);

        Color clr = new Color(1, 1, 1, 0.1f);
        ResumeInnerImg.color = clr;
        OptionInnerImg.color = clr;
        QuitInnerImg.color = clr;

        MainColorCompList.Add(ResumeInnerImg);
        MainColorCompList.Add(OptionInnerImg);
        MainColorCompList.Add(QuitInnerImg);

        Color mainClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, false);
        DevTool.Set_Color(mainClr, MainColorCompList);
        MainColorCompList.Clear();
        MainColorCompList = null;

        Color subClr = PlayerManager.Instance.PlayerController.Get_CorrectColor(eDamageType.Energy, true);
        DevTool.Set_Color(subClr, SubColorCompList);
        SubColorCompList.Clear();
        SubColorCompList = null;
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {
        // Base Btns
        if (CurrentBtn == ResumeBtn)
        {
            SetOff_ThisPanel();
        }
        else if (CurrentBtn == OptionBtn)
        {

        }
        else if (CurrentBtn == QuitBtn)
        {
            LoadingSceneManager.Instance.Play_LoadScene("TitleLobby");
        }
    }

    #endregion
}
