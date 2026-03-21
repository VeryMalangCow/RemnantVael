using UnityEngine;
using UnityEngine.Serialization;

public class DirectionalAllyTypeImgController : DirectionalImgController
{
    #region Value 

    [Space(20)]
    [Header("<><><><><> Ally Type")]

    [Space(10)]
    [Header("=== Sprite")]
    [FormerlySerializedAs("ThisSpriteSetName")][SerializeField] private string spriteSetName;
    [FormerlySerializedAs("AllySpriteSet")][SerializeField] private AllySpriteSet allySpriteSet;

    #endregion

    #region Framework

    protected override void Start()
    {
        allySpriteSet = AllyManager.instance.allySpriteSetDict[spriteSetName];
        Set_Type(eAllyStateMode.Idle);

        base.Start();
    }

    #endregion

    #region Set

    public void Set_Type(eAllyStateMode mode)
    {
        switch (mode)
        {
            case eAllyStateMode.Idle:
                dirList = allySpriteSet.allyIdle;
                break;

            case eAllyStateMode.Move:
                dirList = allySpriteSet.allyMove;
                break;

            case eAllyStateMode.Attack:
                dirList = allySpriteSet.allyAttack;
                break;

            default:
                break;
        }

        comp.sprite = dirList[currentIndex.Value];
    }

    #endregion
}
