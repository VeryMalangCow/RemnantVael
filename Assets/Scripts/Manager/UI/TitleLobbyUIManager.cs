using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Device;

public class TitleLobbyUIManager : Singleton<TitleLobbyUIManager>
{
    #region Value

    #region - Inspector

    [Header("=== UI_Camera")]
    [SerializeField] public Camera UICamera;

    [Header("=== UI_Prefab")]
    [SerializeField] private Transform UIParent;

    // Controller
    [HideInInspector] public TitleLobbyUIController TitleLobby_UIController;

    [Header("=== Screen")]
    [SerializeField] private Canvas ScreenCanvas;

    [SerializeField] private CanvasGroup WarningCG;
    [SerializeField] private TMP_Text WarningTitleTxt;
    [SerializeField] private TMP_Text WarningTxt;
    [SerializeField] private TMP_Text WarningExtraTxt;

    [SerializeField] private float EachFadeInTime = 1f;
    [SerializeField] private float EachFadeOutTime = 1f;
    [SerializeField] private float EachStayTime = 3f;
    [SerializeField] private float EachDelayTime = 0.5f;

    #endregion

    #region - Hide

    [HideInInspector] private CanvasGroup ScreenCG;

    #endregion

    #endregion

    #region Framework

    private void Start()
    {
        TitleLobby_UIController 
            = Gen_UI<TitleLobbyUIController>(ResourceManager.Instance.TitleLobby_CanvasPrefab, true);

        GameManager.Instance.Set_BaseOption();

        SetWarningTxt();
        Start_FirstPlay();
    }

    #endregion

    #region Spawn

    private T Gen_UI<T>(GameObject _UIGO, bool _OnOff)
    {
        GameObject uigo = Instantiate(_UIGO, UIParent);
        uigo.gameObject.SetActive(_OnOff);
        if (uigo.TryGetComponent(out UIController ui))
        {
            ui.Offset();
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

    #region Set

    public void Set_LanguageTxt()
    {
        TitleLobby_UIController.Set_LanguageTxt();
    }

    #endregion

    #region FirstStart

    private void SetWarningTxt()
    {
        if (GameManager.LanguageID == 0)
        {
            WarningTitleTxt.text =
                "WARNING";
            WarningTxt.text =
                "This game contains themes involving war, disease, death, and the loss of humanity.\r\n\r\n" +
                "Some scenes and content may cause emotional discomfort for certain players.\r\n\r\n" +
                "This work is a fictional narrative that explores\r\n" +
                "the identity of AI and humanity, personal choice, and ethical conflict.\r\n" +
                "Any resemblance to real individuals, events, or organizations is purely coincidental.\r\n\r\n" +
                "Some sound assets and cutscene images in this game were\r\n" +
                "created with the assistance of AI technologies for production efficiency.\r\n" +
                "All game design, structure, and narrative are original works of the developer.\r\n\r\n" +
                "By continuing, you acknowledge and agree to the above.";

            WarningExtraTxt.text =
                "This game is an unfinished work and may contain unimplemented features or incomplete content.";

        }
        else if (GameManager.LanguageID == 1)
        {
            WarningTitleTxt.text =
                "경고문";

            WarningTxt.text =
                "본 게임은 전쟁, 질병, 사망, 인간의 상실과 같은 무거운 주제를 다루고 있습니다.\r\n\r\n" +
                "일부 장면과 내용은 플레이어에게 심리적 불편함을 줄 수 있습니다.\r\n\r\n" +
                "또한 본 작품은 AI와 인간의 정체성, 선택, 윤리적 갈등을 주제로 하며\r\n" +
                "특정 인물·사건·단체와는 무관한 허구의 이야기입니다.\r\n\r\n" +
                "본 게임의 일부 사운드 및 컷씬 이미지는 제작 효율을 위해 AI 기술을 보조적으로 활용하였으며,\r\n" +
                "게임의 기획·구조·서사는 개발자의 창작물입니다.\r\n\r\n" +
                "게임을 계속 진행함으로써 위 내용을 이해하고 동의한 것으로 간주합니다.";

            WarningExtraTxt.text =
                "본 게임은 미완성 게임으로, 미구현되거나 부족한 부분이 많습니다.";
        }
    }

    private void Start_FirstPlay()
    {
        SoundManager.Instance.Play_2D_BGM_Title();

        // Sound
        SoundManager.Instance.Set_MasterVolume(0f, 1f, 1f);

        if (ScreenCG == null && ScreenCanvas.TryGetComponent(out CanvasGroup CG))
        {
            ScreenCG = CG;
        }

        if (!GameManager.Instance.WasWatched)
        {
            ScreenCG.alpha = 1f;
            WarningCG.alpha = 0f;

            Get_WarningSeq()
            .OnComplete(() =>
            {
                Get_FadeOut();
            });
        }
        else
        {
            ScreenCG.alpha = 0f;
            WarningCG.alpha = 0f;

            Get_FadeOut();
        }
    }

    private Sequence Get_WarningSeq()
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

    private Sequence Get_FadeOut()
    {
        Sequence firstSeq = DOTween.Sequence();
        firstSeq.Append(ScreenCG.DOFade(0f, EachFadeOutTime));
        firstSeq
            .OnComplete(() =>
            {
                ScreenCanvas.gameObject.SetActive(false);
                GameManager.Instance.WasWatched = true;

                TitleLobby_UIController.IsInIntro = false;
            });

        return firstSeq;
    }

    public Sequence Get_JustFadeIn(float _DurTime)
    {
        WarningCG.alpha = 0;
        ScreenCG.alpha = 0;

        ScreenCanvas.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(ScreenCG.DOFade(1f, _DurTime));

        return seq;
    }

    #endregion
}
