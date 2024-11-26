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
    [SerializeField] private SatelliteController WalkingSatellite;
    [SerializeField] private SatelliteController LookingSatellite;
    [SerializeField] private float BaseUnderFootAnimSpeed = 1.0f;
    [SerializeField] private List<SetEightDirAnim> ThisSEDA;

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

        SetImgPosSort(WalkingSatellite); 
        SetImgPosSort(LookingSatellite);
    }

    private void LateUpdate()
    {
        SetImg(WalkingSatellite, ThisRb.velocity);
        SetImg(LookingSatellite, LookAtDir);

        SetAnimSpeed();
    }

    #endregion

    #region Img or Anim

    private void SetImg(SatelliteController _SC, Vector2 _Dir)
    {
        if (_Dir != Vector2.zero)
        {
            _Dir = new Vector2(-_Dir.x, _Dir.y);
            if (SatelliteController.GetIndex(Quaternion.FromToRotation(Vector3.up, _Dir).eulerAngles.z) != CurrentIndex.Value)
            {
                CurrentIndex.Value = SatelliteController.GetIndex(Quaternion.FromToRotation(Vector3.up, _Dir).eulerAngles.z);
            }

            _SC.PitchTF.transform.localRotation = _SC.RotateSmooth(SatelliteController.GetNormalizedVec(CurrentIndex.Value));
        }
    }

    private void SetImgPosSort(SatelliteController _SC)
    {
        foreach (Satellite hand in _SC.Hands)
        { hand.SetPos(_SC.PlayerSR.sortingOrder); }
    }

    private void SetAnimSpeed()
    {
        float dis = Vector2.Distance(Vector2.zero, ThisRb.velocity);
        for (int i = 0; i < ThisSEDA.Count; i++)
        {
            ThisSEDA[i].SetAnimSpeed(BaseUnderFootAnimSpeed * dis);
        }
    }
    #endregion
}
