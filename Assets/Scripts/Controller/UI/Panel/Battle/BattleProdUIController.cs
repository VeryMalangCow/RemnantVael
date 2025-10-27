using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleProdUIController : UIController
{
    #region Value

    [Space(10)]
    [SerializeField] private CanvasGroup BattleProd_CG;

    [Space(10)]
    [SerializeField] private RectTransform BattleProd_PlayerRT;
    [SerializeField] private Image BattleProd_PlayerImg;
    [SerializeField] private TMP_Text BattleProd_PlayerNameTxt;

    [Space(10)]
    [SerializeField] private RectTransform BattleProd_EnemyRT;
    [SerializeField] private Image BattleProd_EnemyImg;
    [SerializeField] private TMP_Text BattleProd_EnemyNameTxt;

    #endregion

    #region BattleProd

    public override void Offset()
    {
        base.Offset(); 

        Reset_BattleProd();
    }

    private void Reset_BattleProd()
    {
        BattleProd_CG.gameObject.SetActive(false);
        BattleProd_CG.alpha = 0;
        BattleProd_PlayerRT.anchoredPosition = new Vector2(-100, 150);
        BattleProd_EnemyRT.anchoredPosition = new Vector2(100, -150);
    }

    public void Play_BattleOnProd(PlayerController _Player, EliteEnemyController _Enemy, out float _DurTime)
    {
        SoundManager.Instance.Play_2D_SFX_UI("StartBattleProd");
        Play_BattleOnProd(
            _Player.BattleProdSprite, 
            _Enemy.BattleProdSprite, 
            ResourceManager.Instance.Get_PlayerName(_Player.GetNameID),
            ResourceManager.Instance.Get_EnemyName(_Enemy.GetNameID),
            out _DurTime);
    }

    public void Play_BattleOnProd(PlayerController _Player, BossEnemyController _Enemy, out float _DurTime)
    {
        SoundManager.Instance.Play_2D_SFX_UI("StartBossBattleProd");
        Play_BattleOnProd(
            _Player.BattleProdSprite,
            _Enemy.BattleProdSprite,
            ResourceManager.Instance.Get_PlayerName(_Player.GetNameID),
            ResourceManager.Instance.Get_EnemyName(_Enemy.GetNameID),
            out _DurTime);
    }

    private void Play_BattleOnProd(Sprite _PlayerImg, Sprite _EnemyImg, string _PlayerName, string _EnemyName, out float _DurTime)
    {
        BattleProd_CG.gameObject.SetActive(true);

        BattleProd_PlayerImg.sprite = _PlayerImg;
        BattleProd_PlayerImg.SetNativeSize();
        BattleProd_PlayerNameTxt.text = _PlayerName;

        BattleProd_EnemyImg.sprite = _EnemyImg;
        BattleProd_EnemyImg.SetNativeSize();
        BattleProd_EnemyNameTxt.text = _EnemyName;

        Sequence seq = DOTween.Sequence();

        float time0 = 0.2f;
        seq.Append(BattleProd_CG.DOFade(1f, time0));
        float time1 = 1f;
        seq.Append(BattleProd_PlayerRT.DOAnchorPos(new Vector2(650f, 150f), time1).SetEase(Ease.Linear));
        seq.Join(BattleProd_EnemyRT.DOAnchorPos(new Vector2(-650f, -150f), time1).SetEase(Ease.Linear));
        float time2 = 3f;
        seq.Append(BattleProd_PlayerRT.DOAnchorPosX(700, time2).SetEase(Ease.OutQuad));
        seq.Join(BattleProd_EnemyRT.DOAnchorPosX(-700, time2).SetEase(Ease.OutQuad));

        _DurTime = time0 + time1 + time2;
    }

    public void Play_BattleOffProd(out float _DurTime)
    {
        _DurTime = 0.25f;
        BattleProd_CG.DOFade(0f, _DurTime)
            .OnComplete(() => Reset_BattleProd())
            .SetUpdate(true);
    }

    #endregion
}
