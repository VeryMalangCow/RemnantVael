using System.Collections.Generic;
using UnityEngine;

public class SolarSystemController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Solar System")]

    [Space(10)]
    [Header("=== Pivot")]
    [SerializeField] public SpriteRenderer PivotObjectSR;

    [Space(10)]
    [Header("=== Satellite")]
    [Header("-- Roll")]
    [SerializeField] private float DefualtRoll = -85f;
    [HideInInspector] private Transform RollTF;

    [Header("-- Pitch")]
    [SerializeField] protected float rotateSpeed = 4f;
    [HideInInspector] public Transform PitchTF;

    [Header("-- Hand")]
    [SerializeField] public List<SatelliteSideController> SatelliteSideList;
    [SerializeField] public List<SatelliteCenterController> SatelliteCenterList;

    #endregion

    #region Offset

    private void Offset()
    {
        Offset_TF();
    }

    private void Offset_TF()
    {
        RollTF = transform.GetChild(0);
        PitchTF = RollTF.GetChild(0);
    }

    #endregion

    #region Fremework

    private void Awake()
    {
        Offset();
    }

    protected void OnEnable()
    {
        RollTF.rotation = Quaternion.Euler(DefualtRoll, 0f, 0f);
    }

    protected virtual void LateUpdate()
    {
        Set_SortingOrderAll();
    }

    #endregion

    #region SortingOrder

    // 솔팅
    private void Set_SortingOrderAll()
    {
        for (int i = 0; i < SatelliteSideList.Count; i++)
        {
            SatelliteSideList[i].Set_SortingOrder(PivotObjectSR.sortingOrder);
        }
        for (int i = 0; i < SatelliteCenterList.Count; i++)
        {
            SatelliteCenterList[i].Set_SortingOrder(PivotObjectSR.sortingOrder);
        }
    }

    #endregion

    #region Rotate

    // 바로 Rot 설정
    public void Set_Rot(Vector2 _Dir)
    {
        PitchTF.transform.localRotation = Quaternion.Euler(0f, -Vector2.SignedAngle(Vector2.up, _Dir), 0f);
    }

    // 부드럽게 Rot 설정
    public void Set_RotSmooth(Vector2 _Dir, float _DeltaTime)
    {
        PitchTF.transform.localRotation = Quaternion.Slerp(
            PitchTF.transform.localRotation, 
            Quaternion.Euler(0f, -Vector2.SignedAngle(Vector2.up, _Dir), 0f), 
            rotateSpeed * Time.deltaTime);
    }

    #endregion
}