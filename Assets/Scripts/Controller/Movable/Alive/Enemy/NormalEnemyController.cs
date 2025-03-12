using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class NormalEnemyController : EnemyController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Normal Enemy")]

    [Space(10)]
    [Header("=== Img or Anim")]
    [SerializeField] private ReactiveProperty<int> CurrentIndex = new();
    [SerializeField] private SolarSystemController WalkingSatellite;
    [SerializeField] private SolarSystemController LookingSatellite;
    [SerializeField] private float BaseUnderFootAnimSpeed = 1.0f;
    [SerializeField] private List<DirectionalAnimController> ThisSEDA;

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        CurrentIndex.Value = 5;
    }

    protected override void Update()
    {
        base.Update();

        Set_ImgPosSort(WalkingSatellite); 
        Set_ImgPosSort(LookingSatellite);
    }

    private void LateUpdate()
    {
        Set_Img(WalkingSatellite, ThisRb.velocity, Time.deltaTime);
        Set_Img(LookingSatellite, LookAtDir, Time.deltaTime);
        DevTool.Set_AnimSpeed(ThisSEDA, BaseUnderFootAnimSpeed * ThisRb.velocity.sqrMagnitude);
    }

    #endregion

    #region Img or Anim

    private void Set_Img(SolarSystemController _SC, Vector2 _Dir, float _DeltaTime)
    {
        if (_Dir != Vector2.zero)
        {
            _Dir = new Vector2(-_Dir.x, _Dir.y);
            int index = DevTool.Get_Index(Quaternion.FromToRotation(Vector3.up, _Dir).eulerAngles.z);
            if (index != CurrentIndex.Value)
            {
                CurrentIndex.Value = index;
            }

            _SC.Set_RotSmooth(DevTool.Get_NormalizedVec(CurrentIndex.Value), _DeltaTime);
        }
    }

    private void Set_ImgPosSort(SolarSystemController _SC)
    {
        foreach (SatelliteSideController hand in _SC.SatelliteSideList)
        { hand.Set_SortingOrder(_SC.PivotObjectSR.sortingOrder); }
    }

    #endregion
}
