using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxConnectionEUIController : ElementUIController
{
    #region Value

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] private Vector2Int[] contactPositions;
    [SerializeField] private RectTransform rt;

    [HideInInspector] private Image innerImg;

    #endregion

    #region Init

    public void Init(Vector2Int vecIntA, Vector2Int vecIntB, Vector2 pos, bool rotateRightAngle)
    {
        contactPositions = new Vector2Int[2];
        contactPositions[0] = vecIntA;
        contactPositions[1] = vecIntB;

        rt.anchoredPosition = pos;
        if (rotateRightAngle)
            rt.rotation = Quaternion.Euler(0, 0, 90);
        else
            rt.rotation = Quaternion.identity;
    }

    #endregion

    #region Offset

    public override void Offset()
    {
        innerImg = DevTool.Get_ComponentTType(gameObject.transform.GetChild(0).gameObject, out Image _img) ? _img : null;
    }

    #endregion

    #region Set

    public void SetActive(bool onOff)
    {
        gameObject.SetActive(onOff);
    }

    public void SetActiveByCondition(HashSet<Vector2Int> onDirBoxCell)
    {
        for (int i = 0; i < contactPositions.Length; i++)
        {
            if (!onDirBoxCell.Contains(contactPositions[i]))
            {
                SetActive(false);
                return;
            }
        }
        SetActive(true);
        return;
    }


    public void SetInnerColor(Color clr)
    {
        DevTool.SetColor(clr, innerImg);
    }
    #endregion
}
