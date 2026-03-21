using UnityEngine;
using UniRx;

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

}
