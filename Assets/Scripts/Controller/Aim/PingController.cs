using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PingController : MonoBehaviour
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Ping")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private SpriteRenderer PingFrameSR;

    #endregion

    #region - Hide

    [HideInInspector] private SortingGroup ThisSG;

    #endregion

    #endregion

    #region Offset

    private void Offset()
    {
        ThisSG = DevTool.Get_ComponentTType(gameObject, out SortingGroup sg) ? sg : null;
    }

    #endregion

    #region Framework

    private void Awake()
    {
        Offset();
    }

    #endregion

    #region Set (Sort)

    public void Set_SortingOrder(int _Order)
    {
        ThisSG.sortingOrder = _Order;
    }

    #endregion

    #region Set On/Off

    public void SetOn_Ping(EnemyController _Enemy, float _DurTime = 0.2f)
    {
        gameObject.SetActive(true);
        transform.SetParent(_Enemy.gameObject.transform);

        transform.SetAsLastSibling();

        DevTool.Set_KillTween(PingFrameSR.color);
        DevTool.Set_KillTween(PingFrameSR.size);

        transform.localPosition = Vector2.zero;
        PingFrameSR.color = new Color(1, 1, 1, 0);
        PingFrameSR.size = new Vector2(0.6f, 0.6f);
        PingFrameSR.transform.localPosition = _Enemy.PingOffsetVec;

        PingFrameSR.DOFade(1f, _DurTime);
        DOTween.To(() => PingFrameSR.size, vec => PingFrameSR.size = vec, _Enemy.PingSizeVec, _DurTime);

    }

    public void SetOff_Ping(Transform _TF)
    {
        gameObject.SetActive(false);
        transform.SetParent(_TF);

        transform.localPosition = Vector2.zero;
    }

    #endregion
}
