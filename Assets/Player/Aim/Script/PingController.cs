using DG.Tweening;
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
    [SerializeField] private SpriteRenderer pingFrameSr;

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

    public void Set_SortingOrder(int order)
    {
        ThisSG.sortingOrder = order;
    }

    #endregion

    #region Set On/Off

    public void SetOn_Ping(EnemyController enemy, float durTime = 0.2f)
    {
        gameObject.SetActive(true);
        transform.SetParent(enemy.gameObject.transform);

        transform.SetAsLastSibling();

        DevTool.SetKillTween(pingFrameSr.color);
        DevTool.SetKillTween(pingFrameSr.size);

        transform.localPosition = Vector2.zero;
        pingFrameSr.color = new Color(1, 1, 1, 0);
        pingFrameSr.size = new Vector2(0.6f, 0.6f);
        pingFrameSr.transform.localPosition = enemy.pingOffsetVec;

        pingFrameSr.DOFade(1f, durTime);
        DOTween.To(() => pingFrameSr.size, vec => pingFrameSr.size = vec, enemy.pingSizeVec, durTime);

    }

    public void SetOff_Ping(Transform tf)
    {
        gameObject.SetActive(false);
        transform.SetParent(tf);

        transform.localPosition = Vector2.zero;
    }

    #endregion
}
