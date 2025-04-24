using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NSCRollColorCellEUIController : NSCRollCellEUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Color")]

    [Space(10)]
    [Header("=== Color")]


    #endregion

    #region - Hide

    [HideInInspector] private List<Color> RollColorList;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        Offset_Sprite();
    }

    private void Offset_Sprite()
    {
        switch (ThisNSCType)
        {
            case eNSCPuzzleType.Color:
                RollColorList = UnitManager.Instance.NSC_ColorList;
                for (int i = 0; i < RollImgList.Count; i++)
                    RollImgList[i].sprite = UnitManager.Instance.NSC_ColorSprite;
                break;

            default:
                break;
        }
    }

    #endregion

    #region Set

    public override void Set_ImgByIndex(int _Index)
    {
        for (int i = 0; i < RollImgList.Count; i++)
        {
            int targetIndex = (_Index + i) % RollImgList.Count;
            RollImgList[i].color = RollColorList[targetIndex];
        }
    }


    #endregion
}
