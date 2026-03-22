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

    [HideInInspector] private Color[] rollClrArr;

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
        switch (nscType)
        {
            case eNSCPuzzleType.Color:
                rollClrArr = ResourceManager.instance.nsc_colorArr;
                for (int i = 0; i < rollImgList.Count; i++)
                    rollImgList[i].sprite = ResourceManager.instance.nsc_colorSprite;
                break;

            default:
                break;
        }
    }

    #endregion

    #region Set

    public override void Set_ImgByIndex(int index)
    {
        for (int i = 0; i < rollImgList.Count; i++)
        {
            int targetIndex = (index + i) % rollImgList.Count;
            rollImgList[i].color = rollClrArr[targetIndex];
        }
    }


    #endregion
}
