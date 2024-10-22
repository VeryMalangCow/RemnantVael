using System.Collections.Generic;
using UnityEngine;

public class SatelliteController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Waepon")]

    [Space(10)]
    [Header("=== Player")]
    [SerializeField] protected PlayerController PlayerController;
    [SerializeField] protected SpriteRenderer PlayerSR;
    [SerializeField] public bool IsInputed = false;

    [Space(10)]
    [Header("=== Hand Things")]
    [Header("-- Roll")]
    [SerializeField] private float DefualtRoll = -85f;
    [SerializeField] private Transform RollTF;

    [Header("-- Pitch")]
    [SerializeField] protected float rotateSpeed = 4f;
    [SerializeField] public Transform PitchTF;

    [Header("-- Hand")]
    [SerializeField] public List<Satellite> Hands;

    #endregion

    #region Fremework

    protected void OnEnable()
    {
        RollTF.rotation = Quaternion.Euler(DefualtRoll, 0f, 0f);
    }

    #endregion

    #region Rotate

    protected Quaternion RotateSmooth(Vector2 _Dir, Transform _PitchTF, float _RotateSpeed)
    {
        Quaternion targetQuat = Quaternion.Euler(0f, -Vector2.SignedAngle(Vector2.up, _Dir), 0f);
        targetQuat = Quaternion.Slerp(_PitchTF.transform.localRotation, targetQuat, _RotateSpeed * Time.deltaTime);

        return targetQuat;
    }

    #endregion
}

[System.Serializable]
public class Satellite
{
    [SerializeField] public Transform ObjectTF;
    [SerializeField] public Transform TargetTF;
    [SerializeField] public SpriteRenderer ThisActualSR;
    [SerializeField] public int UpperOrder;
    [SerializeField] public int FarFromCenter;

    public void SetPos(int _PlayerSortOrder)
    {
        ObjectTF.position = TargetTF.position;

        if (ObjectTF.localPosition.y > 0)
        {
            ThisActualSR.sortingOrder = _PlayerSortOrder + UpperOrder - FarFromCenter;
        }
        else
        {
            ThisActualSR.sortingOrder = _PlayerSortOrder + UpperOrder + FarFromCenter;
        }
    }
}
