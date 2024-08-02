using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;

public class PlayerHUDController : UIController
{
    #region Value

    [Space(20)] [Header("<><><><><> Player HUD")]

    [Header("=== Energy")]
    [SerializeField] private Image AfterImageEP_Img;
    [SerializeField] private Image ActualEP_Img;

    [Header("=== Bettery")]

    [Header("-- Current")]
    [SerializeField] public Modify_Sprite CurrentEmptyBC;

    [Header("-- Bettery (Image Amount Class)")]
    [SerializeField] private Modify_ImgAmountAndTxt EmptyBC;
    [SerializeField] private Modify_ImgAmountAndTxt FullBC;

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
                UIManager.Instance.PlayerHUDController.SetFillImgSmooth_EP();
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBS
            .Subscribe(_CurrentBS =>
            {
                CurrentEmptyBC.ModifySprite(_CurrentBS);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBC
            .Subscribe(_CurrentBC =>
            {
                EmptyBC.SetAmount(_CurrentBC, 0.5f);
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

    #region Energy

    public void SetFillImgSmooth_EP()
    {
        base.SetFillImgSmooth(
            ActualEP_Img, 
            PlayerManager.Instance.PlayerController.CurrentEP.Value, 
            PlayerManager.Instance.LifeState.MaxEP);
    }

    #endregion
}

