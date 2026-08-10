using UnityEngine;

public class EnemyMeleeModule : EnemyModule
{
    [Header("=== Attack Pointer")]
    [SerializeField] private DepthController[] attackDepths;

    [Header("=== WeaponScalePresenter")]
    [SerializeField] private SpriteRenderer[] weaponSpriteRenderers;
    [SerializeField] private Color weaponVfxClr = Color.black;
    [SerializeField] private float weaponVfxSize = 1.5f;

    public EnemyAttackPointer attackPointer { get; private set; }
    public EnemyWeaponScalePresenter weaponScalePresenter { get; private set; }

    private void Awake()
    {
        attackPointer = new EnemyAttackPointer(
            attackDepths);
        weaponScalePresenter = new EnemyWeaponScalePresenter(
            weaponSpriteRenderers, weaponVfxClr, weaponVfxSize);
    }
}
