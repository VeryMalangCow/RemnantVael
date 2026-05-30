using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleProdUIController : UIController
{
    #region Value

    [Space(10)]
    [SerializeField] private CanvasGroup battleProd_Cg;

    [Space(10)]
    [SerializeField] private RectTransform battleProd_PlayerRt;
    [SerializeField] private Image battleProd_PlayerImg;
    [SerializeField] private TMP_Text battleProd_PlayerNameTxt;

    [Space(10)]
    [SerializeField] private RectTransform battleProd_EnemyRt;
    [SerializeField] private Image battleProd_EnemyImg;
    [SerializeField] private TMP_Text battleProd_EnemyNameTxt;

    #endregion

    #region BattleProd

    public override void Offset(Camera camera)
    {
        base.Offset(camera); 

        Reset_BattleProd();
    }

    private void Reset_BattleProd()
    {
        battleProd_Cg.gameObject.SetActive(false);
        battleProd_Cg.alpha = 0;
        battleProd_PlayerRt.anchoredPosition = new Vector2(-100, 150);
        battleProd_EnemyRt.anchoredPosition = new Vector2(100, -150);
    }

    public void Play_BattleOnProd(PlayerController player, EliteEnemyController enemy, out float durTime)
    {
        SoundManager.instance.Play_2D_SFX_UI("StartBattleProd");
        Play_BattleOnProd(
            player.battleProdSprite, 
            enemy.battleProdSprite, 
            ResourceManager.instance.Get_PlayerName(player.GetNameID),
            ResourceManager.instance.Get_EnemyName(enemy.GetNameID),
            out durTime);
    }

    public void Play_BattleOnProd(PlayerController player, BossEnemyController enemy, out float durTime)
    {
        SoundManager.instance.Play_2D_SFX_UI("StartBossBattleProd");
        Play_BattleOnProd(
            player.battleProdSprite,
            enemy.battleProdSprite,
            ResourceManager.instance.Get_PlayerName(player.GetNameID),
            ResourceManager.instance.Get_EnemyName(enemy.GetNameID),
            out durTime);
    }

    private void Play_BattleOnProd(Sprite playerImg, Sprite enemyImg, string playerName, string enemyName, out float durTime)
    {
        battleProd_Cg.gameObject.SetActive(true);

        battleProd_PlayerImg.sprite = playerImg;
        battleProd_PlayerImg.SetNativeSize();
        battleProd_PlayerNameTxt.text = playerName;

        battleProd_EnemyImg.sprite = enemyImg;
        battleProd_EnemyImg.SetNativeSize();
        battleProd_EnemyNameTxt.text = enemyName;

        Sequence seq = DOTween.Sequence();

        float time0 = 0.2f;
        seq.Append(battleProd_Cg.DOFade(1f, time0));
        float time1 = 1f;
        seq.Append(battleProd_PlayerRt.DOAnchorPos(new Vector2(650f, 150f), time1).SetEase(Ease.Linear));
        seq.Join(battleProd_EnemyRt.DOAnchorPos(new Vector2(-650f, -150f), time1).SetEase(Ease.Linear));
        float time2 = 3f;
        seq.Append(battleProd_PlayerRt.DOAnchorPosX(700, time2).SetEase(Ease.OutQuad));
        seq.Join(battleProd_EnemyRt.DOAnchorPosX(-700, time2).SetEase(Ease.OutQuad));

        durTime = time0 + time1 + time2;
    }

    public void Play_BattleOffProd(out float durTime)
    {
        durTime = 0.25f;
        battleProd_Cg.DOFade(0f, durTime)
            .OnComplete(() => Reset_BattleProd())
            .SetUpdate(true);
    }

    #endregion
}
