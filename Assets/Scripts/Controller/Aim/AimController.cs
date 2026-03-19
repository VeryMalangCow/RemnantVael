using UnityEngine;
using System.Collections.Generic;


public class AimController : StaticDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Aim")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float AimFollowSpeed = 30f;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private SpriteRenderer CenterSR;
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
        Offset_Sorting();
    }

    private void Offset_SkillAimList()
    {
        DevTool.Set_ListDele(SkillAimList, new Dele_T<SpriteRenderer>(sprite => sprite.gameObject.SetActive(false)));
    }

    private void Offset_Sorting()
    {
        CenterSR.sortingOrder = LayerOrderManager.order_Aim;
        AimSR.typeBase.sortingOrder = LayerOrderManager.order_Aim;
        AimSR.typeSpecial.sortingOrder = LayerOrderManager.order_Aim;
        for (int i = 0; i < SkillAimList.Count; i++) SkillAimList[i].sortingOrder = LayerOrderManager.order_Aim;
    }

    #endregion

    #region Framework


    private void LateUpdate()
    {
        Update_AimPos(Time.deltaTime);
        Update_AimRot(Time.deltaTime);
    }

    #endregion

    #region Update

    // 위치 값 업데이트
    private void Update_AimPos(float _DeltaTime)
    {
        gameObject.transform.position = Vector2.Lerp(
            this.transform.position, 
            InputManager.instance.mousePosByWorld, 
            AimFollowSpeed * _DeltaTime);
    }

    // 회전 값 업데이트
    private void Update_AimRot(float _DeltaTime)
    {
        TargetObject.transform.localRotation = Quaternion.Slerp(
            TargetObject.transform.localRotation,
            DevTool.Get_RotFromDir(InputManager.instance.dirFromPlayerPos), 
            AimFollowSpeed * _DeltaTime);
    }

    #endregion

    #region Set 

    // 데미지 타입: 물리
    public void Set_PhysicsType()
    {
        Set_DmgType(AimSprite.Physics.typeBase, AimSprite.Physics.typeSpecial);
    }

    // 데미지 타입: 에너지
    public void Set_EnergyType()
    {
        Set_DmgType(AimSprite.Energy.typeBase, AimSprite.Energy.typeSpecial);
    }

    // 데미지 타입만으로 변경
    public void Set_DmgType(eDamageType _DmgType)
    {
        if (_DmgType == eDamageType.Physics)
        { Set_PhysicsType(); }
        else
        { Set_EnergyType(); }
    }

    // 공격 타입: On / Off (화살표)
    public void Set_ActivingAttack(bool _OnOff)
    {
        Set_ActiveSprite(AimSR.typeSpecial.gameObject, _OnOff);
    }

    // 스킬 타입: On / Off (사용 스킬의 아이콘)
    public void Set_ActivingSkill(int _Index, bool _OnOff)
    {
        Set_ActiveSprite(SkillAimList[_Index].gameObject, _OnOff);
    }

    #endregion

    #region Set Module

    // 데미지 타입에 따른 이미지 변경
    private void Set_DmgType(Sprite _AimSprite, Sprite _ShootMarkSprite)
    {
        AimSR.typeBase.sprite = _AimSprite;
        AimSR.typeSpecial.sprite = _ShootMarkSprite;
    }

    // 스프라이트 오브젝트 끄고 키기
    private void Set_ActiveSprite(GameObject _GO, bool _OnOff)
    {
        _GO.gameObject.SetActive(_OnOff);
    }

    #endregion
}
