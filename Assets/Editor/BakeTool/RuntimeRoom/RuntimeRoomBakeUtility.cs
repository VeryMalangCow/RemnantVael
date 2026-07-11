using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
Room Bake에서만 사용하는 전용 유틸리티
공용 기능은 BakeUtility에 두고, Room Bake 전용 기능만 담당
*/

/// <summary> Room Bake에서 사용하는 전용 유틸리티 </summary>
public static class RuntimeRoomBakeUtility
{
    private const string Prefix = "Map00_";
    private const string Prefix_Passage = "MapPassage_";

    /// <summary> Sprite 이름에서 Stage Sprite Index를 추출 - 예) Map00_021 -> 21 </summary>
    public static bool TryParseSpriteIndex(Sprite sprite, out int index)
    {
        index = -1;

        if (sprite == null)
            return false;

        string spriteName = sprite.name;

        if (!spriteName.StartsWith(Prefix))
            return false;

        string indexString = spriteName.Substring(Prefix.Length);

        return int.TryParse(indexString, out index);
    }

    /// <summary> Sprite 이름에서 Passage Stage Sprite Index를 추출 - 예) MapPassage_021 -> 21 </summary>
    public static bool TryParsePassageSpriteIndex(Sprite sprite, out int index)
    {
        index = -1;

        if (sprite == null)
            return false;

        string spriteName = sprite.name;

        if (!spriteName.StartsWith(Prefix_Passage))
            return false;

        string indexString = spriteName.Substring(Prefix_Passage.Length);

        return int.TryParse(indexString, out index);
    }

}