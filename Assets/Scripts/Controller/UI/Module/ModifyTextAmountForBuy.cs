using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ModifyTextAmountForBuy : UIModule
{
    #region Value

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private RectTransform ThisRT;

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private Vector2 MaximumSize = new Vector2(1350, 225);
    [SerializeField] private Vector2 MinimumSize = new Vector2(300, 225);
    [SerializeField] private float SizeDeltaTime = 0.2f;

    [Space(10)]
    [Header("=== Always Component")]
    [SerializeField] private Button AlwaysPanelBtn;
    [SerializeField] private TMP_Text SkillNameTxt;
    [SerializeField] private TMP_Text SkillLvTxt;
    [SerializeField] private Image SkillIconImg;
    [SerializeField] private Image UpgradeIconImg;

    #endregion

    #region Offset

    public override void Offset()
    {
        ThisRT.sizeDelta = MinimumSize;

        AlwaysPanelBtn.OnClickAsObservable()
            .Subscribe(_ =>
            {
                if (DOTween.IsTweening(ThisRT)) { return; }

                if (ThisRT.sizeDelta == MaximumSize)
                {
                    ThisRT.DOSizeDelta(MinimumSize, SizeDeltaTime);
                }
                else
                {
                    ThisRT.DOSizeDelta(MaximumSize, SizeDeltaTime);
                }
            });
    }

    #endregion

    #region Unique

    public void Open()
    {
        Debug.Log(gameObject.name + ": Open");
    }

    public void Close()
    {
        Debug.Log(gameObject.name + ": Close");
    }

    #endregion
}
