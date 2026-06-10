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
    [HideInInspector] private Sprite[] rollSpriteList;

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
            case eNSCPuzzleType.Shape:
                rollSpriteList = ResourceManager.instance.nsc_shapeSpriteArr;
                break;

            case eNSCPuzzleType.Num:
                rollSpriteList = ResourceManager.instance.nsc_numSpriteArr;
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
            rollImgList[i].sprite = rollSpriteList[targetIndex];
        }
    }

    #endregion
}
