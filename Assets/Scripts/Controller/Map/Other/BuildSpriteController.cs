using UnityEngine;

public class BuildSpriteController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Build Sprite")]
    [SerializeField] private string SpriteKey;

    #endregion

    #region Offset

    private void Offset()
    {
        if (DevTool.Get_ComponentTType(gameObject, out SpriteRenderer sr))
        {
            if (SpriteKey == "")
                SpriteKey = sr.sprite.name.Substring(5, sr.sprite.name.Length - 5);

            StageManager.Instance.Set_MapSprite(sr, SpriteKey);
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
