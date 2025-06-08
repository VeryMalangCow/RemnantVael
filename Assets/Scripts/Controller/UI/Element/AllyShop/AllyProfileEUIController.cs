using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyProfileEUIController : ElementUIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Profile")]
    [SerializeField] private Image FaceImg;
    [SerializeField] private TMP_Text NameTxt;


    [HideInInspector] private RectTransform ThisRT;

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisRT = DevTool.Get_ComponentTType(gameObject, out RectTransform rt) ? rt : null;
    }

    #endregion

    #region Set

    public void Set_Profile(Sprite _FaceSprite, string _Name, float _YPos)
    {
        FaceImg.sprite = _FaceSprite;
        NameTxt.text = _Name;
        ThisRT.anchoredPosition = new Vector2(0, _YPos);
    }

    #endregion

}
