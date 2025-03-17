using UnityEngine;
using System.Collections;

public class EnemyPattern_None : EnemyPattern
{
    #region Can Check

    public override bool Can_PlayPattern()
    {
        return true;
    }

    #endregion

    #region Actual

    protected override IEnumerator Play_ThisPattern_Cor()
    {
        yield return new WaitForSeconds(StartDelay + EndDelay);

        base.End_Pattern();
        ThisEnemy.Play_Pattern();
    }

    #endregion
}
