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
    [SerializeField] public List<Vector2Int> roomVec;

    [Space(5)]
    [Header("-- Type")]
    [SerializeField] public eRoomType roomType;
    [SerializeField] public eEnemy enemyType = eEnemy.Normal;

    [Space(10)]
    [Header("=== Parent TF")]

    [Space(5)]
    [Header("-- Build")]
    [SerializeField] private Transform inRoom_ObstacleParentTF;

    [Space(5)]
    [Header("-- Enemy")]
    [SerializeField] private Transform inRoom_EnemySpawnParentTF;

    [Space(5)]
    [Header("-- Field Obj")]
    [SerializeField] private Transform inRoom_FieldObjSpawnerParentTF;

    #endregion

    #region - Hide

    [HideInInspector] private List<EnemySpawnContoller> inRoom_AllEnemySpawn;
    [HideInInspector] public List<SortingObjectController> inRoom_AllObstacle;
    [HideInInspector] private InteractableBuildController inRoom_ShopBuild;

    [HideInInspector] protected int needKeyCardId = -1;

    #endregion

    #endregion

    #region Offset

    public virtual void Offset()
    {
        inRoom_AllObstacle = inRoom_ObstacleParentTF != null &&
            inRoom_ObstacleParentTF.childCount > 0 ?
            DevTool.Get_ChildList<SortingObjectController>(inRoom_ObstacleParentTF) : null;

        inRoom_AllEnemySpawn = inRoom_EnemySpawnParentTF != null &&
            inRoom_EnemySpawnParentTF.childCount > 0 ?
            DevTool.Get_ChildList<EnemySpawnContoller>(inRoom_EnemySpawnParentTF) : null;

    }

    #endregion

    #region KillAll

    public bool Is_EliteEnemyRoom(out int eliteEnemyId)
    {
        eliteEnemyId = 0;
        if (enemyType != eEnemy.Elite || 
            roomType != eRoomType.KillAll) 
            return false;

        for (int i = 0; i < inRoom_AllEnemySpawn.Count; i++)
        {
            if (inRoom_AllEnemySpawn[i].Get_EnemyType() == eEnemy.Elite)
            {
                eliteEnemyId = inRoom_AllEnemySpawn[i].Get_SpawnID();
                return true;
            }
        }

        return false;
    }

    public bool Is_BossEnemyRoom(out int bossEnemyId)
    {
        bossEnemyId = 0;
        if (enemyType != eEnemy.Boss ||
            roomType != eRoomType.KillAll)
            return false;

        for (int i = 0; i < inRoom_AllEnemySpawn.Count; i++)
        {
            if (inRoom_AllEnemySpawn[i].Get_EnemyType() == eEnemy.Boss)
            {
                bossEnemyId = inRoom_AllEnemySpawn[i].Get_SpawnID();
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Set

    #region Completed

    public virtual void Complete()
    {
        SetOn_Shop();

        if (!SoundManager.isPlayingBaseBGM)
            SoundManager.instance.CastBGM_ToBase();
    }

    private void SetOn_Shop()
    {
        if (inRoom_ShopBuild != null &&
            !inRoom_ShopBuild.gameObject.activeSelf)
        {
            inRoom_ShopBuild.gameObject.SetActive(true);
        }
    }


    #endregion

    #region Kill All

    public void KillAll()
    {
        Spawn_AllEnemy();

        if (enemyType == eEnemy.Elite && SoundManager.isPlayingBaseBGM)
        {
            SoundManager.instance.CastBGM_ToExtra();
            SoundManager.instance.Play_2D_ExtraBGM("Elite");
        }
        else if (enemyType == eEnemy.Boss && SoundManager.isPlayingBaseBGM)
        {
            SoundManager.instance.CastBGM_ToExtra();
            SoundManager.instance.Play_2D_ExtraBGM("Boss");
        }
    }

    private void Spawn_AllEnemy()
    {
        for (int i = 0; i < inRoom_AllEnemySpawn.Count; i++)
        {
            Vector2 spawnPos = inRoom_AllEnemySpawn[i].transform.position;

            EnemyController enemy = EnemyManager.instance.SpawnEnemy(inRoom_AllEnemySpawn[i].Get_EnemyType(), inRoom_AllEnemySpawn[i].Get_SpawnID());

            enemy.transform.position = spawnPos;
            enemy.gameObject.SetActive(true);

            // VFX
            VFXManager.instance.enemy_ExplImgGenerator.Expl_Enemy(spawnPos + (Vector2.up * enemy.targetRange));
        }
    }

    #endregion

    #endregion

    #region KeyCard

    public int Get_NeedKeyCardID()
    {
        return needKeyCardId;
    }

    #endregion

    #region FieldObj

    public List<Vector2> Get_FieldObjPos()
    {
        List<FieldObjectSpawnController> fieldObjSpawners = inRoom_FieldObjSpawnerParentTF != null &&
            inRoom_FieldObjSpawnerParentTF.childCount > 0 ?
            DevTool.Get_AllChildList<FieldObjectSpawnController>(inRoom_FieldObjSpawnerParentTF) : null;

        List<Vector2> result = new List<Vector2>();

        if (fieldObjSpawners != null)
            for (int i = 0; i < fieldObjSpawners.Count; i++)
                result.AddRange(fieldObjSpawners[i].Get_RandomPointsInSector_Self());

        return result;
    }

    #endregion
}