using UnityEngine;

/*
Room 프리팹을 Runtime용 데이터로 변환
프리팹 저장, 에셋 생성, 로그 출력 등의 작업은 수행하지 않음
*/

/// <summary> Room 프리팹을 Runtime용으로 변환 </summary>
public static class RuntimeRoomProcessor
{
    public static void Process(GameObject roomRoot, BakeResult result)
    {
        if (roomRoot == null || result == null)
            return;

        RoomVisualSprite[] visualSprites =
            roomRoot.GetComponentsInChildren<RoomVisualSprite>(true);

        RoomPassageVisualSprite[] passageVisualSprites =
            roomRoot.GetComponentsInChildren<RoomPassageVisualSprite>(true);

        RoomPassageMiddleVisualSprite[] passageVisualMiddleSprites =
            roomRoot.GetComponentsInChildren<RoomPassageMiddleVisualSprite>(true);

        result.TotalCount 
            = visualSprites.Length 
            + passageVisualSprites.Length
            + passageVisualMiddleSprites.Length;


        foreach (RoomVisualSprite visualSprite in visualSprites)
            ProcessVisualSprite(visualSprite, result);

        foreach (RoomPassageVisualSprite passageVisualSprite in passageVisualSprites)
            ProcessPassageVisualSprite(passageVisualSprite, result);

        foreach (RoomPassageMiddleVisualSprite passageVisualMiddleSprite in passageVisualMiddleSprites)
            ProcessPassageMiddleVisualSprite(passageVisualMiddleSprite, result);
    }

    private static void ProcessVisualSprite(RoomVisualSprite controller, BakeResult result)
    {
        if (controller == null)
            return;

        SpriteRenderer sr = controller.GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            result.AddWarning($"{controller.name} : Missing SpriteRenderer.");
            return;
        }

        if (!RuntimeRoomBakeUtility.TryParseSpriteIndex(sr.sprite, out int index))
        {
            result.AddWarning($"{controller.name} : Invalid Sprite Name.");

            sr.sprite = null;
            return;
        }

        controller.SetData(index);

        sr.sprite = null;

        result.SuccessCount++;
    }
    private static void ProcessPassageVisualSprite(RoomPassageVisualSprite controller, BakeResult result)
    {
        if (controller == null)
            return;

        SpriteRenderer sr = controller.GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            result.AddWarning($"{controller.name} : Missing SpriteRenderer.");
            return;
        }

        if (!RuntimeRoomBakeUtility.TryParseSpriteIndex(sr.sprite, out int index))
        {
            result.AddWarning($"{controller.name} : Invalid Sprite Name.");

            sr.sprite = null;
            return;
        }

        controller.SetData(index);

        sr.sprite = null;

        result.SuccessCount++;
    }
    private static void ProcessPassageMiddleVisualSprite(RoomPassageMiddleVisualSprite controller, BakeResult result)
    {
        if (controller == null)
            return;

        SpriteRenderer sr = controller.GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            result.AddWarning($"{controller.name} : Missing SpriteRenderer.");
            return;
        }

        if (!RuntimeRoomBakeUtility.TryParsePassageSpriteIndex(sr.sprite, out int index))
        {
            result.AddWarning($"{controller.name} : Invalid Sprite Name.");

            sr.sprite = null;
            return;
        }

        controller.SetData(index);

        sr.sprite = null;

        result.SuccessCount++;
    }

}
