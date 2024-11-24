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

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        CurrentIndex.Value = 5;
    }

    private void LateUpdate()
    {
        SetImg(WalkingSatellite, ThisRb.velocity);
        SetImg(LookingSatellite, LookAtDir);
        //SetImg_ByVelocity(WalkingSatellite);
        //SetImg_ByLookingTarget(LookingSatellite);
    }

    #endregion

    #region Img or Anim

    private void SetImg(SatelliteController _SC, Vector2 _Dir)
    {
        if (_Dir != Vector2.zero)
        {
            _Dir = new Vector2(-_Dir.x, _Dir.y);
            SetImgActual(_Dir, _SC);
        }
    }

    private void SetImgActual(Vector2 _Dir, SatelliteController _SC)
    {
        if (SatelliteController.GetIndex(Quaternion.FromToRotation(Vector3.up, _Dir).eulerAngles.z) != CurrentIndex.Value)
        {
            CurrentIndex.Value = SatelliteController.GetIndex(Quaternion.FromToRotation(Vector3.up, _Dir).eulerAngles.z);
        }

        _SC.PitchTF.transform.localRotation = _SC.RotateSmooth(SatelliteController.GetNormalizedVec(CurrentIndex.Value));
        foreach (Satellite hand in _SC.Hands)
        {
            hand.SetPos(_SC.PlayerSR.sortingOrder);
        }
    }

    #endregion
}
