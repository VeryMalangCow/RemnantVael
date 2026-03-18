using System.Collections.Generic;
using UnityEngine;

public class RoomRuleController : MonoBehaviour
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Room Rule")]

    [Space(10)]
    [Header("=== Data")]

    [Space(5)]
    [Header("-- Vec")]
    [SerializeField] public List<Vector2Int> RoomVec;

    [Space(5)]
    [Header("-- Type")]
    [SerializeField] public eRoomType RoomType;
    [SerializeField] public eEnemy EnemyType = eEnemy.Normal;
    [SerializeField] public bool IsAlreadyRoomClear = false;

    [Space(10)]
    [Header("=== Parent TF")]

    [Space(5)]
    [Header("-- Build")]
    [SerializeField] private Transform InRoom_ObstacleParentTF;

    [Space(5)]
    [Header("-- Enemy")]
    [SerializeField] private Transform InRoom_EnemySpawnParentTF;

    [Space(5)]
    [Header("-- Field Obj")]
    [SerializeField] private Transform InRoom_FieldObjSpawnerParentTF;

    #endregion

    #region - Hide

    [HideInInspector] private List<EnemySpawnContoller> InRoom_AllEnemySpawn;
    [HideInInspector] public List<SortingObjectController> InRoom_AllObstacle;
    [HideInInspector] private InteractableBuildController InRoom_ShopBuild;

    [HideInInspector] protected int NeedKeyCardID = -1;

    #endregion

    #endregion

    #region Offset

    public virtual void Offset()
    {
        InRoom_AllObstacle = InRoom_ObstacleParentTF != null &&
            InRoom_ObstacleParentTF.childCount > 0 ?
            DevTool.Get_ChildList<SortingObjectController>(InRoom_ObstacleParentTF) : null;

        InRoom_AllEnemySpawn = InRoom_EnemySpawnParentTF != null &&
            InRoom_EnemySpawnParentTF.childCount > 0 ?
            DevTool.Get_ChildList<EnemySpawnContoller>(InRoom_EnemySpawnParentTF) : null;

    }

    #endregion

    #region KillAll

    public bool Is_EliteEnemyRoom(out int _EliteEnemyID)
    {
        _EliteEnemyID = 0;
        if (EnemyType != eEnemy.Elite || 
            RoomType != eRoomType.KillAll) 
            return false;

        for (int i = 0; i < InRoom_AllEnemySpawn.Count; i++)
        {
            if (InRoom_AllEnemySpawn[i].Get_EnemyType() == eEnemy.Elite)
            {
                _EliteEnemyID = InRoom_AllEnemySpawn[i].Get_SpawnID();
                return true;
            }
        }

        return false;
    }

    public bool Is_BossEnemyRoom(out int _BossEnemyID)
    {
        _BossEnemyID = 0;
        if (EnemyType != eEnemy.Boss ||
            RoomType != eRoomType.KillAll)
            return false;

        for (int i = 0; i < InRoom_AllEnemySpawn.Count; i++)
        {
            if (InRoom_AllEnemySpawn[i].Get_EnemyType() == eEnemy.Boss)
            {
                _BossEnemyID = InRoom_AllEnemySpawn[i].Get_SpawnID();
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Set

    #region Completed

    public virtual void Set_Completed()
    {
        SetOn_Shop();

        if (!SoundManager.isPlayingBaseBGM)
            SoundManager.instance.CastBGM_ToBase();
    }

    private void SetOn_Shop()
    {
        if (InRoom_ShopBuild != null &&
            !InRoom_ShopBuild.gameObject.activeSelf)
        {
            InRoom_ShopBuild.gameObject.SetActive(true);
        }
    }


    #endregion

    #region Kill All

    public void Set_KillAll()
    {
        Spawn_AllEnemy();

        if (EnemyType == eEnemy.Elite && SoundManager.isPlayingBaseBGM)
        {
            SoundManager.instance.CastBGM_ToExtra();
            SoundManager.instance.Play_2D_ExtraBGM("Elite");
        }
        else if (EnemyType == eEnemy.Boss && SoundManager.isPlayingBaseBGM)
        {
            SoundManager.instance.CastBGM_ToExtra();
            SoundManager.instance.Play_2D_ExtraBGM("Boss");
        }
    }

    private void Spawn_AllEnemy()
    {
        for (int i = 0; i < InRoom_AllEnemySpawn.Count; i++)
        {
            Vector2 spawnPos = InRoom_AllEnemySpawn[i].transform.position;

            EnemyController enemy = PoolingManager.instance.Get_OP_Enemy(
                InRoom_AllEnemySpawn[i].Get_EnemyType(),
                InRoom_AllEnemySpawn[i].Get_SpawnID());

            enemy.transform.position = spawnPos;
            enemy.gameObject.SetActive(true);

            // VFX
            UnitManager.instance.enemy_ExplImgGenerator.Expl_Enemy(spawnPos + (Vector2.up * enemy.TargetRange));
        }
    }

    #endregion

    #endregion

    #region Sorting

    public void Set_SortingStaticObjects()
    {
        if (InRoom_AllObstacle != null && InRoom_AllObstacle.Count > 0)
            LayerOrderManager.instance.Add_NeedSortObj(InRoom_AllObstacle);
    }

    #endregion

    #region KeyCard

    public int Get_NeedKeyCardID()
    {
        return NeedKeyCardID;
    }

    #endregion

    #region FieldObj

    public List<Vector2> Get_FieldObjPos()
    {
        List<FieldObjectSpawnController> fieldObjSpawners = InRoom_FieldObjSpawnerParentTF != null &&
            InRoom_FieldObjSpawnerParentTF.childCount > 0 ?
            DevTool.Get_AllChildList<FieldObjectSpawnController>(InRoom_FieldObjSpawnerParentTF) : null;

        List<Vector2> result = new List<Vector2>();

        if (fieldObjSpawners != null)
            for (int i = 0; i < fieldObjSpawners.Count; i++)
                result.AddRange(fieldObjSpawners[i].Get_RandomPointsInSector_Self());

        return result;
    }

    #endregion
}