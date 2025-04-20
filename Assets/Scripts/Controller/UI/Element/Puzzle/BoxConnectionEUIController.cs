using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxConnectionEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private List<Vector2Int> ThisContactPosList;

    [HideInInspector] private Image InnerImg;

    #endregion

    #region Offset

    public override void Offset()
    {
        InnerImg = DevTool.Get_ComponentTType(gameObject.transform.GetChild(0).gameObject, out Image img) ? img : null;
    }

    #endregion

    #region Set

    public void Set_Active(bool _OnOff)
    {
        gameObject.SetActive(_OnOff);
    }

    public void Set_ActiveByCondition(HashSet<Vector2Int> _OnDirBoxCell)
    {
        for (int i = 0; i < ThisContactPosList.Count; i++)
        {
            if (!_OnDirBoxCell.Contains(ThisContactPosList[i]))
            {
                Set_Active(false);
                return;
            }
        }
        Set_Active(true);
        return;
    }


    public void Set_InnerColor(Color _Clr)
    {
        DevTool.Set_Color(_Clr, InnerImg);
    }
    #endregion
}
