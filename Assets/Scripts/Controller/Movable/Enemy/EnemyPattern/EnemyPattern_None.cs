using UnityEngine;
using System.Collections;

public class EnemyPattern_None : EnemyPattern
{
    #region Can Check

    public override bool CanPlayPattern()
    {
        return true;
    }

    #endregion

    #region Actual

    protected override IEnumerator ThisPattern()
    {
        yield return new WaitForSeconds(StartDelay);

        #region Actual

        // NONE

        #endregion

        yield return new WaitForSeconds(EndDelay);

        EndPattern();
        ThisEnemy.TryGetAnyPattern();
    }

    #endregion
}
