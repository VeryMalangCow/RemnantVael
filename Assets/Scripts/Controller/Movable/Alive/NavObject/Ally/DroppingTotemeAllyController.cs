using UnityEngine;

public class DroppingTotemeAllyController : DroppingAllyController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Dropping")]

    [Space(10)]
    [Header("=== Bullet")]
    [SerializeField] private Sprite HoloSprite;

    #endregion

    #region Attack

    protected override bool Can_Shot()
    {
        return base.Can_Shot();
        //return base.Can_Shot() && StageManager.Instance.CurrentRoomController.RoomRuleController.RoomType != eRoomType.Completed;
    }

    protected override void Shot()
    {
        base.Shot();

        Fire_Toteme(PoolingManager.Instance.Get_OP_AllyToteme(), Get_RandomNavPos(PlayerManager.Instance.PlayerController.transform.position, 5f));
    }


    private void Fire_Toteme(AllyTotemeController _Toteme, Vector2 _TargetPos)
    {
        // ÃÑ¾Ë ½ºÅÈ°ú SortingOrder ¼³Á¤
        _Toteme.Set_State(
            _DroppingTime: ActualAllyState.MuzzleSpeed.Value,
            _TopYPos: 5f,
            _BottomYPos: DropBottomYPos,
            _Dur: ActualAllyState.Dur.Value,
            _HoloSprite: HoloSprite,
            _BuffAreaSize: 1f,
            _State_PosAndRot: Get_BulletState_PosAndRot(_TargetPos),
            _State_Size: Get_BulletState_Shadow_Size());

        // Light & Trail
        _Toteme.SetOn_LightIntensity(LightIntensity);
        _Toteme.SetOn_TrailState(TrailTime, TrailStartWidth * ActualAllyState.AttackSize.Value, ThisExtraGradient);

        // ÀÌ¹ÌÁö
        _Toteme.ThisSR.sprite = ThisSprite;
    }

    #endregion

    #region State (Toteme)

    private BulletState_PosAndRot Get_BulletState_PosAndRot(Vector2 _TargetPos)
    {
        return new BulletState_PosAndRot(
            _TargetPos,
            Vector2.zero,
            0);
    }

    private BulletState_Size Get_BulletState_Shadow_Size()
    {
        return new BulletState_Size(
            Vector2.one * ActualAllyState.AttackSize.Value,
            new Vector2(0.3f, 0.15f));
    }

    #endregion
}
