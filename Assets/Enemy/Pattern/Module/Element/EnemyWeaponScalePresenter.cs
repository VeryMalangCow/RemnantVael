using DG.Tweening;
using UnityEngine;

public class EnemyWeaponScalePresenter
{
    private SpriteRenderer[] spriteRenderers;
    private Color vfxClr;
    private float vfxSize;

    public EnemyWeaponScalePresenter(SpriteRenderer[] srs, Color clr, float size)
    {
        spriteRenderers = srs;
        vfxClr = clr;
        vfxSize = size;
    }

    public void PlayVfxOn(float durTime)
    {
        PlayVfx(vfxClr, vfxSize, durTime * 0.8f);
    }

    public void PlayVfxOff(float durTime)
    {
        PlayVfx(Color.white, 1f, durTime * 0.8f);
    }

    private void PlayVfx(Color clr, float size, float durTime = 1f)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            var sr = spriteRenderers[i];
            sr.DOColor(clr, durTime);
            sr.transform.DOScale(size, durTime);
        }
    }
}
