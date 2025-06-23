using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffIconEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff Icon")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public bool UsingNow = false;

    [SerializeField] private Image ThisImg;
    [SerializeField] public Image ThisShadowImg;

    [SerializeField] private TMP_Text ThisTxt;

    [HideInInspector] public RectTransform ThisRT;

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisRT = DevTool.Get_ComponentTType(gameObject, out RectTransform rt) ? rt : null;
    }

    #endregion

    #region Set

    public void SetOn(Sprite _Icon, bool _ShowTxt)
    {
        UsingNow = true;
        gameObject.SetActive(true);

        ThisImg.sprite = _Icon;
        ThisTxt.gameObject.SetActive(_ShowTxt);
        ThisShadowImg.fillAmount = 0;
    }

    public void SetOff()
    {
        UsingNow = false;
        gameObject.SetActive(false);
    }


    public void Set_Icon(Sprite _Sprite, int _BuffAmount, int _MaxBuffAmount)
    {
        Set_Icon(_Sprite);
        Set_Icon(_BuffAmount, _MaxBuffAmount);
    }

    public void Set_Icon(Sprite _Sprite)
    {
        ThisImg.sprite = _Sprite;
    }

    public void Set_Icon(int _BuffAmount, int _MaxBuffAmount)
    {
        if (_BuffAmount > 0 && _MaxBuffAmount != 1)
        {
            ThisTxt.gameObject.SetActive(true);
            ThisTxt.text = _BuffAmount.ToString();
        }
        else
        {
            ThisTxt.gameObject.SetActive(false);
        }
    }


    public void Set_Cooltime(float _FillAmount)
    {
        ThisShadowImg.fillAmount = _FillAmount;
    }


    #endregion
}
