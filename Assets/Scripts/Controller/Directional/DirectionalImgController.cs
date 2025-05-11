using UnityEngine;
using UniRx;

public class DirectionalImgController : DirectionalController<Sprite, SpriteRenderer>
{
    #region Framework

    protected override void Start()
    {
        base.Start();

        CurrentIndex.Subscribe(index =>
        {
            ThisComp.sprite = ThisDirectionalList[index];
        });
    }

    #endregion

}
