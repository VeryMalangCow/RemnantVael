using UnityEngine;
using System.Collections.Generic;
using UniRx;

public class AimRoundController : StaticDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Aim Round")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float AimFollowSpeed = 30f;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] List<Transform> LineList;

    [HideInInspector] private float SpreadMaxAngle;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Offset_Subscribe();
        Offset_Sorting();
    }

    private void Offset_Subscribe()
    {
        PlayerManager.Instance.PlayerController.BaseWeapon.AccuracyRate.ActualState
            .Subscribe(value =>
            {
                Set_AngleRoundValue(value);
            });
    }

    private void Offset_Sorting()
    {
        for (int i = 0; i < LineList.Count; i++)
            DevTool.Get_ComponentTType<SpriteRenderer>(LineList[i].transform.GetChild(0).gameObject).sortingOrder = LayerOrderManager.Order_Aim;
    }

    #endregion

    #region Framework

    private void LateUpdate()
    {
        Update_AimDis();
        Update_AimRot();
    }

    #endregion

    #region Update

    private void Update_AimRot()
    {
        TargetObject.transform.localRotation = Quaternion.Slerp(
            TargetObject.transform.localRotation,
            Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, InputManager.Instance.DirFromPlayerPos)),
            AimFollowSpeed * Time.deltaTime);
    }

    private void Update_AimDis()
    {
        float dis = Vector2.Distance(Vector2.zero, InputManager.Instance.DirFromPlayerPos);
        for (int i = 0; i < LineList.Count; i++)
        {
            LineList[i].GetChild(0).localPosition =
                Vector2.Lerp(LineList[i].GetChild(0).localPosition,
                new Vector2(0, dis),
                AimFollowSpeed * Time.deltaTime);
        }
    }

    #endregion

    #region SetAngle

    private void Set_AngleRoundValue(float _Value)
    {
        SpreadMaxAngle = 100 - _Value;

        DevTool.Add_LocalRotZValue(LineList[0], SpreadMaxAngle);
        DevTool.Add_LocalRotZValue(LineList[1], -SpreadMaxAngle);
    }

    #endregion
}
