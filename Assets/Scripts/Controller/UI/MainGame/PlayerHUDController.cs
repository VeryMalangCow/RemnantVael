using UnityEngine;
using UniRx;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using LeTai.TrueShadow;

public class PlayerHUDController : UIController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Player HUD")]

    [Space(10)]
    [Header("=== Energy")]
    [SerializeField] private ModifyReductionFocusProgressBar EP;
    [SerializeField] private RectTransform EP_FlowRT;

    [Space(10)]
    [Header("=== Bettery")]
    [Header("-- Current")]
    [SerializeField] public ModifySprite CurrentEmptyBC;

    [Header("-- Bettery (Image Amount Class)")]
    [SerializeField] private ModifyImgAmountAndTxt EmptyBC;
    [SerializeField] private ModifyImgAmountAndTxt FullBC;

    [Space(10)]
    [Header("=== Boost")]
    [SerializeField] private TMP_Text BoostLv;
    [SerializeField] private GameObject[] BoostLightArr;
    [SerializeField] private GameObject[] BoostLightWheelArr;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        EP.Offset();
        CurrentEmptyBC.Offset();
        EmptyBC.Offset();
        FullBC.Offset();
    }

    protected override void Offset_UI()
    {
        PlayerManager.Instance.PlayerController.MaxEP.ActualState
            .Subscribe(_MaxEP =>
            {
                EP.SetMaxFillRT(_MaxEP * 3);

                EP.SetFillImgSmooth(
                    PlayerManager.Instance.PlayerController.CurrentEP.Value,
                    PlayerManager.Instance.PlayerController.MaxEP.ActualState.Value);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentEP
            .Subscribe(_CurrentEP =>
            {
                EP.SetFillImgSmooth(
                    PlayerManager.Instance.PlayerController.CurrentEP.Value,
                    PlayerManager.Instance.PlayerController.MaxEP.ActualState.Value);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBS
            .Subscribe(_CurrentBS =>
            {
                if (PlayerManager.Instance.PlayerController.CurrentBS.Value < PlayerManager.Instance.PlayerController.NeedBS_ForMakeBC)
                {
                    CurrentEmptyBC.Modify_Sprite(_CurrentBS);
                }
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBC
            .Subscribe(_CurrentBC =>
            {
                EmptyBC.SetAmount(_CurrentBC, 0.5f);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentEC
            .Subscribe(_CurrentEC =>
            {
                FullBC.SetAmount(_CurrentEC, 0.5f);
            })
            .AddTo(gameObject);

        PlayerManager.Instance.PlayerController.CurrentBoostLv
            .Subscribe(_BoostLevel =>
            {
                SetTextOfBoost(_BoostLevel);

                SetActiveAmountBoost(BoostLightArr, _BoostLevel);
                SetActiveAmountBoost(BoostLightWheelArr, _BoostLevel);

                SetRollAmountBoost(BoostLightWheelArr, _BoostLevel);
            })
            .AddTo(gameObject);

        EP_FlowRT.DOAnchorPos(new Vector2(1920, 0), 2f, false)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        Transform[] allChildren = this.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if(child.gameObject.TryGetComponent(out TrueShadow ts))
            {
                float sizeDefault = ts.Size * 1.5f;
                DOTween.To(() => ts.Size, x => ts.Size = x, sizeDefault, 1)
                    .SetLoops(-1, LoopType.Yoyo);
            }
        }
    }

    #endregion

    #region Boost

    private void SetTextOfBoost(int _CurrentLv)
    {
        BoostLv.text = _CurrentLv.ToString();
        Color clr = BoostLv.color;
        clr.a = 0.25f * (_CurrentLv + 1);
        BoostLv.color = clr;
        
    }

    private void SetActiveAmountBoost(GameObject[] _Arr, int _CurrentLv)
    {
        for (int i = 0; i < _Arr.Length; i++)
        {
            _Arr[i].TryGetComponent(out Image img);
            if (DOTween.IsTweening(img))
            { DOTween.Kill(img); }

            if (i < _CurrentLv)
            { 
                _Arr[i].SetActive(true);
                img.DOFade(1f, 0.2f);
            }
            else
            { 
                img.DOFade(0f, 0.2f)
                    .OnComplete(() =>
                    {
                        _Arr[i].SetActive(false);
                    });
            }
        }
    }

    private void SetRollAmountBoost(GameObject[] _Arr, int _CurrentLv)
    {
        for (int i = 0; i < _Arr.Length; i++)
        {
            _Arr[i].TryGetComponent(out RectTransform rt);
            
            if (i < _CurrentLv)
            {
                rt.DOLocalRotate(new Vector3(0, 0, 360), 0.2f, RotateMode.LocalAxisAdd)
                    .SetEase(Ease.OutCubic)
                    .OnComplete(() =>
                    {
                        rt.DOLocalRotate(new Vector3(0, 0, 360), 3f / (i + 1f), RotateMode.LocalAxisAdd)
                            .SetEase(Ease.Linear)
                            .SetLoops(-1, LoopType.Restart);
                    });
            }
            else
            {
                DOTween.Kill(rt);
            }
        }
    }

    #endregion
}

