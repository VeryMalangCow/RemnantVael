using System.Collections.Generic;
using UnityEngine;

public class SolarSystemController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Solar System")]

    [Space(10)]
    [Header("=== Pivot")]
    [SerializeField] public SpriteRenderer pivotObjSr;

    [Space(10)]
    [Header("=== Satellite")]
    [Header("-- Roll")]
    [SerializeField] private float defualtRoll = -85f;
    [HideInInspector] private Transform rollTf;

    [Header("-- Pitch")]
    [SerializeField] protected float rotSpeed = 8f;
    [HideInInspector] public Transform pitchTf;

    [Header("-- Satellite")]
    [SerializeField] public List<SatelliteSideController> satelliteSideList;
    [SerializeField] public List<SatelliteCenterController> satelliteCenterList;

    #endregion

    #region Offset

    protected virtual void Offset()
    {
        Offset_TF();
    }

    private void Offset_TF()
    {
        rollTf = transform.GetChild(0);
        pitchTf = rollTf.GetChild(0);
    }

    #endregion

    #region Fremework

    private void Awake()
    {
        Offset();
    }

    protected void OnEnable()
    {
        rollTf.rotation = Quaternion.Euler(defualtRoll, 0f, 0f);
    }

    protected virtual void LateUpdate()
    {
        Set_Side();
        Set_Center();
    }

    #endregion

    #region SortingOrder

    // 솔팅
    private void Set_Side()
    {
        if (satelliteSideList != null && satelliteSideList.Count > 0)
        {
            for (int i = 0; i < satelliteSideList.Count; i++)
            {
                satelliteSideList[i].Set_Pos();
                satelliteSideList[i].Set_SortingOrder();
            }
        }
    }

    private void Set_Center()
    {
        if (satelliteCenterList != null && satelliteCenterList.Count > 0)
        {
            for (int i = 0; i < satelliteCenterList.Count; i++)
            {
                satelliteCenterList[i].Set_Pos();
                satelliteCenterList[i].Set_SortingOrder();
            }
        }
    }

    #endregion

    #region Rotate

    // 바로 Rot 설정
    public void Set_Rot(Vector2 dir)
    {
        pitchTf.transform.localRotation = DevTool.Get_RotFromDir_Solar(dir);
    }

    // 부드럽게 Rot 설정
    public void Set_RotSmooth(Vector2 dir, float deltaTime)
    {
        if (dir != Vector2.zero)
        {
            pitchTf.transform.localRotation = Quaternion.Slerp(
                pitchTf.transform.localRotation,
                DevTool.Get_RotFromDir_Solar(dir), 
                rotSpeed * deltaTime);
        }
    }

    #endregion
}