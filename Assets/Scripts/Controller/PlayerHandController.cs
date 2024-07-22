using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandController : MonoBehaviour
{
    #region Value

    [Header("=== Roll")]
    [SerializeField] private float DefualtRoll;
    [SerializeField] private Transform RollTF;

    [Header("=== Pitch")]
    [SerializeField] private Transform PitchTF;

    [Header("=== Hand")]
    [SerializeField] private Satellite RightHand;
    [SerializeField] private Satellite LeftHand;

    #endregion

    #region Framework

    private void OnEnable()
    {
        RollTF.rotation = Quaternion.Euler(DefualtRoll, 0f, 0f);
    }

    private void Update()
    {
        Vector2 dir = InputManager.Instance.DirFromPlayerPos.normalized;
        PitchTF.transform.localRotation = Quaternion.Euler(0, -Vector2.SignedAngle(Vector2.up, dir), 0f);

        RightHand.SetPos();
        LeftHand.SetPos();
    }

    #endregion

    #region 



    #endregion
}

[System.Serializable]
public class Satellite
{
    [SerializeField] public Transform ObjectTF;
    [SerializeField] public Transform TargetTF;

    public void SetPos()
    {
        ObjectTF.position = TargetTF.position;
    }
}
