using UnityEngine;
using UniRx;
using System;

public class DirectionalAllyTypeImgController : DirectionalImgController
{
    #region Value 

    [Space(20)]
    [Header("<><><><><> Ally Type")]

    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] private string ThisSpriteSetName;
    [SerializeField] private AllySpriteSet AllySpriteSet;

    #endregion

    #region Framework

    protected override void Start()
    {
        AllySpriteSet = AllyManager.instance.allySpriteSetDict[ThisSpriteSetName];
        Set_Type(eAllyStateMode.Idle);

        base.Start();
    }

    #endregion

    #region Set

    public void Set_Type(eAllyStateMode _Mode)
    {
        switch (_Mode)
        {
            case eAllyStateMode.Idle:
                ThisDirectionalList = AllySpriteSet.AllyIdle;
                break;

            case eAllyStateMode.Move:
                ThisDirectionalList = AllySpriteSet.AllyMove;
                break;

            case eAllyStateMode.Attack:
                ThisDirectionalList = AllySpriteSet.AllyAttack;
                break;

            default:
                break;
        }

        ThisComp.sprite = ThisDirectionalList[CurrentIndex.Value];
    }

    #endregion
}
