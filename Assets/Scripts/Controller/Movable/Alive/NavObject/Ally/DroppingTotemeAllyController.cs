using UnityEngine;
using UnityEngine.Serialization;

public class DroppingTotemeAllyController : DroppingAllyController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Dropping")]

    [Space(10)]
    [Header("=== Bullet")]
    [FormerlySerializedAs("HoloSprite")][SerializeField] private Sprite holoSprite;

    [FormerlySerializedAs("PlayerBuffID")][SerializeField] private int playerBuffId;
    [FormerlySerializedAs("AllyBuffID")][SerializeField] private string allyBuffId;

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

        Debug.Log(_name[1] + ": Toteme");
    }


    private void Fire_Toteme(AllyTotemeController toteme, Vector2 targetPos)
    {
        // ÃÑ¾Ë ½ºÅÈ°ú SortingOrder ¼³Á¤
        toteme.Set_State(
            droppingTime: actualAllyState.muzzleSpeed.value,
            topYPos: 5f,
            bottomYPos: dropBottomYPos,
            dur: actualAllyState.dur.value,
            holoSprite: holoSprite,
            clr: extraClr,
            buffAreaSize: 1f,
            state_PosAndRot: Get_BulletState_PosAndRot(targetPos),
            state_Size: Get_BulletState_Shadow_Size());

        toteme.Set_State_BuffID(playerBuffId, allyBuffId);

        // Light & Trail
        toteme.SetOn_LightIntensity(lightIntensity);
        toteme.SetOn_TrailState(trailTime, trailStartWidth * actualAllyState.attackSize.value, extraGradient);

        // ÀÌ¹ÌÁö
        toteme.thisSr.sprite = sprite;
    }

    #endregion

    #region State (Toteme)

    private BulletState_PosAndRot Get_BulletState_PosAndRot(Vector2 targetPos)
    {
        return new BulletState_PosAndRot(
            targetPos,
            Vector2.zero,
            0);
    }

    private BulletState_Size Get_BulletState_Shadow_Size()
    {
        return new BulletState_Size(
            Vector2.one * actualAllyState.attackSize.value,
            new Vector2(0.3f, 0.15f));
    }

    #endregion
}
