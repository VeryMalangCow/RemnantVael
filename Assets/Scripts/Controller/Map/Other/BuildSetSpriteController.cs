
// +클리어할 시 변경 적용하는 Build Sprite System
public class BuildSetSpriteController : BuildSpriteController
{
    #region Set

    private void Add_CurrentSetSprite()
    {
        StageManager.instance.AddSetSprite(this);
    }

    #endregion

    #region Framework

    private void OnEnable()
    {
        Add_CurrentSetSprite();
    }

    #endregion
}
