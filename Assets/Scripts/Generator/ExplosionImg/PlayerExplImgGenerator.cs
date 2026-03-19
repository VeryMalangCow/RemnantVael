using System.Collections.Generic;
using UnityEngine;

public class PlayerExplImgGenerator : ExplosionImgGenerator
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]
    [SerializeField] private List<PlayerVisual<List<Sprite>>> SmokeSpriteList;

    #endregion

    #region Expl

    #region Find 

    // 기본 공격 발사
    public void Expl_Player_ShootBaseBullet(int _PlayerID, Vector2 _SpawnPos, Vector2 _Dir, eDamageType _DmgType, bool _IsCritical)
    {
        switch (_PlayerID)
        {
            case 0:
                Expl_Player00_ShootBaseBullet(_SpawnPos, _Dir, _DmgType, _IsCritical);
                break;

            default:
                break;
        }
    }

    // 스킬0 발사
    public void Expl_Player_Skill0(int _PlayerID, Vector2 _SpawnPos, Vector2 _Dir, bool _IsCritical)
    {
        switch (_PlayerID)
        {
            case 0:
                Expl_Player00_Skill0(_SpawnPos, _Dir, _IsCritical);
                break;

            default:
                break;
        }
    }

    // 스킬1 발사
    public void Expl_Player_Skill1(int _PlayerID, Vector2 _SpawnPos)
    {
        switch (_PlayerID)
        {
            case 0:
                Expl_Player00_Skill1(_SpawnPos);
                break;

            default:
                break;
        }
    }

    // 회피
    public void Expl_Player_Avoid(int _PlayerID, Vector2 _SpawnPos)
    {
        switch (_PlayerID)
        {
            case 0:
                Expl_Player00_Avoid(_SpawnPos);
                break;

            default:
                break;
        }
    }


    // 플레이어 오브젝트 파괴
    public void Expl_Player_ObjectDestroy(int _PlayerID, Vector2 _SpawnPos, eDamageType _DmgType, bool _IsCritical)
    {
        switch (_PlayerID)
        {
            case 0:
                Expl_Player00_ObjectDestroy(_SpawnPos, _DmgType, _IsCritical);
                break;

            default:
                break;
        }
    }

    // 플레이어 (큰) 오브젝트 파괴
    public void Expl_Player_BigObjectDestroy(int _PlayerID, Vector2 _SpawnPos, eDamageType _DmgType, bool _IsCritical)
    {
        switch (_PlayerID)
        {
            case 0:
                Expl_Player00_BigObjectDestroy(_SpawnPos, _DmgType, _IsCritical);
                break;

            default:
                break;
        }
    }

   

    #endregion

    #region Player 00

    // 총알 발사
    private void Expl_Player00_ShootBaseBullet(Vector2 _SpawnPos, Vector2 _Dir, eDamageType _DmgType, bool _IsCritical)
    {
        Gen_ExplImg_Sector(
            new ExplState(
                new ExplState_Base(_SpawnPos, 3), 
                Get_Sprite(0,_DmgType, _IsCritical, 0),
                new ExplState_MoveAndScale(_Dir, dis:0.5f, scale:0.8f, time:0.075f, 0.025f), 
                new ExplState_MoveAndScale(_Dir, dis:1.0f, scale:0.4f, time:0.750f, 0.250f)),
            _Dir, 30f);
    }

    // 미사일 발사
    private void Expl_Player00_Skill0(Vector2 _SpawnPos, Vector2 _Dir, bool _IsCritical)
    {
        Gen_ExplImg_Sector(
            new ExplState(
                new ExplState_Base(_SpawnPos, 4),
                Get_Sprite(0, eDamageType.Physics, _IsCritical, 0),
                new ExplState_MoveAndScale(_Dir, dis: 0.6f, scale: 1.4f, time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(_Dir, dis: 1.2f, scale: 0.7f, time: 0.750f, 0.250f)),
            _Dir, 75f);
    }

    // 쇼크웨이브 발사
    private void Expl_Player00_Skill1(Vector2 _SpawnPos)
    {
        if (PlayerManager.instance.playerController.SkillWeapon.SkillList[1] is ShockwaveSkillController shock)
        {
            float usableMaxSize = shock.Get_UsableMaxSize();
            float cc = PlayerManager.instance.playerController.BaseWeapon.CC.actualState.Value;
            int NormalAmount = (int)(36f * (1f - cc));
            int SpecialAmount = (int)(36f * cc);

            for (int i = 2; i < 4; i++)
            {
                Gen_ExplImg_Ellipse(
                    new ExplState(
                        new ExplState_Base(_SpawnPos + new Vector2(0, -0.5f), i == 2 ? NormalAmount : SpecialAmount),
                        Get_Sprite(0, DevTool.Get_DmgTypeFromIndex(i), DevTool.Get_CriticalFromIndex(i), 0),
                        new ExplState_MoveAndScale(Vector2.zero, dis: usableMaxSize * 0.90f, scale: usableMaxSize * 0.65f, time: 0.25f, 0.05f),
                        new ExplState_MoveAndScale(Vector2.zero, dis: usableMaxSize * 1.25f, scale: usableMaxSize * 0.2f, time: 0.45f, 0.05f)),
                    1f, 0.5f);
            }
        }

        
    }

    // 오브젝트 파괴
    private void Expl_Player00_ObjectDestroy(Vector2 _SpawnPos, eDamageType _DmgType, bool _IsCritical)
    {
        Gen_ExplImg_Circle(
            new ExplState(
                new ExplState_Base(_SpawnPos, 4),
                Get_Sprite(0, _DmgType, _IsCritical, 0),
                new ExplState_MoveAndScale(Vector2.zero, dis:0.3f, scale:0.6f, time:0.075f, 0.025f),
                new ExplState_MoveAndScale(Vector2.zero, dis:0.4f, scale:0.3f, time:0.750f, 0.250f)));
    }

    // 큰 오브젝트 파괴
    private void Expl_Player00_BigObjectDestroy(Vector2 _SpawnPos, eDamageType _DmgType, bool _IsCritical)
    {
        Gen_ExplImg_Circle(
            new ExplState(
                new ExplState_Base(_SpawnPos, 4),
                Get_Sprite(0, _DmgType, _IsCritical, 0),
                new ExplState_MoveAndScale(Vector2.zero, dis: 0.3f, scale: 2.0f, time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(Vector2.zero, dis: 0.4f, scale: 1.0f, time: 0.750f, 0.250f)));
    }

    // 회피
    private void Expl_Player00_Avoid(Vector2 _SpawnPos)
    {
        for (int i = 0; i < 4; i++)
        {
            Gen_ExplImg_Circle(
                new ExplState(
                    new ExplState_Base(_SpawnPos, 4),
                    Get_Sprite(0, DevTool.Get_DmgTypeFromIndex(i), DevTool.Get_CriticalFromIndex(i), 0),
                    new ExplState_MoveAndScale(Vector2.zero, dis: 0.3f, scale: 2.0f, time: 0.075f, 0.025f),
                    new ExplState_MoveAndScale(Vector2.zero, dis: 0.8f, scale: 1.0f, time: 0.750f, 0.250f)));
        }
    }

    #endregion

    #endregion

    #region Get

    private ExplState_Sprite Get_Sprite(int _PlayerID, eDamageType _DmgType, bool _IsCritical, int _MaterialIndex)
    {
        return new ExplState_Sprite(
            SmokeSpriteList[_PlayerID].Get_CorrectType(_DmgType).Get_Special(_IsCritical),
            PlayerManager.instance.playerController.MaterialList[_MaterialIndex]);
    }

    #endregion
}