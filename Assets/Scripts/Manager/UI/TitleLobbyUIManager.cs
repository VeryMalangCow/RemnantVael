using DG.Tweening;
using TMPro;
using UnityEngine;

public class TitleLobbyUIManager : Singleton<TitleLobbyUIManager>
{
    #region Value

    #region - Inspector

    [Header("=== UI_Camera")]
    [SerializeField] public Camera uiCamera;

    [Header("=== UI_Prefab")]
    [SerializeField] private Transform uiParent;

    // Controller
    [HideInInspector] public TitleLobbyUIController titleLobby_UIController;

    [Header("=== Screen")]
    [SerializeField] private Canvas screenCanvas;

    [SerializeField] private CanvasGroup warningCG;
    [SerializeField] private TMP_Text warningTitleTxt;
    [SerializeField] private TMP_Text warningTxt;
    [SerializeField] private TMP_Text warningExtraTxt;

    [SerializeField] private float eachFadeInTime = 1f;
    [SerializeField] private float eachFadeOutTime = 1f;
    [SerializeField] private float eachStayTime = 1.5f;
    [SerializeField] private float eachDelayTime = 0.5f;

    #endregion

    #region - Hide

    [HideInInspector] private CanvasGroup screenCG;

    #endregion

    #endregion

    #region Framework

    private void Start()
    {
        titleLobby_UIController 
            = Gen_UI<TitleLobbyUIController>(ResourceManager.instance.titleLobby_CanvasPrefab, true);

        GameManager.instance.Set_BaseOption();

        SetWarningTxt();
        Start_FirstPlay();
    }

    #endregion

    #region Spawn

    private T Gen_UI<T>(GameObject uiGo, bool onOff)
    {
        GameObject uigo = Instantiate(uiGo, uiParent);
        uigo.gameObject.SetActive(onOff);
        if (uigo.TryGetComponent(out UIController ui))
        {
            ui.Offset(uiCamera);
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
        titleLobby_UIController.SetLanguageTxt();
    }

    #endregion

    #region FirstStart

    private void SetWarningTxt()
    {
        if (GameManager.languageID == 0)
        {
            warningTitleTxt.text =
                "WARNING";
            warningTxt.text =
                "This game contains themes involving war, disease, death, and the loss of humanity.\r\n\r\n" +
                "Some scenes and content may cause emotional discomfort for certain players.\r\n\r\n" +
                "This work is a fictional narrative that explores\r\n" +
                "the identity of AI and humanity, personal choice, and ethical conflict.\r\n" +
                "Any resemblance to real individuals, events, or organizations is purely coincidental.\r\n\r\n" +
                "Some sound assets and cutscene images in this game were\r\n" +
                "created with the assistance of AI technologies for production efficiency.\r\n" +
                "All game design, structure, and narrative are original works of the developer.\r\n\r\n" +
                "By continuing, you acknowledge and agree to the above.";

            warningExtraTxt.text =
                "This game is an unfinished work and may contain unimplemented features or incomplete content.";

        }
        else if (GameManager.languageID == 1)
        {
            warningTitleTxt.text =
                "경고문";

            warningTxt.text =
                "본 게임은 전쟁, 질병, 사망, 인간의 상실과 같은 무거운 주제를 다루고 있습니다.\r\n\r\n" +
                "일부 장면과 내용은 플레이어에게 심리적 불편함을 줄 수 있습니다.\r\n\r\n" +
                "또한 본 작품은 AI와 인간의 정체성, 선택, 윤리적 갈등을 주제로 하며\r\n" +
                "특정 인물·사건·단체와는 무관한 허구의 이야기입니다.\r\n\r\n" +
                "본 게임의 일부 사운드 및 컷씬 이미지는 제작 효율을 위해 AI 기술을 보조적으로 활용하였으며,\r\n" +
                "게임의 기획·구조·서사는 개발자의 창작물입니다.\r\n\r\n" +
                "게임을 계속 진행함으로써 위 내용을 이해하고 동의한 것으로 간주합니다.";

            warningExtraTxt.text =
                "본 게임은 미완성 게임으로, 미구현되거나 부족한 부분이 많습니다.";
        }
    }

    private void Start_FirstPlay()
    {
        SoundManager.instance.Play_2D_BGM_Title();

        // Sound
        SoundManager.instance.Set_MasterVolume(0f, 1f, 1f);

        if (screenCG == null && screenCanvas.TryGetComponent(out CanvasGroup CG))
        {
            screenCG = CG;
        }

        if (!GameManager.instance.wasWatched)
        {
            screenCG.alpha = 1f;
            warningCG.alpha = 0f;

            Get_WarningSeq()
            .OnComplete(() =>
            {
                Get_FadeOut();
            });
        }
        else
        {
            screenCG.alpha = 0f;
            warningCG.alpha = 0f;

            Get_FadeOut();
        }
    }

    private Sequence Get_WarningSeq()
    {
        Sequence warningSeq = DOTween.Sequence();
        warningSeq.Append(warningCG.DOFade(1f, eachFadeInTime));
        warningSeq.AppendInterval(eachStayTime);
        warningSeq.Append(warningCG.DOFade(0f, eachFadeOutTime));
        warningSeq.AppendInterval(eachDelayTime);
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
        firstSeq.Append(screenCG.DOFade(0f, eachFadeOutTime));
        firstSeq
            .OnComplete(() =>
            {
                screenCanvas.gameObject.SetActive(false);
                GameManager.instance.wasWatched = true;

                titleLobby_UIController.isInIntro = false;
            });

        return firstSeq;
    }

    public Sequence Get_JustFadeIn(float _DurTime)
    {
        warningCG.alpha = 0;
        screenCG.alpha = 0;

        screenCanvas.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence();
        seq.Append(screenCG.DOFade(1f, _DurTime));

        return seq;
    }

    #endregion
}
