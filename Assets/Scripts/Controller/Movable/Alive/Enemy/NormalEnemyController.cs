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
        Set_Img(WalkingSatellite, ThisRb.velocity);
        Set_Img(LookingSatellite, LookAtDir);
        DevTool.Set_AnimSpeedAnd(ThisSEDA, BaseUnderFootAnimSpeed * ThisRb.velocity.sqrMagnitude);
    }

    #endregion

    #region Img or Anim

    private void Set_Img(SolarSystemController _SC, Vector2 _Dir)
    {
        if (_Dir != Vector2.zero)
        {
            _Dir = new Vector2(-_Dir.x, _Dir.y);
            if (SolarSystemController.Get_Index(Quaternion.FromToRotation(Vector3.up, _Dir).eulerAngles.z) != CurrentIndex.Value)
            {
                CurrentIndex.Value = SolarSystemController.Get_Index(Quaternion.FromToRotation(Vector3.up, _Dir).eulerAngles.z);
            }

            _SC.PitchTF.transform.localRotation = _SC.Get_RotationSmooth(SolarSystemController.Get_NormalizedVec(CurrentIndex.Value));
        }
    }

    private void Set_ImgPosSort(SolarSystemController _SC)
    {
        foreach (SatelliteController hand in _SC.Hands)
        { hand.SetPos(_SC.PlayerSR.sortingOrder); }
    }

    #endregion
}
