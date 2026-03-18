using UnityEngine;

public class RangeAbsorbItemController : AbsorbItemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Range Absorb")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private float AbsorbRange = 1f;


    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        if (!IsAbsorbing)
        {
            IsAbsorbing =
                Vector2.Distance(PlayerManager.instance.playerController.gameObject.transform.position, this.gameObject.transform.position)
                <= AbsorbRange;
        }
    }

    #endregion

    #region State

    public override void Set_State(Vector2 _SpawnPos)
    {
        base.Set_State(_SpawnPos);

        IsSpawnNow = true;
    }

    #endregion
}
