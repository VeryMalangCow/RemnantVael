using DG.Tweening;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Unit Manager")]

    [Space(10)]
    [Header("=== Material")]
    [SerializeField] public Material ModuleM_000_Explosion;
    [SerializeField] public Material ModuleM_000_Hitted;
    [SerializeField] public Material EnemyM_000_Explosion;

    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] public Sprite BuildingDurFrame;
    [SerializeField] public Sprite BuildingDurInner;

    [Space(10)]
    [Header("=== Color")]
    [SerializeField] public Color RandomColor = Color.red;
    [HideInInspector] private Sequence RandomColorSetSeq;

    [Space(10)]
    [Header("=== Generator")]

    [Space(5)]
    [Header("-- Explosion")]
    [SerializeField] public PlayerExplImgGenerator Player_ExplImgGenerator;
    [SerializeField] public BuildExplImgGenerator Build_ExplImgGenerator;
    [SerializeField] public EnemyExplImgGenerator Enemy_ExplImgGenerator;

    [Space(5)]
    [Header("-- Anim")]
    [SerializeField] public OnceTimeAnimGenerator OnceTime_AnimGenerator;



    #endregion

    #region Framework

    protected override void Awake()
    {
        base.Awake();

        Set_RainbowColorDotween();
    }

    #endregion

    #region Generate Unit

    public static T Gen_Unit<T>(GameObject _GO, Transform _ParentTF)
    {
        GameObject SpawnedPlayerGO = Instantiate(_GO, _ParentTF);
        SpawnedPlayerGO.TryGetComponent(out T type);
        return type;
    }

    #endregion


    #region Set

    // 무지개 컬러 Dotween
    private void Set_RainbowColorDotween()
    {
        RandomColorSetSeq = DOTween.Sequence();
        RandomColor = Color.red;

        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(1, 1, 0, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(0, 1, 0, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(0, 1, 1, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(0, 0, 1, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(1, 0, 1, 1), 0.5f).SetEase(Ease.Linear));
        RandomColorSetSeq.Append(DOTween.To(() => RandomColor, x => RandomColor = x, new Color(1, 0, 0, 1), 0.5f).SetEase(Ease.Linear));

        RandomColorSetSeq.SetLoops(-1, LoopType.Restart);
    }

    #endregion
}
