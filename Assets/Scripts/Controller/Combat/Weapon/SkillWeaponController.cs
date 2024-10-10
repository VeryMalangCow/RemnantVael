using System.Collections.Generic;
using UnityEngine;

public class SkillWeaponController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player")]

    [Space(10)]
    [Header("=== Player")]
    [SerializeField] private PlayerController PlayerController;
    [SerializeField] private SpriteRenderer PlayerSR;

    [Space(10)]
    [Header("=== Input")]
    [SerializeField] public bool IsInputed = false;

    [Space(10)]
    [Header("=== Hand Things")]
    [Header("-- Roll")]
    [SerializeField] private float DefualtRoll = -85f;
    [SerializeField] private Transform RollTF;

    [Header("-- Pitch")]
    [SerializeField] private float rotateSpeed = 4f;
    [SerializeField] private Transform PitchTF;

    [Header("-- Hand")]
    [SerializeField] private List<Satellite> Hands;

    [Header("-- ActiveSkill")]
    [SerializeField] public ActiveSkillController Skill_0;
    [SerializeField] public ActiveSkillController Skill_1;

    #endregion

    #region Framework

    private void Start()
    {
        if (Hands[0].ObjectTF.gameObject.TryGetComponent(out ActiveSkillController ASC_Q))
        { Skill_0 = ASC_Q; }
        if (Hands[1].ObjectTF.gameObject.TryGetComponent(out ActiveSkillController ASC_E))
        { Skill_1 = ASC_E; }
    }

    private void Update()
    {
        RotateSmooth();

        foreach (Satellite hand in Hands)
        {
            hand.SetPosOffset();
            hand.SetSortOrder(PlayerSR.sortingOrder);
        }
    }

    private void OnEnable()
    {
        RollTF.rotation = Quaternion.Euler(DefualtRoll, 0f, 0f);
    }

    #endregion

    #region Rotate

    private void RotateSmooth()
    {
        Vector2 dir = InputManager.Instance.DirFromPlayerPos.normalized;
        Quaternion targetQuat = Quaternion.Euler(0f, -Vector2.SignedAngle(Vector2.up, dir), 0f);
        targetQuat = Quaternion.Slerp(PitchTF.transform.localRotation, targetQuat, rotateSpeed * Time.deltaTime);

        PitchTF.transform.localRotation = targetQuat;
    }

    #endregion
}
