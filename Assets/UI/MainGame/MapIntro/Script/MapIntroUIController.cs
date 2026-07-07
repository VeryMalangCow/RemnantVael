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

    [Space(5)]
    [Header("-- Txt")]
    [SerializeField] private TMP_Text mapNameTxt;
    [SerializeField] private TMP_Text mapDescTxt;

    #endregion

    #region Usable

    public void Play_IntroLabel()
    {
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

        seq.AppendInterval(stayTime);

        seq.Append(movingRt.DOAnchorPos(new Vector2(0, 0), upTime).SetEase(Ease.InCubic));

        return seq;
    }

    #endregion

    #region Set (Language)

    public override void SetLanguageTxt()
    {
        base.SetLanguageTxt();

        var data = StageManager.instance.stageObjectGenerator;
        Set_Txt(data.stageNames[GameManager.languageID], data.stageDescs[GameManager.languageID]);
    }

    #endregion
}
