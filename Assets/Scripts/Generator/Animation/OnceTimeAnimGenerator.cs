using UnityEngine;

public class OnceTimeAnimGenerator : MonoBehaviour
{
    private Material enemyHittedExplosionMaterial;

    private void Start()
    {
        enemyHittedExplosionMaterial = StaticResourceManager.instance.ExplosionReso.enemyHittedVfxMaterial;
    }

    #region Player Attack

    // 적에게 공격이 명중했을 경우 (공격자 기준)
    public void Anim_AttackSuccess(Vector2 spawnPos, eDamageType dmgType, bool isCritical, float animSize = 1)
    {
        State_Anim anim = new State_Anim(
            PlayerManager.instance.playerController.Get_CorrectAC(dmgType, isCritical), 2f);
        State_TF2D tf = new State_TF2D(
            spawnPos, Quaternion.identity, Vector2.one * animSize);
        State_Sprite sprite = new State_Sprite(
            PlayerManager.instance.playerController.materialList[0], Color.white);
        
        Gen_OOA().Start_Anim(anim, tf, sprite);
    }

    #endregion

    #region Enemy Hitted

    // 적이 공격을 받았을 경우 (피해자 기준)
    public void Anim_Attacked_Circle(Vector2 spawnPos, Quaternion rot)
    {
        State_Anim anim = new State_Anim(
            EnemyManager.instance.hittedAC_0, 1.5f);
        State_TF2D tf = new State_TF2D(
            spawnPos, DevTool.Get_FlipRotation(rot), Vector2.one);
        State_Sprite sprite = new State_Sprite(
            enemyHittedExplosionMaterial, Color.white);

        Gen_OOA().Start_Anim(anim, tf, sprite);
    }

    // 적이 공격을 받았을 경우 (피해자 기준)
    public void Anim_Attacked_Slice(Vector2 spawnPos, bool isCritical, Quaternion rot)
    {
        if (!isCritical)
        { return; }

        for (int i = 0; i < 2; i++)
        {
            State_Anim anim = new State_Anim(
                EnemyManager.instance.hittedAC_1, 2.5f);
            State_TF2D tf = new State_TF2D(
                spawnPos, DevTool.Add_RotZValue(rot, i == 0 ? -45 : 45), Vector2.one);
            State_Sprite sprite = new State_Sprite(
                enemyHittedExplosionMaterial, Color.white);

            Gen_OOA().Start_Anim(anim, tf, sprite);
        }

    }

    // 적이 죽을 경우
    public void Anim_Attacked_BigSlice(Vector2 spawnPos)
    {
        State_Anim anim = new State_Anim(
            EnemyManager.instance.hittedAC_2, 1.5f);
        State_TF2D tf = new State_TF2D(
            spawnPos, DevTool.Add_RotZValue(Quaternion.identity, DevTool.Get_RandomValueBaseZero(45f)), Vector2.one * 2f);
        State_Sprite sprite = new State_Sprite(
            enemyHittedExplosionMaterial, Color.white);

        Gen_OOA().Start_Anim(anim, tf, sprite);
    }

    
    #endregion

    #region Module

    private OnceTimeAnimController Gen_OOA()
    {
        OnceTimeAnimController otac = VFXManager.instance.SpawnOnlyOnceAnim();
        otac.gameObject.transform.SetParent(StageManager.instance.currentRoomController.transform);
        return otac;
    }

    #endregion
}
