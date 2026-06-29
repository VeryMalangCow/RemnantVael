using UnityEngine;

public class PassageRoomController : RoomController
{
    protected StageThemeSO afterStageThemeSO;

    private StagePassageThemeSO passageStageThemeSO;


    public void Offset(RoomRuleController rule, int instanceId, int typeId, Vector2Int worldGridPivot, StageThemeSO beforeStageThemeSO, StageThemeSO afterStageThemeSO, StagePassageThemeSO passageStageThemeSO)
    {
        Offset(rule, instanceId, typeId, worldGridPivot, beforeStageThemeSO);
        this.afterStageThemeSO = afterStageThemeSO;
        this.passageStageThemeSO = passageStageThemeSO;
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
            else if (child.TryGetComponent(out RoomPassageMiddleVisualSprite visualMiddleSprite))
            {
                visualMiddleSprite.SetSprite(passageStageThemeSO);
            }
        }
    }
}
