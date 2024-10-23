using UniRx;
using UnityEngine;

public class BuildingController_OnlyPlayerLayer : HaveShadowThingStatic
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Building")]

    [Space(10)]
    [Header("=== Layer")]
    [SerializeField] private SpriteRenderer TargetSR;
    [SerializeField] private SpriteRenderer ShadowSR;
    [SerializeField] private int LowestLayerOrder = -2000;
    [SerializeField] private int HighestLayerOrder = 2000;

    [SerializeField] private ReactiveProperty<bool> IsUpper = new();
    [HideInInspector] private Transform PlayerTF;

    [Space(10)]
    [Header("=== Effect")]
    [SerializeField] public MakeExplosionImage MEI;

    #endregion

    #region Framework

    private void Awake()
    {
        IsUpper.Value = false;
        IsUpper.Subscribe(isUp =>
        {
            SetLayerOrder();
        });
    }

    private void Start()
    {
        if (PlayerTF == null)
        {
            if (PlayerManager.Instance != null)
            {
                PlayerTF = PlayerManager.Instance.PlayerController.gameObject.transform;
            }
            else if (TitlePlayerManager.Instance != null)
            {
                PlayerTF = TitlePlayerManager.Instance.PlayerController.gameObject.transform;
            }
        }
    }


    private void Update()
    {
        SetLayerSort();
    }

    private void SetLayerOrder()
    {
        if (IsUpper.Value)
        {
            TargetSR.sortingOrder = LowestLayerOrder;
        }
        else
        {
            TargetSR.sortingOrder = HighestLayerOrder;
        }
    }

    #endregion

    #region Set

    private void SetLayerSort()
    {
        if (PlayerTF != null)
        {
            float targetY = PlayerTF.transform.position.y;
            float thisY = this.gameObject.transform.position.y;

            if (thisY > targetY)
            {
                IsUpper.Value = true;
            }
            else if (thisY < targetY)
            {
                IsUpper.Value = false;
            }
        }
    }

    #endregion
}
