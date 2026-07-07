
public class NeoTeamPrisonController : PrisonController
{
    #region Offset

    private void Offset_TypeIconTxt()
    {
        var prefab = StaticResourceManager.instance.BuildReso.prisonPrefab;
        var build = prefab.builds[2];
        typeIcon.sprite = build.teamIcon.typeSpecial;
        allySprites = build.allySprite;

        for (int i = 0; i < prisonAllySrList.Count; i++)
        {
            prisonAllySrList[i].sprite = allySprites.bind;
            prisonAllySrList[i].material = prefab.allyMaterial;
        }
    }

    protected override void Offset()
    {
        base.Offset();

        Offset_TypeIconTxt();
    }

    #endregion

    #region Unlock

    public override void Set_Unlock()
    {
        base.Set_Unlock();

        PlayerManager.instance.playerController.GainNeoPresence(allyAmount);
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();
        var lang = StaticResourceManager.instance.staticWords;
        typeTxt.text = $"<size=150%>)</size> {lang.GetLanguage(63)}<size=85%> ({lang.GetLanguage(73)})</size>";
    }

    #endregion
}
