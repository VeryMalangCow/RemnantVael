using UnityEngine;

public class BuildingController : HaveShadowThing
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Building")]

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private float ResistancePower = 1f;

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        GameObject MovableObject = _Col.gameObject.transform.parent.gameObject;
        if (MovableObject.tag == "Player")
        {

        }
    }


    #endregion
}
