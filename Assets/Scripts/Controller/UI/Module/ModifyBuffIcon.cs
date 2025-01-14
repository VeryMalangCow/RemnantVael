using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

public class ModifyBuffIcon : UIModule
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff Icon")]

    [Space(10)]
    [Header("=== Component")]
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

    #endregion
}
