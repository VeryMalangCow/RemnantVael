using UnityEngine;

public class EnemyRangeModule : EnemyModule
{
    [SerializeField] private DepthController[] attackDepths;

    [SerializeField] private SpriteRenderer[] waeponSpriteRenderers;
    [SerializeField] private Color weaponVfxClr = Color.black;
    [SerializeField] private float weaponVfxSize = 1.5f;

    public EnemyAttackPointer pointer { get; private set; }
    public EnemyWeaponScalePresenter presenter { get; private set; }

    private void Awake()
    {
        pointer = new EnemyAttackPointer(
            attackDepths);
        presenter = new EnemyWeaponScalePresenter(
            waeponSpriteRenderers, weaponVfxClr, weaponVfxSize);
    }
}
