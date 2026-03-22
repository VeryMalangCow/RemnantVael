using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BoxConnectionEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Value")]
    [FormerlySerializedAs("ThisContactPosList")][SerializeField] private List<Vector2Int> contactPosList;

    [HideInInspector] private Image innerImg;

    #endregion

    #region Offset

    public override void Offset()
    {
        innerImg = DevTool.Get_ComponentTType(gameObject.transform.GetChild(0).gameObject, out Image _img) ? _img : null;
    }

    #endregion

    #region Set

    public void Set_Active(bool onOff)
    {
        gameObject.SetActive(onOff);
    }

    public void Set_ActiveByCondition(HashSet<Vector2Int> onDirBoxCell)
    {
        for (int i = 0; i < contactPosList.Count; i++)
        {
            if (!onDirBoxCell.Contains(contactPosList[i]))
            {
                Set_Active(false);
                return;
            }
        }
        Set_Active(true);
        return;
    }


    public void Set_InnerColor(Color clr)
    {
        DevTool.Set_Color(clr, innerImg);
    }
    #endregion
}
