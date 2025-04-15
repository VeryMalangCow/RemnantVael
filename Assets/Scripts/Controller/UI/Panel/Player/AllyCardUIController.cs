using System.Collections.Generic;
using UnityEngine;

public class AllyCardUIController : PanelUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ally Card UI")]

    [Space(10)]
    [Header("=== TF")]
    [SerializeField] private Transform CardParentTF;

    #endregion

    #region - Hide

    [HideInInspector] private List<AllyCardEUIController> Cards;

    #endregion

    #endregion

    #region Offset

    public override void Offset() 
    {
        Cards = DevTool.Get_ChildList<AllyCardEUIController>(CardParentTF);
        for (int i = 0; i < Cards.Count; i++)
            Cards[i].gameObject.SetActive(false);
    }

    #endregion

    #region Interact

    public void Try_Interact()
    {

    }

    #endregion
}
