using UnityEngine;

public class DroppingTotemeAllyController : DroppingAllyController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Dropping")]

    [Space(10)]
    [Header("=== Bullet")]
    [SerializeField] private Sprite HoloSprite;

    [SerializeField] private int PlayerBuffID;
    [SerializeField] private string AllyBuffID;

    #endregion

    #region Attack

    protected override bool Can_Shot()
    {
        return base.Can_Shot();   
    }

    protected override void Shot()
    {
        base.Shot();

        Fire_Toteme(PoolingManager.instance.Get_OP_AllyToteme(), Get_RandomNavPos(PlayerManager.instance.playerController.transform.position, 5f));

        Debug.Log(Name[1] + ": Toteme");
    }


    private void Fire_Toteme(AllyTotemeController _Toteme, Vector2 _TargetPos)
    {
        // ÃÑ¾Ë ½ºÅÈ°ú SortingOrder ¼³Á¤
        _Toteme.Set_State(
            _DroppingTime: ActualAllyState.muzzleSpeed.value,
            _TopYPos: 5f,
            _BottomYPos: DropBottomYPos,
            _Dur: ActualAllyState.dur.value,
            _HoloSprite: HoloSprite,
            _Clr: ThisExtraColor,
            _BuffAreaSize: 1f,
            _State_PosAndRot: Get_BulletState_PosAndRot(_TargetPos),
            _State_Size: Get_BulletState_Shadow_Size());

        _Toteme.Set_State_BuffID(PlayerBuffID, AllyBuffID);

        // Light & Trail
        _Toteme.SetOn_LightIntensity(LightIntensity);
        _Toteme.SetOn_TrailState(TrailTime, TrailStartWidth * ActualAllyState.attackSize.value, ThisExtraGradient);

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
            Vector2.one * ActualAllyState.attackSize.value,
            new Vector2(0.3f, 0.15f));
    }

    #endregion
}
