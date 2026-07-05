
public class AllyTotemeController : TotemeController
{
    protected override void RemoveObject()
    {
        AllyManager.instance.RemoveAllyToteme(this);
    }
}
