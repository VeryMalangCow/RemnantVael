using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Waepon")]

    [Space(10)]
    [Header("=== Player")]
    [SerializeField] protected PlayerController PlayerController;
    [SerializeField] private SpriteRenderer PlayerSR;
    [SerializeField] public bool IsInputed = false;

    [Space(10)]
    [Header("=== Hand Things")]
    [Header("-- Roll")]
    [SerializeField] private float DefualtRoll = -85f;
    [SerializeField] private Transform RollTF;

    [Header("-- Pitch")]
    [SerializeField] private float rotateSpeed = 4f;
    [SerializeField] private Transform PitchTF;

    [Header("-- Hand")]
    [SerializeField] protected List<Satellite> Hands;
    [SerializeField] int FarFromCenter;

    #endregion

    #region Fremework

    protected virtual void Update()
    {
        PitchTF.transform.localRotation = RotateSmooth(PitchTF, rotateSpeed);


        foreach (Satellite hand in Hands)
        {
            hand.SetPosOffset();
            hand.SetSortOrder(PlayerSR.sortingOrder, FarFromCenter);
        }
    }

    private void OnEnable()
    {
        RollTF.rotation = Quaternion.Euler(DefualtRoll, 0f, 0f);
    }

    #endregion

    #region Rotate

    protected Quaternion RotateSmooth(Transform _PitchTF, float _RotateSpeed)
    {
        Vector2 dir = InputManager.Instance.DirFromPlayerPos.normalized;
        Quaternion targetQuat = Quaternion.Euler(0f, -Vector2.SignedAngle(Vector2.up, dir), 0f);
        targetQuat = Quaternion.Slerp(_PitchTF.transform.localRotation, targetQuat, _RotateSpeed * Time.deltaTime);

        return targetQuat;
    }

    #endregion
}
