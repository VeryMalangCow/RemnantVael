using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBuffIconUIController : MonoBehaviour
{
    #region Value

    public bool UsingNow = false;
    public Image IconImg;
    public Image ShadowImg;
    public TMP_Text AmountTxt;

    #endregion

    #region Offset

    public void Offset()
    {
        Off();
    }

    #endregion

    #region On / Off

    public void On()
    {
        UsingNow = true;
        gameObject.SetActive(true);
    }

    public void Off()
    {
        UsingNow = false;
        gameObject.SetActive(false);
    }

    #endregion
}
