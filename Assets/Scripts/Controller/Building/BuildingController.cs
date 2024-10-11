using UnityEngine;

public class BuildingController : HaveShadowThingStatic
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

    [SerializeField] private bool IsUpper = false;
    [HideInInspector] private Transform PlayerTF;

    #endregion

    #region Framework

    private void Start()
    {
        if (PlayerTF == null)
        {
            PlayerTF = PlayerManager.Instance.PlayerController.gameObject.transform;
        }
    }

    private void Update()
    {
       if (PlayerTF != null)
       {
            float targetY = PlayerTF.transform.position.y;
            float thisY = this.gameObject.transform.position.y;

            if (thisY > targetY && !IsUpper)
            {
                IsUpper = true;
                SetLayerOrder();
            }
            else if (thisY < targetY && IsUpper)
            {
                IsUpper = false;
                SetLayerOrder();
            }
       }
    }

    private void SetLayerOrder()
    {
        if (IsUpper)
        {
            TargetSR.sortingOrder = LowestLayerOrder;
            ShadowSR.sortingOrder = LowestLayerOrder - 1;
        }
        else
        {
            TargetSR.sortingOrder = HighestLayerOrder;
            ShadowSR.sortingOrder = HighestLayerOrder - 1;
        }
    }
    #endregion
}
