using UnityEngine;
using UniRx;
using System.Collections.Generic;

public class DirectionalImgController : DirectionalController<Sprite, SpriteRenderer>
{
    #region Framework

    protected override void Start()
    {
        base.Start();

        currentIndex.Subscribe(index =>
        {
            comp.sprite = dirList[index];
        });
    }

    #endregion

    public void SetDir(List<Sprite> sprites, Material material)
    {
        dirList = sprites;
        comp.material = material;
    }
}
