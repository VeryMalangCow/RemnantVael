using DG.Tweening;
using UnityEngine;

public class TitleLobbyUIManager : Singleton<TitleLobbyUIManager>
{
    #region Value

    [Header("=== UI_Camera")]
    [SerializeField] private Camera UICamera;

    [Header("=== UI_Prefab")]
    [SerializeField] private Transform UIParent;
    [SerializeField] private GameObject TitleLobby_CanvasPrefab;
    [SerializeField] private GameObject ChoiceCharacter_CanvasPrefab;
    [SerializeField] private GameObject EntranceSpace_CanvasPrefab;

    // Controller
    [HideInInspector] public TitleLobbyUIController TitleLobby_UIController;
    [HideInInspector] public ChoiceCharacterUIController ChoiceCharacter_UIController;
    [HideInInspector] public EntranceSpaceUIController EntranceSpace_UIController;

    [Header("=== Screen")]
    [SerializeField] private Canvas ScreenCanvas;
    [HideInInspector] private CanvasGroup ScreenCG;

    [SerializeField] private CanvasGroup WarningCG;
    [SerializeField] private CanvasGroup SimpleCreditCG;

    [SerializeField] private float EachFadeInTime = 1f;
    [SerializeField] private float EachFadeOutTime = 1f;
    [SerializeField] private float EachStayTime = 1.5f;
    [SerializeField] private float EachDelayTime = 0.5f;

    [Header("=== Map")]
    [SerializeField] private Transform MapObjectParentTF;


    #endregion

    #region Framework

    private void Start()
    {
        TitleLobby_UIController 
            = SpawnUI<TitleLobbyUIController>(TitleLobby_CanvasPrefab, true);

        ChoiceCharacter_UIController
            = SpawnUI<ChoiceCharacterUIController>(ChoiceCharacter_CanvasPrefab, false);

        EntranceSpace_UIController
            = SpawnUI<EntranceSpaceUIController>(EntranceSpace_CanvasPrefab, false);

        FirstStart();
    }

    #endregion

    #region Spawn

    private T SpawnUI<T>(GameObject _UIGO, bool _OnOff)
    {
        GameObject uigo = Instantiate(_UIGO, UIParent);
        uigo.gameObject.SetActive(_OnOff);
        if (uigo.TryGetComponent(out UIController ui))
        {
            ui.Offset_Main();
        }
        if (uigo.TryGetComponent(out Canvas canvas))
        {
            canvas.worldCamera = UICamera;
        }

        if (uigo.TryGetComponent(out T spawnUI))
        { return spawnUI; }
        else
        { return default; }
    }



    #endregion

    #region FirstStart

    private void FirstStart()
    {
        if (ScreenCG == null && ScreenCanvas.TryGetComponent(out CanvasGroup CG))
        {
            ScreenCG = CG;
        }

        if (!GameManager.Instance.WasWatched)
        {
            ScreenCG.alpha = 1f;
            WarningCG.alpha = 0f;
            SimpleCreditCG.alpha = 0f;

            WarningSeq()
            .OnComplete(() =>
            {
                // Loading Map
                MapObjectParentTF.gameObject.SetActive(true);

                SimpleCreditSeq()
                .OnComplete(() =>
                {
                    FadeOut();
                });
            });
        }
        else
        {
            ScreenCG.alpha = 0f;
            WarningCG.alpha = 0f;
            SimpleCreditCG.alpha = 1f;

            FadeOut();
        }
        

    }

    private Sequence WarningSeq()
    {
        Sequence warningSeq = DOTween.Sequence();
        warningSeq.Append(WarningCG.DOFade(1f, EachFadeInTime));
        warningSeq.AppendInterval(EachStayTime);
        warningSeq.Append(WarningCG.DOFade(0f, EachFadeOutTime));
        warningSeq.AppendInterval(EachDelayTime);
        warningSeq
            .SetId("WarningSeq")
            .OnUpdate(() =>
            {
                if (Input.anyKeyDown)
                {
                    DOTween.Complete("WarningSeq");
                }
            });

        return warningSeq;
    }

    private Sequence SimpleCreditSeq()
    {
        Sequence simpleCreditSeq = DOTween.Sequence();
        simpleCreditSeq.Append(SimpleCreditCG.DOFade(1f, EachFadeInTime));
        simpleCreditSeq.AppendInterval(EachStayTime);
        simpleCreditSeq.Append(SimpleCreditCG.DOFade(0f, EachFadeOutTime));
        simpleCreditSeq.AppendInterval(EachDelayTime);
        simpleCreditSeq
            .SetId("SimpleCreditSeq")
            .OnUpdate(() =>
            {
                if (Input.anyKeyDown)
                {
                    DOTween.Complete("SimpleCreditSeq");
                }
            });

        return simpleCreditSeq;
    }

    private Sequence FadeOut()
    {
        Sequence firstSeq = DOTween.Sequence();
        firstSeq.Append(ScreenCG.DOFade(0f, EachFadeOutTime));
        firstSeq
            .OnComplete(() =>
            {
                ScreenCanvas.gameObject.SetActive(false);
                GameManager.Instance.WasWatched = true;
                InputTitleManager.Instance.OnEnableInput();
            });

        return firstSeq;
    }

    #endregion
}
