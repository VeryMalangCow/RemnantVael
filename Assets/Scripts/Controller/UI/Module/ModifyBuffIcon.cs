using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifyBuffIcon : UIModule
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
        if (this.TryGetComponent(out RectTransform rt) && ThisRT == null)
        {
            ThisRT = rt;
        }
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

    public void Set_Icon(Sprite _Sprite, int _BuffAmount)
    {
        Set_Icon(_Sprite);
        Set_Icon(_BuffAmount);
    }

    public void Set_Icon(Sprite _Sprite)
    {
        ThisImg.sprite = _Sprite;
    }

    public void Set_Icon(int _BuffAmount)
    {
        if (_BuffAmount > 1)
        {
            ThisTxt.gameObject.SetActive(true);
            ThisTxt.text = _BuffAmount.ToString();
        }
        else
        {
            ThisTxt.gameObject.SetActive(false);
        }
    }

    public void Set_BuffState(int _CurrentStack)
    {
        ThisTxt.text = _CurrentStack.ToString();
    }

    public void Set_BuffState(float _FillAmount)
    {
        ThisShadowImg.fillAmount = _FillAmount;
    }


    #endregion
}
