using UnityEngine;
using UniRx;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

public class AimRoundController : StaticDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Aim Round")]
    [SerializeField] private float AimFollowSpeed = 22f;
    [SerializeField] List<Transform> LineList;
    [HideInInspector] private float SpreadMaxAngle;

    #endregion

    #region Framework

    private void Start()
    {
        PlayerManager.Instance.PlayerController.BaseWeapon.AccuracyRate.ActualState
            .Subscribe(value =>
            {
                Set_AngleRound(value);
            });
    }

    private void LateUpdate()
    {
        Set_Distance();
        Set_Rotation();
    }

    #endregion

    #region SetAngle

    private void Set_AngleRound(float _Value)
    {
        SpreadMaxAngle = 100 - _Value;

        SetEachAngleRound(LineList[0], SpreadMaxAngle);
        SetEachAngleRound(LineList[1], -SpreadMaxAngle);

        void SetEachAngleRound(Transform _TF, float _Value)
        {
            Vector3 currentRotation = _TF.localEulerAngles;
            currentRotation.z = _Value;
            _TF.localEulerAngles = currentRotation;
        }
    }

    private void Set_Rotation()
    {
        TargetObject.transform.localRotation =
                    Quaternion.Slerp(TargetObject.transform.localRotation,
                    Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, InputManager.Instance.DirFromPlayerPos)),
                    AimFollowSpeed * Time.deltaTime);
    }

    private void Set_Distance()
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
}
