using UnityEngine;

public class BuldPassageMiddleSpriteController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Passage Middle Sprite")]
    [SerializeField] private string SpriteKey;

    #endregion

    #region Offset

    private void Offset()
    {
        if (DevTool.Get_ComponentTType(gameObject, out SpriteRenderer sr))
        {
            if (SpriteKey == "")
            {
                string[] fullName = sr.sprite.name.Split("_");
                SpriteKey = 
                    $"{fullName[1]}_" +
                    $"{DevTool.Get_LengthString(StageManager.instance.Get_BeforeStageID(), 2)}_" +
                    $"{DevTool.Get_LengthString(StageManager.instance.Get_AfterStageID(), 2)}";
            }

            StageManager.instance.Set_PassageMiddleSprite(sr, SpriteKey);
        }
    }

    #endregion

    #region Framework

    private void Start()
    {
        Offset();
    }

    #endregion
}
