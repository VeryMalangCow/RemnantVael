using System.Collections.Generic;
using UnityEngine;

public class AimController : StaticDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Aim")]
    [SerializeField] private float AimFollowSpeed = 30f;

    [Space(10)]
    [Header("=== Aim")]
    [SerializeField] private CoupleData<SpriteRenderer> AimSR;
    [SerializeField] private PlayerVisual<Sprite> AimSprite;

    [Space(10)]
    [Header("=== Skill")]
    [SerializeField] private List<SpriteRenderer> SkillAimList;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        Offset_SkillAimList();
    }

    private void Offset_SkillAimList()
    {
        DevTool.Set_ListDele(SkillAimList, new Dele_T<SpriteRenderer>(sprite => sprite.gameObject.SetActive(false)));
    }

    #endregion

    #region Framework


    private void LateUpdate()
    {
        Set_AimPosUpdate(Time.deltaTime);
        Set_AimRotUpdate(Time.deltaTime);
    }

    #endregion

    #region Set 

    // 위치 값 업데이트
    private void Set_AimPosUpdate(float _DeltaTime)
    {
        Vector2 fromPos = this.transform.position;
        Vector2 toPos = InputManager.Instance.MousePosByWorld;

        gameObject.transform.position =
            Vector2.Lerp(fromPos, toPos, AimFollowSpeed * _DeltaTime);
    }

    // 회전 값 업데이트
    private void Set_AimRotUpdate(float _DeltaTime)
    {
        Quaternion fromRot = TargetObject.transform.localRotation;
        Quaternion toRot = DevTool.Get_RotFromDir(InputManager.Instance.DirFromPlayerPos);

        TargetObject.transform.localRotation =
                    Quaternion.Slerp(fromRot, toRot, AimFollowSpeed * _DeltaTime);
    }

    // 데미지 타입: 물리
    public void Set_PhysicsType()
    {
        Set_DmgType(AimSprite.Physics.TypeBase, AimSprite.Physics.TypeSpecial);
    }

    // 데미지 타입: 에너지
    public void Set_EnergyType()
    {
        Set_DmgType(AimSprite.Energy.TypeBase, AimSprite.Energy.TypeSpecial);
    }

    // 공격 타입: On / Off (화살표)
    public void Set_AttackState(bool _OnOff)
    {
        Set_ActiveSprite(AimSR.TypeSpecial.gameObject, _OnOff);
    }

    // 스킬 타입: On / Off (사용 스킬의 아이콘)
    public void Set_SkillState(int _Index, bool _OnOff)
    {
        Set_ActiveSprite(SkillAimList[_Index].gameObject, _OnOff);
    }


    /* Module */

    // 데미지 타입에 따른 이미지 변경
    private void Set_DmgType(Sprite _AimSprite, Sprite _ShootMarkSprite)
    {
        AimSR.TypeBase.sprite = _AimSprite;
        AimSR.TypeSpecial.sprite = _ShootMarkSprite;
    }

    // 스프라이트 오브젝트 끄고 키기
    private void Set_ActiveSprite(GameObject _GO, bool _OnOff)
    {
        _GO.gameObject.SetActive(_OnOff);
    }

    #endregion
}
