using UnityEngine;

public class ShopController : BuildingController, IInteract
{
    #region Value
    [Space(20)]
    [Header("<><><><><> OneOff Shop")]

    [Space(10)]
    [Header("=== Prefab")]
    [SerializeField] private GameObject g;

    #endregion

    public void Interact()
    {
        
    }
}
