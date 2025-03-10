using UnityEngine;

public class OnceTimeAnimGenerator : MonoBehaviour
{
    #region Player Attack

    // 적에게 공격이 명중했을 경우 (공격자 기준)
    public void Anim_AttackSuccess(Vector2 _SpawnPos, eDamageType _DamageType, bool _IsCritical, float _AnimSize = 1)
    {
        Gen_OOA().Start_Anim(
            PlayerManager.Instance.PlayerController.Get_AnimClip_CorrectHitted(_DamageType, _IsCritical),
            _SpawnPos,
            PlayerManager.Instance.PlayerController.ThisPlayerMaterialList[0],
            2.0f, _AnimSize);
    }

    #endregion

    #region Enemy Hitted

    // 적이 공격을 받았을 경우 (피해자 기준)
    public void Anim_Attacked_Circle(Vector2 _SpawnPos, Quaternion _Rotation)
    {
        Gen_OOA().Start_Anim(
            EnemyManager.Instance.HittedAC_0,
            _SpawnPos,
            UnitManager.Instance.ModuleM_000_Explosion,
            DevTool.Get_FlipRotation(_Rotation),
            1.5f, 1f);
    }

    // 적이 공격을 받았을 경우 (피해자 기준)
    public void Anim_Attacked_Slice(Vector2 _SpawnPos, bool _IsCritical, Quaternion _Rotation)
    {
        if (!_IsCritical)
        { return; }

        Gen_OOA().Start_Anim(
            EnemyManager.Instance.HittedAC_1,
            _SpawnPos,
            UnitManager.Instance.ModuleM_000_Explosion,
            DevTool.Add_RotZValue(_Rotation, -45f),
            2.5f, 1.0f);

        Gen_OOA().Start_Anim(
            EnemyManager.Instance.HittedAC_1,
            _SpawnPos,
            UnitManager.Instance.ModuleM_000_Explosion,
            DevTool.Add_RotZValue(_Rotation, 45f),
            2.5f, 1.0f);
    }

    // 적이 죽을 경우
    public void Anim_Attacked_BigSlice(Vector2 _SpawnPos)
    {
        Gen_OOA().Start_Anim(
            EnemyManager.Instance.HittedAC_2,
            _SpawnPos,
            UnitManager.Instance.ModuleM_000_Explosion,
            DevTool.Add_RotZValue(Quaternion.identity, DevTool.Get_RandomValueBaseZero(45f)),
            2.5f, 2f);
    }

    
    #endregion

    #region Module

    private OnceTimeAnimController Gen_OOA()
    {
        return PoolingManager.Instance.Get_OP_OnlyOnceAnimator();
    }

    #endregion
}
