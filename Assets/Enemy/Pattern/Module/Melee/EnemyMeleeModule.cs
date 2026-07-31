using UnityEngine;

public class EnemyMeleeModule : EnemyModule
{
    [SerializeField] private DepthController[] attackDepths;

    [SerializeField] private SpriteRenderer[] weaponSpriteRenderers;
    [SerializeField] private Color weaponVfxClr = Color.black;
    [SerializeField] private float weaponVfxSize = 1.5f;

    public EnemyMeleePointer pointer { get; private set; }
    public EnemyMeleePresenter presenter { get; private set; }

    private void Awake()
    {
        pointer = new EnemyMeleePointer(attackDepths);
        presenter = new EnemyMeleePresenter(weaponSpriteRenderers, weaponVfxClr, weaponVfxSize);
    }
}
