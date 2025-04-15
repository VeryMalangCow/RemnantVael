using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllyCardEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private Image FrameImg;
    [SerializeField] private Image LightImg;
    [SerializeField] private TMP_Text RankTxt;
    [SerializeField] private TMP_Text NameTxt;

    #endregion

    #region Offset

    public override void Offset()
    {
        
    }

    #endregion

    #region Set

    public void Set_Card(int _ID)
    {

    }

    #endregion
}
