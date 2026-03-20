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
    [SerializeField] private float aimFollowSpeed = 30f;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] List<Transform> lineList;

    [HideInInspector] private float spreadMaxAngle;

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
        PlayerManager.instance.playerController.BaseWeapon.accRate.actualState
            .Subscribe(value =>
            {
                Set_AngleRoundValue(value);
            });
    }

    private void Offset_Sorting()
    {
        for (int i = 0; i < lineList.Count; i++)
            DevTool.Get_ComponentTType<SpriteRenderer>(lineList[i].transform.GetChild(0).gameObject).sortingOrder = LayerOrderManager.order_Aim;
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
            Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, InputManager.instance.dirFromPlayerPos)),
            aimFollowSpeed * Time.deltaTime);
    }

    private void Update_AimDis()
    {
        float dis = Vector2.Distance(Vector2.zero, InputManager.instance.dirFromPlayerPos);
        for (int i = 0; i < lineList.Count; i++)
        {
            lineList[i].GetChild(0).localPosition =
                Vector2.Lerp(lineList[i].GetChild(0).localPosition,
                new Vector2(0, dis),
                aimFollowSpeed * Time.deltaTime);
        }
    }

    #endregion

    #region SetAngle

    private void Set_AngleRoundValue(float value)
    {
        spreadMaxAngle = 100 - value;

        DevTool.Add_LocalRotZValue(lineList[0], spreadMaxAngle);
        DevTool.Add_LocalRotZValue(lineList[1], -spreadMaxAngle);
    }

    #endregion
}
