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

    public void On(Sprite _Icon, bool _ShowTxt)
    {
        UsingNow = true;
        gameObject.SetActive(true);

        ThisImg.sprite = _Icon;
        ThisTxt.gameObject.SetActive(_ShowTxt);
        ThisShadowImg.fillAmount = 0;
    }

    public void Off()
    {
        UsingNow = false;
        gameObject.SetActive(false);
    }

    public void SetIcon(Sprite _Sprite, int _BuffAmount)
    {
        SetIcon(_Sprite);
        SetIcon(_BuffAmount);
    }

    public void SetIcon(Sprite _Sprite)
    {
        ThisImg.sprite = _Sprite;
    }

    public void SetIcon(int _BuffAmount)
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

    public void SetBuffState(int _CurrentStack)
    {
        ThisTxt.text = _CurrentStack.ToString();
    }

    public void SetBuffState(float _FillAmount)
    {
        ThisShadowImg.fillAmount = _FillAmount;
    }


    #endregion
}
