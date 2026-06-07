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

    [Space(5)]
    [Header("-- RT")]
    [SerializeField] private RectTransform movingRt;
    [SerializeField] private RectTransform shakingRt;

    [Space(5)]
    [Header("-- Txt")]
    [SerializeField] private TMP_Text mapNameTxt;
    [SerializeField] private TMP_Text mapDescTxt;

    #endregion

    #region Usable

    public void Play_IntroLabel()
    {
        SetLanguageTxt();

        Play_Label(downTime: 1.5f, stayTime: 2.5f, upTime: 2f)
            .OnStart(() => { this.gameObject.SetActive(true); })
            .OnComplete(() => { this.gameObject.SetActive(false); });
    }

    private void Set_Txt(string name, string desc)
    {
        mapNameTxt.text = name;
        mapDescTxt.text = desc;
    }

    private Sequence Play_Label(float downTime, float stayTime, float upTime)
    {
        DevTool.SetKillTween(movingRt);
        Sequence seq = DOTween.Sequence();

        seq.Append(movingRt.DOAnchorPos(new Vector2(0, -movingRt.rect.height), downTime).SetEase(Ease.OutCubic));
        seq.Join(shakingRt.DOShakeAnchorPos(downTime * 1.5f, 1f, 50, 90, false, true));

        seq.AppendInterval(stayTime);

        seq.Append(movingRt.DOAnchorPos(new Vector2(0, 0), upTime).SetEase(Ease.InCubic));
        seq.Join(shakingRt.DOShakeAnchorPos(upTime * 1.5f, 1f, 50, 90, false, true).SetEase(Ease.InCubic));

        return seq;
    }

    #endregion

    #region Set (Language)

    public override void SetLanguageTxt()
    {
        base.SetLanguageTxt();

        StageData sd =
            StageManager.instance.Get_CurrentStageData();

        Set_Txt(
            ResourceManager.instance.Get_MapName(sd.infoData.stageId),
            ResourceManager.instance.Get_MapDesc(sd.infoData.stageId));
    }

    #endregion
}
