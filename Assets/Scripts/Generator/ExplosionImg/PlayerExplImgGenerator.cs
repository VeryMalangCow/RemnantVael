using System.Collections.Generic;
using UnityEngine;

public class PlayerExplImgGenerator : ExplosionImgGenerator
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]
    [SerializeField] private List<PlayerVisual<List<Sprite>>> smokeSpriteList;

    #endregion

    #region Expl

    #region Find 

    // 기본 공격 발사
    public void Expl_Player_ShootBaseBullet(int playerID, Vector2 spawnPos, Vector2 dir, eDamageType dmgType, bool isCritical)
    {
        switch (playerID)
        {
            case 0:
                Expl_Player00_ShootBaseBullet(spawnPos, dir, dmgType, isCritical);
                break;

            default:
                break;
        }
    }

    // 스킬0 발사
    public void Expl_Player_Skill0(int playerID, Vector2 spawnPos, Vector2 dir, bool isCritical)
    {
        switch (playerID)
        {
            case 0:
                Expl_Player00_Skill0(spawnPos, dir, isCritical);
                break;

            default:
                break;
        }
    }

    // 스킬1 발사
    public void Expl_Player_Skill1(int playerID, Vector2 spawnPos)
    {
        switch (playerID)
        {
            case 0:
                Expl_Player00_Skill1(spawnPos);
                break;

            default:
                break;
        }
    }

    // 회피
    public void Expl_Player_Avoid(int playerID, Vector2 spawnPos)
    {
        switch (playerID)
        {
            case 0:
                Expl_Player00_Avoid(spawnPos);
                break;

            default:
                break;
        }
    }


    // 플레이어 오브젝트 파괴
    public void Expl_Player_ObjectDestroy(int playerID, Vector2 spawnPos, eDamageType dmgType, bool isCritical)
    {
        switch (playerID)
        {
            case 0:
                Expl_Player00_ObjectDestroy(spawnPos, dmgType, isCritical);
                break;

            default:
                break;
        }
    }

    // 플레이어 (큰) 오브젝트 파괴
    public void Expl_Player_BigObjectDestroy(int playerID, Vector2 spawnPos, eDamageType dmgType, bool isCritical)
    {
        switch (playerID)
        {
            case 0:
                Expl_Player00_BigObjectDestroy(spawnPos, dmgType, isCritical);
                break;

            default:
                break;
        }
    }

   

    #endregion

    #region Player 00

    // 총알 발사
    private void Expl_Player00_ShootBaseBullet(Vector2 spawnPos, Vector2 dir, eDamageType dmgType, bool isCritical)
    {
        Gen_ExplImg_Sector(
            new ExplState(
                new ExplState_Base(spawnPos, 3), 
                Get_Sprite(0,dmgType, isCritical, 0),
                new ExplState_MoveAndScale(dir, dis:0.5f, scale:0.8f, time:0.075f, 0.025f), 
                new ExplState_MoveAndScale(dir, dis:1.0f, scale:0.4f, time:0.750f, 0.250f)),
            dir, 30f);
    }

    // 미사일 발사
    private void Expl_Player00_Skill0(Vector2 spawnPos, Vector2 dir, bool isCritical)
    {
        Gen_ExplImg_Sector(
            new ExplState(
                new ExplState_Base(spawnPos, 4),
                Get_Sprite(0, eDamageType.Physics, isCritical, 0),
                new ExplState_MoveAndScale(dir, dis: 0.6f, scale: 1.4f, time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(dir, dis: 1.2f, scale: 0.7f, time: 0.750f, 0.250f)),
            dir, 75f);
    }

    // 쇼크웨이브 발사
    private void Expl_Player00_Skill1(Vector2 spawnPos)
    {
        if (PlayerManager.instance.playerController.skillWeapon.skillList[1] is ShockwaveSkillController shock)
        {
            float usableMaxSize = shock.Get_UsableMaxSize();
            float cc = PlayerManager.instance.playerController.baseWeapon.cc.actualState.Value;
            int normalAmount = (int)(36f * (1f - cc));
            int specialAmount = (int)(36f * cc);

            for (int i = 2; i < 4; i++)
            {
                Gen_ExplImg_Ellipse(
                    new ExplState(
                        new ExplState_Base(spawnPos + new Vector2(0, -0.5f), i == 2 ? normalAmount : specialAmount),
                        Get_Sprite(0, DevTool.Get_DmgTypeFromIndex(i), DevTool.Get_CriticalFromIndex(i), 0),
                        new ExplState_MoveAndScale(Vector2.zero, dis: usableMaxSize * 0.90f, scale: usableMaxSize * 0.65f, time: 0.25f, 0.05f),
                        new ExplState_MoveAndScale(Vector2.zero, dis: usableMaxSize * 1.25f, scale: usableMaxSize * 0.2f, time: 0.45f, 0.05f)),
                    1f, 0.5f);
            }
        }

        
    }

    // 오브젝트 파괴
    private void Expl_Player00_ObjectDestroy(Vector2 spawnPos, eDamageType dmgType, bool isCritical)
    {
        Gen_ExplImg_Circle(
            new ExplState(
                new ExplState_Base(spawnPos, 4),
                Get_Sprite(0, dmgType, isCritical, 0),
                new ExplState_MoveAndScale(Vector2.zero, dis:0.3f, scale:0.6f, time:0.075f, 0.025f),
                new ExplState_MoveAndScale(Vector2.zero, dis:0.4f, scale:0.3f, time:0.750f, 0.250f)));
    }

    // 큰 오브젝트 파괴
    private void Expl_Player00_BigObjectDestroy(Vector2 spawnPos, eDamageType dmgType, bool isCritical)
    {
        Gen_ExplImg_Circle(
            new ExplState(
                new ExplState_Base(spawnPos, 4),
                Get_Sprite(0, dmgType, isCritical, 0),
                new ExplState_MoveAndScale(Vector2.zero, dis: 0.3f, scale: 2.0f, time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(Vector2.zero, dis: 0.4f, scale: 1.0f, time: 0.750f, 0.250f)));
    }

    // 회피
    private void Expl_Player00_Avoid(Vector2 spawnPos)
    {
        for (int i = 0; i < 4; i++)
        {
            Gen_ExplImg_Circle(
                new ExplState(
                    new ExplState_Base(spawnPos, 4),
                    Get_Sprite(0, DevTool.Get_DmgTypeFromIndex(i), DevTool.Get_CriticalFromIndex(i), 0),
                    new ExplState_MoveAndScale(Vector2.zero, dis: 0.3f, scale: 2.0f, time: 0.075f, 0.025f),
                    new ExplState_MoveAndScale(Vector2.zero, dis: 0.8f, scale: 1.0f, time: 0.750f, 0.250f)));
        }
    }

    #endregion

    #endregion

    #region Get

    private ExplState_Sprite Get_Sprite(int playerID, eDamageType dmgType, bool isCritical, int materialIndex)
    {
        return new ExplState_Sprite(
            smokeSpriteList[playerID].Get_CorrectType(dmgType).Get_Special(isCritical),
            PlayerManager.instance.playerController.materialList[materialIndex]);
    }

    #endregion
}