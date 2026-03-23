using UnityEngine;

public class RangeAbsorbItemController : AbsorbItemController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Range Absorb")]

    [Space(10)]
    [Header("=== State")]
    [SerializeField] private float absorbRange = 1f;


    #endregion

    #region Framework

    protected override void Update()
    {
        base.Update();

        if (!isAbsorbing)
        {
            isAbsorbing =
                Vector2.Distance(PlayerManager.instance.playerController.gameObject.transform.position, this.gameObject.transform.position)
                <= absorbRange;
        }
    }

    #endregion

    #region State

    public override void Set_State(Vector2 spawnPos)
    {
        base.Set_State(spawnPos);

        isSpawnNow = true;
    }

    #endregion
}
