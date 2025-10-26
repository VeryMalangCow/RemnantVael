using UnityEngine;

public class OnceTimeAnimGenerator : MonoBehaviour
{
    #region Player Attack

    // 적에게 공격이 명중했을 경우 (공격자 기준)
    public void Anim_AttackSuccess(Vector2 _SpawnPos, eDamageType _DamageType, bool _IsCritical, float _AnimSize = 1)
    {
        State_Anim anim = new State_Anim(
            PlayerManager.Instance.PlayerController.Get_CorrectAC(_DamageType, _IsCritical), 2f);
        State_TF2D tf = new State_TF2D(
            _SpawnPos, Quaternion.identity, Vector2.one * _AnimSize);
        State_Sprite sprite = new State_Sprite(
            PlayerManager.Instance.PlayerController.MaterialList[0], Color.white);
        
        Gen_OOA().Start_Anim(anim, tf, sprite);
    }

    #endregion

    #region Enemy Hitted

    // 적이 공격을 받았을 경우 (피해자 기준)
    public void Anim_Attacked_Circle(Vector2 _SpawnPos, Quaternion _Rotation)
    {
        State_Anim anim = new State_Anim(
            EnemyManager.Instance.HittedAC_0, 1.5f);
        State_TF2D tf = new State_TF2D(
            _SpawnPos, DevTool.Get_FlipRotation(_Rotation), Vector2.one);
        State_Sprite sprite = new State_Sprite(
            ResourceManager.Instance.Get_ModuleMaterial("Explosion"), Color.white);

        Gen_OOA().Start_Anim(anim, tf, sprite);
    }

    // 적이 공격을 받았을 경우 (피해자 기준)
    public void Anim_Attacked_Slice(Vector2 _SpawnPos, bool _IsCritical, Quaternion _Rotation)
    {
        if (!_IsCritical)
        { return; }

        for (int i = 0; i < 2; i++)
        {
            State_Anim anim = new State_Anim(
                EnemyManager.Instance.HittedAC_1, 2.5f);
            State_TF2D tf = new State_TF2D(
                _SpawnPos, DevTool.Add_RotZValue(_Rotation, i == 0 ? -45 : 45), Vector2.one);
            State_Sprite sprite = new State_Sprite(
                ResourceManager.Instance.Get_ModuleMaterial("Explosion"), Color.white);

            Gen_OOA().Start_Anim(anim, tf, sprite);
        }

    }

    // 적이 죽을 경우
    public void Anim_Attacked_BigSlice(Vector2 _SpawnPos)
    {
        State_Anim anim = new State_Anim(
            EnemyManager.Instance.HittedAC_2, 1.5f);
        State_TF2D tf = new State_TF2D(
            _SpawnPos, DevTool.Add_RotZValue(Quaternion.identity, DevTool.Get_RandomValueBaseZero(45f)), Vector2.one * 2f);
        State_Sprite sprite = new State_Sprite(
            ResourceManager.Instance.Get_ModuleMaterial("Explosion"), Color.white);

        Gen_OOA().Start_Anim(anim, tf, sprite);
    }

    
    #endregion

    #region Module

    private OnceTimeAnimController Gen_OOA()
    {
        OnceTimeAnimController otac = PoolingManager.Instance.Get_OP_OnlyOnceAnimator();
        otac.gameObject.transform.SetParent(StageManager.Instance.CurrentRoomController.transform);
        return otac;
    }

    #endregion
}
