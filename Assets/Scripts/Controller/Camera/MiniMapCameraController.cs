using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCameraController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> MiniMap Camera")]

    [Space(10)]
    [Header("=== Value")]
    [SerializeField] Vector3 OffsetVec = new Vector3(-275, -150, -10);
    [SerializeField] GameObject CenterImg;

    #endregion

    #region Set Pos

    public void SetPos(Vector2 _CurrentMapPos)
    {
        this.transform.position = (Vector3)_CurrentMapPos + OffsetVec;
        CenterImg.transform.position = _CurrentMapPos;
    }

    #endregion
}
