using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;

public class PlayerHUDController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player HUD")]

    [Header("=== Energy")]

    [SerializeField] private ModifyReductionFocusProgressBar EP;

    [Header("=== Bettery")]

    [Header("-- Current")]
    [SerializeField] public ModifySprite CurrentEmptyBC;

    [Header("-- Bettery (Image Amount Class)")]
    [SerializeField] private ModifyImgAmountAndTxt EmptyBC;
    [SerializeField] private ModifyImgAmountAndTxt FullBC;

    #endregion

    #region Offset
    private void Offset_Value()
    {
        CurrentEmptyBC.Offset();
        EmptyBC.Offset();
        FullBC.Offset();
    }

    private void Offset_UI()
    {
        PlayerManager.Instance.PlayerController.CurrentEP
            .Subscribe(_CurrentEP =>
            {
                EP.SetFillImgSmooth(
                    PlayerManager.Instance.PlayerController.CurrentEP.Value,
                    PlayerManager.Instance.PlayerController.MaxEP.Value);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBS
            .Subscribe(_CurrentBS =>
            {
                CurrentEmptyBC.Modify_Sprite(_CurrentBS);
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
    }

    #endregion

    #region Framework

    private void Start()
    {
        Offset_Value();
        Offset_UI();
    }

    #endregion
}

