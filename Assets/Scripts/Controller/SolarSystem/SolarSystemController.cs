using System.Collections.Generic;
using UnityEngine;

public class SolarSystemController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Waepon")]

    [Space(10)]
    [Header("=== Player")]
    [SerializeField] protected PlayerController PlayerController;
    [SerializeField] public SpriteRenderer PlayerSR;
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
    [SerializeField] public List<SatelliteController> Hands;

    #endregion

    #region Fremework

    protected void OnEnable()
    {
        RollTF.rotation = Quaternion.Euler(DefualtRoll, 0f, 0f);
    }

    #endregion

    #region Rotate

    public void Set_Rotation(Vector2 _Dir)
    {
        Set_Rotation(PitchTF, _Dir);
    }

    public void Set_Rotation(Transform _PitchTF, Vector2 _Dir)
    {
        _PitchTF.transform.localRotation = Quaternion.Euler(0f, -Vector2.SignedAngle(Vector2.up, _Dir), 0f);
        foreach (SatelliteController hand in Hands)
        { hand.SetPos(PlayerSR.sortingOrder); }
    }

    public Quaternion Get_RotationSmooth(Vector2 _Dir)
    {
        return Get_RotationSmooth(_Dir, PitchTF, rotateSpeed);
    }

    protected Quaternion Get_RotationSmooth(Vector2 _Dir, Transform _PitchTF, float _RotateSpeed)
    {
        Quaternion targetQuat = Quaternion.Euler(0f, -Vector2.SignedAngle(Vector2.up, _Dir), 0f);
        targetQuat = Quaternion.Slerp(_PitchTF.transform.localRotation, targetQuat, _RotateSpeed * Time.deltaTime);

        return targetQuat;
    }

    #endregion

    #region Get

    public static int Get_Index(float _EulerAngleY)
    {
        int index = 0;
        float angle = _EulerAngleY + 67.5f;
        angle = angle >= 360 ? angle -= 360 : angle;

        index = (int)(angle / 45);
        return index;
    }

    public static Vector2Int Get_NormalizedVec(int _Index)
    {
        switch (_Index)
        {
            case 0:
                return new Vector2Int(-1, 1);
            case 1:
                return new Vector2Int(0, 1);
            case 2:
                return new Vector2Int(1, 1);
            case 3:
                return new Vector2Int(1, 0);
            case 4:
                return new Vector2Int(1, -1);
            case 5:
                return new Vector2Int(0, -1);
            case 6:
                return new Vector2Int(-1, -1);
            case 7:
                return new Vector2Int(-1, 0);

        }
        return Vector2Int.zero;
    }

    #endregion
}