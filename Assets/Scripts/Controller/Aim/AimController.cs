using System.Collections.Generic;
using UnityEngine;

public class AimController : StaticDepthController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Aim")]
    [SerializeField] private float AimFollowSpeed = 22f;

    [Space(10)]
    [Header("=== Aim")]
    [SerializeField] private SpriteRenderer Aim;
    [SerializeField] private Sprite AimP;
    [SerializeField] private Sprite AimE;

    [Space(10)]
    [Header("=== Shoot")]
    [SerializeField] private SpriteRenderer ShootAim;
    [SerializeField] private Sprite ShootAimP;
    [SerializeField] private Sprite ShootAimE;

    [Space(10)]
    [Header("=== Skill")]
    [SerializeField] private List<SpriteRenderer> SkillAimList;

    #endregion

    #region Framework

    private void Start()
    {
        for (int i = 0; i < SkillAimList.Count; i++)
        {
            SkillAimList[i].gameObject.SetActive(false);
        }
    }

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
        Quaternion fromRot = this.transform.localRotation;
        Quaternion toRot = Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, InputManager.Instance.DirFromPlayerPos));

        TargetObject.transform.localRotation = 
            Quaternion.Slerp(fromRot, toRot, AimFollowSpeed * _DeltaTime);
    }

    // 데미지 타입: 물리
    public void Set_PhysicsType()
    {
        Set_DmgType(AimP, ShootAimP);
    }

    // 데미지 타입: 에너지
    public void Set_EnergyType()
    {
        Set_DmgType(AimE, ShootAimE);
    }

    // 공격 타입: On / Off (화살표)
    public void Set_AttackState(bool _OnOff)
    {
        Set_ActiveSprite(ShootAim.gameObject, _OnOff);
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
        Aim.sprite = _AimSprite;
        ShootAim.sprite = _ShootMarkSprite;
    }

    // 스프라이트 오브젝트 끄고 키기
    private void Set_ActiveSprite(GameObject _GO, bool _OnOff)
    {
        if (_GO.gameObject.activeSelf != _OnOff)
        {
            _GO.gameObject.SetActive(_OnOff);
        }
    }

    #endregion
}
