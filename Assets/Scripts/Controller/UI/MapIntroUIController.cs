using DG.Tweening;
using TMPro;
using UnityEngine;

public class MapIntroUIController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Map Intro")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private RectTransform ThisMovingRT;
    [SerializeField] private RectTransform ThisShakingRT;
    [SerializeField] private TMP_Text MapNameTxt;
    [SerializeField] private TMP_Text MapDescriptionTxt;

    #endregion

    #region Usable

    public void SetOn_IntroLabel()
    {
        StageManager.StageData sd = StageManager.Instance.Get_CollectStageData(StageManager.Instance.TargetStageID);
        MapNameTxt.text = sd.StageName;
        MapDescriptionTxt.text = sd.StageDescription;

        DOTween.Kill(ThisMovingRT);
        Sequence seq = DOTween.Sequence();

        seq.Append(ThisMovingRT.DOAnchorPos(new Vector2(0, -ThisMovingRT.rect.height), 2f)
            .SetEase(Ease.OutCubic));
        seq.Join(ThisShakingRT.DOShakeAnchorPos(2.2f, 1f, 50, 90, false, true));
        seq.AppendInterval(4f);
        seq.Append(ThisMovingRT.DOAnchorPos(new Vector2(0, 0), 2f)
            .SetEase(Ease.InCubic));
        seq.Join(ThisShakingRT.DOShakeAnchorPos(2f, 1f, 50, 90, false, true)
            .SetEase(Ease.InCubic));

        seq
            .OnStart(() =>
            {
                this.gameObject.SetActive(true);
            })
            .OnComplete(() =>
            {
                this.gameObject.SetActive(false);
            });
    }

    #endregion
}
