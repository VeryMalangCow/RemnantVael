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
        yield return new WaitForSeconds(startDelay + endDelay);

        base.End_Pattern();
        enemy.Play_Pattern();
    }

    #endregion
}
