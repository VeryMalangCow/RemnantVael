using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class PlayerHUDController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player HUD")]
    [Header("=== Energy")]
    [SerializeField] private Image AfterImageEP_Img;
    [SerializeField] private Image ActualEP_Img;


    #endregion

    #region Framework

    private void Start()
    {
        PlayerManager.Instance.PlayerController.CurrentEP
            .Subscribe(_CurrentEP =>
            {
                UIManager.Instance.PlayerHUDController.SetFillImgSmooth_EP();
            })
            .AddTo(gameObject);
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
