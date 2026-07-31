using UnityEngine;

public class EnemyMeleePresenter : EnemyPresenter
{
    private SpriteRenderer[] spriteRenderers;
    private Color vfxClr;
    private float vfxSize;

    public EnemyMeleePresenter(SpriteRenderer[] srs, Color clr, float size)
    {
        spriteRenderers = srs;
        vfxClr = clr;
        vfxSize = size;
    }

    public void PlayVfxOn()
    {
        PlayVfx(vfxClr, vfxSize);
    }

    public void PlayVfxOff()
    {
        PlayVfx(Color.white, 1f);
    }

    private void PlayVfx(Color clr, float size)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            var sr = spriteRenderers[i];
            sr.color = clr;
            sr.transform.localScale = Vector2.one * size;
        }
    }
}
