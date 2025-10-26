using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NSCRollImgCellEUIController : NSCRollCellEUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Img")]

    [Space(10)]
    [Header("=== Lock")]

    #endregion

    #region - Hide


    // Comp
    [HideInInspector] private Sprite[] RollSpriteList;

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
            case eNSCPuzzleType.Shape:
                RollSpriteList = ResourceManager.Instance.NSC_ShapeSpriteArr;
                break;

            case eNSCPuzzleType.Num:
                RollSpriteList = ResourceManager.Instance.NSC_NumSpriteArr;
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
            RollImgList[i].sprite = RollSpriteList[targetIndex];
        }
    }

    #endregion
}
