using UnityEngine;
using UnityEngine.UI;

public class ModifyEachInventoryItem : UIModule
{
    #region Value 

    [Header("=== RT")]
    [SerializeField] private Vector2 ThisSizeDelta = new Vector2(90, 90);

    [Header("=== Inner Component")]
    [SerializeField] public Image RankImg;
    [SerializeField] private ModifyImgAmountAndTxt BoostLvMIAAT;

    // Component
    private RectTransform ThisRT;
    [HideInInspector] public int ThisRankLv;
    [HideInInspector] public Image ThisImg;

    #endregion

    #region Offset

    public void SetData(Sprite _ThisIcon, Sprite _RankImg, int _BoostLv)
    {
        ThisImg.sprite = _ThisIcon;
        RankImg.sprite = _RankImg;
        ThisRankLv = _BoostLv;
        BoostLvMIAAT.SetAmount(_BoostLv, 0.1f);
    }

    public override void Offset()
    {
        if(TryGetComponent(out RectTransform rt))
        { ThisRT = rt; }

        if (TryGetComponent(out Image img))
        { ThisImg = img; }

        ThisRT.sizeDelta = ThisSizeDelta;

        BoostLvMIAAT.Offset();
    }

    #endregion
}
