using UnityEngine;

public class DirectionalAllyTypeImgController : DirectionalImgController
{
    #region Value 

    [Space(20)]
    [Header("<><><><><> Ally Type")]

    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] private string spriteSetName;
    [SerializeField] private AllySpriteSet allySpriteSet;

    #endregion

    #region Framework

    protected override void Start()
    {
        allySpriteSet = StaticResourceManager.instance.AllyReso.GetAllySpriteSet(spriteSetName);
        comp.material = allySpriteSet.material;
        Set_Type(AllyStateMode.Idle);

        base.Start();
    }

    #endregion

    #region Set

    public void Set_Type(AllyStateMode mode)
    {
        switch (mode)
        {
            case AllyStateMode.Idle:
                dirList = allySpriteSet.allyIdle;
                break;

            case AllyStateMode.Move:
                dirList = allySpriteSet.allyMove;
                break;

            case AllyStateMode.Attack:
                dirList = allySpriteSet.allyAttack;
                break;

            default:
                break;
        }

        comp.sprite = dirList[currentIndex.Value];
    }

    #endregion
}
