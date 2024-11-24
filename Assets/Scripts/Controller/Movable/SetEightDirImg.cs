using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SetEightDirImg : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Eight Dir Img")]

    [Space(10)]
    [Header("=== TF")]
    [SerializeField] private Transform RotationTargetTF;

    [Space(10)]
    [Header("=== Reso")]
    [SerializeField] private List<Sprite> ThisEightImgList;

    [HideInInspector] private SpriteRenderer ThisSR;
    [HideInInspector] private ReactiveProperty<int> CurrentIndex = new();

    #endregion

    #region Framework

    private void Start()
    {
        CurrentIndex.Value = 5;
        if (TryGetComponent(out SpriteRenderer SR))
        {
            ThisSR = SR;
        }

        CurrentIndex.Subscribe(index =>
        {
            ThisSR.sprite = ThisEightImgList[index];
        });
    }

    private void LateUpdate()
    {
        if (GetIndex(RotationTargetTF.localRotation.eulerAngles.y) != CurrentIndex.Value)
        {
            CurrentIndex.Value = GetIndex(RotationTargetTF.localRotation.eulerAngles.y);
        }
    }

    #endregion

    #region Sprite by Angle

    private int GetIndex(float _EulerAngleY)
    {
        int index = 0;
        float angle = _EulerAngleY + 67.5f;
        angle = angle >= 360 ? angle -= 360 : angle ;

        index = (int)(angle / 45);
        return index;
    }

    #endregion
}
