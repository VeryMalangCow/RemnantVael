using System.Collections.Generic;
using UnityEngine;

public class AimController : HaveShadowThingStatic
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
        gameObject.transform.position = 
            Vector2.Lerp(gameObject.transform.position,
            InputManager.Instance.MousePosByWorld, 
            AimFollowSpeed * Time.deltaTime);

        TargetObject.transform.localRotation =
            Quaternion.Slerp(TargetObject.transform.localRotation,
            Quaternion.Euler(0f, 0f, Vector2.SignedAngle(Vector2.up, InputManager.Instance.DirFromPlayerPos)),
            AimFollowSpeed * Time.deltaTime);
    }

    #endregion

    #region Set 

    public void SetPType()
    {
        Aim.sprite = AimP;
        ShootAim.sprite = ShootAimP;
    }

    public void SetEType()
    {
        Aim.sprite = AimE;
        ShootAim.sprite = ShootAimE;
    }

    public void SetBaseAttack(bool _IsOn)
    {
        if (ShootAim.gameObject.activeSelf != _IsOn)
        {
            ShootAim.gameObject.SetActive(_IsOn);
        }
    }

    public void SetOnSkill(int _Index, bool _IsOn)
    {
        if (SkillAimList[_Index].gameObject.activeSelf != _IsOn)
        {
            SkillAimList[_Index].gameObject.SetActive(_IsOn);
        }
    }

    #endregion
}
