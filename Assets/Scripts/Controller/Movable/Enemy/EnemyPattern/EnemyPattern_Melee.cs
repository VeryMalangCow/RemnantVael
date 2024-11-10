using System.Collections;

public class EnemyPattern_Melee : EnemyPattern
{
    #region Can Check

    public override bool CanPlayPattern()
    {
        return false;
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
        yield return null;
    }

    #endregion
}
