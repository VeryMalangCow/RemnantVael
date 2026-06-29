using UnityEngine;

public class PassageRoomController : RoomController
{
    protected StageThemeSO afterStageThemeSO;


    public void Offset(RoomRuleController rule, int instanceId, int typeId, Vector2Int worldGridPivot, StageThemeSO beforeStageThemeSO, StageThemeSO afterStageThemeSO)
    {
        Offset(rule, instanceId, typeId, worldGridPivot, beforeStageThemeSO);
        this.afterStageThemeSO = afterStageThemeSO;

    }

    public void InitPassageVisualSprite()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.TryGetComponent(out RoomPassageVisualSprite visualSprite))
            {
                if (visualSprite.IsBeforeMap) visualSprite.SetSprite(stageThemeSO);
                else visualSprite.SetSprite(afterStageThemeSO);
            }
        }
    }
}
