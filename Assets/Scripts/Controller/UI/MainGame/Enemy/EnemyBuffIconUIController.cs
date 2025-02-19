using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBuffIconUIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff Icon")]
    [SerializeField] public bool UsingNow = false;
    [SerializeField] public Image IconImg;
    [SerializeField] public Image ShadowImg;
    [SerializeField] public TMP_Text AmountTxt;

    [HideInInspector] public RectTransform ThisRT;
    #endregion

    #region Offset

    public void Offset()
    {
        if (ThisRT == null && TryGetComponent(out RectTransform rt))
        { ThisRT = rt; }

        Off();
    }

    #endregion

    #region On / Off

    public void On(Sprite _Icon, bool _ShowTxt)
    {
        UsingNow = true;
        gameObject.SetActive(true);

        IconImg.sprite = _Icon;
        AmountTxt.gameObject.SetActive(_ShowTxt);
        ShadowImg.fillAmount = 0;
    }

    public void Off()
    {
        UsingNow = false;
        gameObject.SetActive(false);
    }

    #endregion

    #region Update

    public void SetBuffState( int _CurrentStack)
    {
        AmountTxt.text = _CurrentStack.ToString();
    }

    public void SetBuffState(float _FillAmount)
    {
        ShadowImg.fillAmount = _FillAmount;
    }



    #endregion
}
