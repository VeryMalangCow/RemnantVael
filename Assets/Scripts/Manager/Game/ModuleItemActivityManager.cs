using System.Collections.Generic;
using UnityEngine;

public class ModuleItemActivityManager : Singleton<ModuleItemActivityManager>
{
    #region Value

    public delegate void ActivityFuncDele(int _Rank, int BoostLv);
    [HideInInspector] public List<ActivityFuncDele> ActivityFuncList = new List<ActivityFuncDele>();

    #endregion

    #region Offset

    protected override void Awake()
    {
        base.Awake();

        ActivityFuncList = new List<ActivityFuncDele>
        {
            Activity_MI_000,
            Activity_MI_001
        };
    }

    #endregion

    #region Get

    public ActivityFuncDele GetCollectActivity(int _ID)
    {
        return ActivityFuncList[_ID];
    }

    #endregion

    #region ModuleItem

    private void Activity_MI_000(int _Rank, int _BoostLv)
    {
        // 편의성
        PlayerController PC = PlayerManager.Instance.PlayerController;
        PlayerWeaponController PCWeapon = PC.BaseWeapon;

        // 확률
        if ((_Rank * _BoostLv) > UnityEngine.Random.Range(0, 100))
        {
            Debug.Log("스폰");

            // 데미지 계산
            float dmg = _Rank * PCWeapon.BaseDamage.ActualState.Value;
            PlayerBulletController pbc = PoolingManager.Instance.GetOP_MI_000_Bullets();
            Vector2 dir = PCWeapon.GetDir(PC.transform.position);

            // 스폰 탄 스탯
            BulletState bulletState = new BulletState(
                eDamageType.Energy,
                dmg, PCWeapon.MuzzleSpeed.ActualState.Value * 0.7f, 2,
                false, 1,
                false, 0, 0);
            pbc.SetState(PC.transform.position, 10, bulletState, dir, 0.35f);

            // Sorting Layer
            if (PC.TargetObject.gameObject.TryGetComponent(out HaveShadowThing hst))
            { pbc.ThisSR.sortingOrder = hst.ThisSR.sortingOrder - 1; }
        }

    }

    private void Activity_MI_001(int _Rank, int _BoostLv)
    {

    }

    #endregion
}
