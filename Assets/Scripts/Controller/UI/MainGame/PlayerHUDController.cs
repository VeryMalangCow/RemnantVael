using UnityEngine;
using UniRx;
using TMPro;

public class PlayerHUDController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player HUD")]

    [Space(10)]
    [Header("=== Energy")]
    [SerializeField] private ModifyReductionFocusProgressBar EP;

    [Space(10)]
    [Header("=== Bettery")]
    [Header("-- Current")]
    [SerializeField] public ModifySprite CurrentEmptyBC;

    [Header("-- Bettery (Image Amount Class)")]
    [SerializeField] private ModifyImgAmountAndTxt EmptyBC;
    [SerializeField] private ModifyImgAmountAndTxt FullBC;

    [Space(10)]
    [Header("=== Boost")]
    [SerializeField] private TMP_Text BoostLv;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        Debug.Log("HUD 세팅");
        EP.Offset();
        CurrentEmptyBC.Offset();
        EmptyBC.Offset();
        FullBC.Offset();
    }

    protected override void Offset_UI()
    {
        Debug.Log("HUD 세팅");
        PlayerManager.Instance.PlayerController.MaxEP.ActualState
            .Subscribe(_MaxEP =>
            {
                EP.SetMaxFillRT(_MaxEP * 4);

                EP.SetFillImgSmooth(
                    PlayerManager.Instance.PlayerController.CurrentEP.Value,
                    PlayerManager.Instance.PlayerController.MaxEP.ActualState.Value);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentEP
            .Subscribe(_CurrentEP =>
            {
                EP.SetFillImgSmooth(
                    PlayerManager.Instance.PlayerController.CurrentEP.Value,
                    PlayerManager.Instance.PlayerController.MaxEP.ActualState.Value);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBS
            .Subscribe(_CurrentBS =>
            {
                if (PlayerManager.Instance.PlayerController.CurrentBS.Value < PlayerManager.Instance.PlayerController.NeedBS_ForMakeBC)
                {
                    CurrentEmptyBC.Modify_Sprite(_CurrentBS);
                }
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBC
            .Subscribe(_CurrentBC =>
            {
                EmptyBC.SetAmount(_CurrentBC, 0.5f);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentEC
            .Subscribe(_CurrentEC =>
            {
                FullBC.SetAmount(_CurrentEC, 0.5f);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBoostLv
            .Subscribe(_BoostLevel =>
            {
                BoostLv.text = _BoostLevel.ToString();
            })
            .AddTo(gameObject);
    }

    #endregion

}

