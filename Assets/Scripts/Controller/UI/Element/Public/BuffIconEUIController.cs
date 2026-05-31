using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffIconEUIController : ElementUIController, IPoolable
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff Icon")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] public bool usingNow = false;

    [SerializeField] private Image img;
    [SerializeField] public Image shadowImg;

    [SerializeField] private TMP_Text txt;

    [HideInInspector] public RectTransform rt;

    #endregion
    public int PoolIndex { get; set; } = -1;
    public int ActiveIndex { get; set; } = -1;


    #region Pool
    public void PoolOffset()
    {
        gameObject.SetActive(false);
    }

    public void SetActiveOn()
    {
        gameObject.SetActive(true);
    }

    public void SetActiveOff()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Offset

    public override void Offset()
    {
        rt = DevTool.Get_ComponentTType(gameObject, out RectTransform _rt) ? _rt : null;
    }

    #endregion

    #region Set

    public void SetOn(Sprite icon, bool showTxt)
    {
        usingNow = true;
        gameObject.SetActive(true);

        img.sprite = icon;
        txt.gameObject.SetActive(showTxt);
        shadowImg.fillAmount = 0;
    }

    public void SetOff()
    {
        usingNow = false;
        gameObject.SetActive(false);
    }


    public void Set_Icon(Sprite sprite, int buffAmount, int maxBuffAmount)
    {
        Set_Icon(sprite);
        Set_Icon(buffAmount, maxBuffAmount);
    }

    public void Set_Icon(Sprite sprite)
    {
        img.sprite = sprite;
    }

    public void Set_Icon(int buffAmount, int maxBuffAmount)
    {
        if (buffAmount > 0 && maxBuffAmount != 1)
        {
            txt.gameObject.SetActive(true);
            txt.text = buffAmount.ToString();
        }
        else
        {
            txt.gameObject.SetActive(false);
        }
    }


    public void Set_Cooltime(float fillAmount)
    {
        shadowImg.fillAmount = fillAmount;
    }


    #endregion
}
