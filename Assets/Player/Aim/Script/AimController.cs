using UnityEngine;
using System.Collections.Generic;


public class AimController : StaticDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Aim")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private float aimFollowSpeed = 30f;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private SpriteRenderer centerSR;
    [SerializeField] private CoupleData<SpriteRenderer> aimSR;
    [SerializeField] private PlayerVisual<Sprite> aimSprite;

    [Space(10)]
    [Header("=== Skill")]
    [SerializeField] private List<SpriteRenderer> skillAimList;

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
        DevTool.Set_ListDele(skillAimList, new Dele_T<SpriteRenderer>(sprite => sprite.gameObject.SetActive(false)));
    }

    private void Offset_Sorting()
    {
        centerSR.sortingOrder = SortingOrderManager.order_Aim;
        aimSR.typeBase.sortingOrder = SortingOrderManager.order_Aim;
        aimSR.typeSpecial.sortingOrder = SortingOrderManager.order_Aim;
        for (int i = 0; i < skillAimList.Count; i++) skillAimList[i].sortingOrder = SortingOrderManager.order_Aim;
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
    private void Update_AimPos(float deltaTime)
    {
        gameObject.transform.position = Vector2.Lerp(
            this.transform.position, 
            InputManager.instance.mousePosByWorld, 
            aimFollowSpeed * deltaTime);
    }

    // 회전 값 업데이트
    private void Update_AimRot(float deltaTime)
    {
        targetObject.transform.localRotation = Quaternion.Slerp(
            targetObject.transform.localRotation,
            DevTool.GetRotFromDir(InputManager.instance.dirFromPlayerPos), 
            aimFollowSpeed * deltaTime);
    }

    #endregion

    #region Set 

    // 데미지 타입: 물리
    public void Set_PhysicsType()
    {
        Set_DmgType(aimSprite.physics.typeBase, aimSprite.physics.typeSpecial);
    }

    // 데미지 타입: 에너지
    public void Set_EnergyType()
    {
        Set_DmgType(aimSprite.energy.typeBase, aimSprite.energy.typeSpecial);
    }

    // 데미지 타입만으로 변경
    public void Set_DmgType(DamageType dmgType)
    {
        if (dmgType == DamageType.Physics)
        { Set_PhysicsType(); }
        else
        { Set_EnergyType(); }
    }

    // 공격 타입: On / Off (화살표)
    public void Set_ActivingAttack(bool onOff)
    {
        Set_ActiveSprite(aimSR.typeSpecial.gameObject, onOff);
    }

    // 스킬 타입: On / Off (사용 스킬의 아이콘)
    public void Set_ActivingSkill(int index, bool onOff)
    {
        Set_ActiveSprite(skillAimList[index].gameObject, onOff);
    }

    #endregion

    #region Set Module

    // 데미지 타입에 따른 이미지 변경
    private void Set_DmgType(Sprite aimSprite, Sprite shootMarkSprite)
    {
        aimSR.typeBase.sprite = aimSprite;
        aimSR.typeSpecial.sprite = shootMarkSprite;
    }

    // 스프라이트 오브젝트 끄고 키기
    private void Set_ActiveSprite(GameObject go, bool onOff)
    {
        go.gameObject.SetActive(onOff);
    }

    #endregion
}
