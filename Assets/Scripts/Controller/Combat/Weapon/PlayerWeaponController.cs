using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : WeaponController
{
    #region Value
    [Space(20)] [Header("<><><><><> Player")]

    [Header("=== Input")]
    [SerializeField] public bool IsInputed = false;

    [Header("=== Roll")]
    [SerializeField] private float DefualtRoll = -85f;
    [SerializeField] private Transform RollTF;

    [Header("=== Pitch")]
    [SerializeField] private float rotateSpeed = 4f;
    [SerializeField] private Transform PitchTF;

    [Header("=== Hand")]
    [SerializeField] private List<Satellite> Hands;


    #endregion

    #region Framework


    protected override void Update()
    {
        base.Update();

        RotateSmooth();

        foreach (Satellite hand in Hands)
        {
            hand.SetPosOffset();
        }

        if (IsInputed && CurrentDelayROF >= 1)
        {
            List<PlayerBulletController> PBClist = new List<PlayerBulletController>();
            foreach (Transform TF in BulletSpawnTFs)
            { PBClist.Add(PoolingManager.Instance.GetOP_PlayerBullet()); }
            Fire(PBClist);
        }
    }
    private void OnEnable()
    {
        RollTF.rotation = Quaternion.Euler(DefualtRoll, 0f, 0f);
    }

    #endregion

    #region Rotate

    private void RotateSmooth()
    {
        Vector2 dir = InputManager.Instance.DirFromPlayerPos.normalized;
        Quaternion targetQuat = Quaternion.Euler(0f, -Vector2.SignedAngle(Vector2.up, dir), 0f);
        targetQuat = Quaternion.Slerp(PitchTF.transform.localRotation, targetQuat, rotateSpeed * Time.deltaTime);

        PitchTF.transform.localRotation = targetQuat;
    }

    #endregion
}

[System.Serializable]
public class Satellite
{
    [SerializeField] public Transform ObjectTF;
    [SerializeField] public Transform TargetTF;

    public void SetPosOffset()
    {
        ObjectTF.position = TargetTF.position;
    }
}
