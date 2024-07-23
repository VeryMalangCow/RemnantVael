using System.Collections.Generic;
using UnityEngine;

public class PlayerHandController : MonoBehaviour
{
    #region Value

    [Header("=== Roll")]
    [SerializeField] private float DefualtRoll;
    [SerializeField] private Transform RollTF;

    [Header("=== Pitch")]
    [SerializeField] private float rotateSpeed = 4f;
    [SerializeField] private Transform PitchTF;

    [Header("=== Hand")]
    [SerializeField] private List<Satellite> Hands;

    #endregion

    #region Framework

    private void OnEnable()
    {
        RollTF.rotation = Quaternion.Euler(DefualtRoll, 0f, 0f);
    }

    private void Update()
    {
        RotateSmooth();

        foreach (Satellite hand in Hands)
        {
            hand.SetPosOffset();
        }
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
