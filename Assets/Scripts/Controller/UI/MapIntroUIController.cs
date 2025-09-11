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
    [SerializeField] private RectTransform ThisMovingRT;
    [SerializeField] private RectTransform ThisShakingRT;

    [Space(5)]
    [Header("-- Txt")]
    [SerializeField] private TMP_Text MapNameTxt;
    [SerializeField] private TMP_Text MapDescriptionTxt;

    #endregion

    #region Usable

    public void Play_IntroLabel()
    {
        Set_LanguageTxt();

        Play_Label(_DownTime: 1.5f, _StayTime: 2.5f, _UpTime: 2f)
            .OnStart(() => { this.gameObject.SetActive(true); })
            .OnComplete(() => { this.gameObject.SetActive(false); });
    }

    private void Set_Txt(string _Name, string _Desc)
    {
        MapNameTxt.text = _Name;
        MapDescriptionTxt.text = _Desc;
    }

    private Sequence Play_Label(float _DownTime, float _StayTime, float _UpTime)
    {
        DevTool.Set_KillTween(ThisMovingRT);
        Sequence seq = DOTween.Sequence();

        seq.Append(ThisMovingRT.DOAnchorPos(new Vector2(0, -ThisMovingRT.rect.height), _DownTime).SetEase(Ease.OutCubic));
        seq.Join(ThisShakingRT.DOShakeAnchorPos(_DownTime * 1.5f, 1f, 50, 90, false, true));

        seq.AppendInterval(_StayTime);

        seq.Append(ThisMovingRT.DOAnchorPos(new Vector2(0, 0), _UpTime).SetEase(Ease.InCubic));
        seq.Join(ThisShakingRT.DOShakeAnchorPos(_UpTime * 1.5f, 1f, 50, 90, false, true).SetEase(Ease.InCubic));

        return seq;
    }

    #endregion

    #region Set (Language)

    public override void Set_LanguageTxt()
    {
        base.Set_LanguageTxt();

        StageData sd =
            StageManager.Instance.Get_CurrentStageData();

        Set_Txt(
            ResourceManager.Instance.Get_MapName(sd.InfoData.StageID),
            ResourceManager.Instance.Get_MapDesc(sd.InfoData.StageID));
    }

    #endregion
}
