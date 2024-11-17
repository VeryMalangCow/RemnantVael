using System.Collections;
using UnityEngine;

public class EnemyPattern_Range : EnemyPattern
{
    #region Can Check

    public override bool CanPlayPattern()
    {
        return true;
    }

    #endregion

    #region Start End

    public override void StartPattern()
    {
        base.StartPattern();
        StartCoroutine(ThisPattern());
    }

    public override void EndPattern()
    {
        base.EndPattern();
    }

    #endregion

    #region Actual

    private IEnumerator ThisPattern()
    {
        yield return new WaitForSeconds(StartDelay);

        #region Actual



        #endregion

        yield return new WaitForSeconds(EndDelay);

        EndPattern();
    }

    #endregion
}

