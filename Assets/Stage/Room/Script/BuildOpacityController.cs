using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BuildOpacityController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Visible State")]
    [SerializeField] private List<GateController> setSrParentGateList;
    [SerializeField] private List<StaticDepthController> setSrParentDepthList;
    [SerializeField] private Transform setSrDepthListParent;

    // Sr
    [SerializeField] private List<SpriteRenderer> setSrList = new List<SpriteRenderer>();

    // Value
    [HideInInspector] private bool isColliding = false;
    [HideInInspector] private float opacityValue = 0.7f;
    [HideInInspector] private float targetOpactiyValue = 1f;
    [HideInInspector] private float durTime = 0.3f;

    // Data
    [HideInInspector] private Sequence thisSeq = null;

    #endregion

    #region Offset

    private void Offset()
    {
        Offset_TF(setSrDepthListParent);
        Offset_StaticDepthList(setSrParentDepthList);
        Offset_GateList(setSrParentGateList);
    }

    private void Offset_TF(Transform tf)
    {
        if (tf == null || tf.childCount <= 0) return;

        List<StaticDepthController> depthList = DevTool.Get_ChildList<StaticDepthController>(tf);
        for (int i = 0; i < depthList.Count; i++)
        {
            SpriteRenderer sr = DevTool.Get_ComponentTType(depthList[i].targetObject, out SpriteRenderer outSr) ? outSr : null;
            if (sr != null && sr != default) setSrList.Add(sr);
        }

        tf = null;
    }

    private void Offset_StaticDepthList(List<StaticDepthController> depthList)
    {
        if (depthList == null || depthList.Count <= 0) return;

        for (int i = 0; i < depthList.Count; i++)
        {
            SpriteRenderer sr = DevTool.Get_ComponentTType(depthList[i].targetObject, out SpriteRenderer outSr) ? outSr : null;
            if (sr != null && sr != default) setSrList.Add(sr);

            List<SpriteRenderer> srList = DevTool.Get_ChildList<SpriteRenderer>(depthList[i].targetObject.transform);
            if (srList != null && srList.Count > 0) setSrList.AddRange(srList);
        }
        depthList.Clear();
        depthList = null;
    }

    private void Offset_GateList(List<GateController> gateList)
    {
        if (gateList == null || gateList.Count <= 0) return;

        for (int i = 0; i < gateList.Count; i++)
        {
            setSrList.AddRange(gateList[i].opacityLowerSrList);
        }
        gateList.Clear();
        gateList = null;
    }

    #endregion

    #region Framework

    private void Start()
    {
        Offset();
    }

    private void FixedUpdate()
    {
        isColliding = false;
    }

    private void LateUpdate()
    {
        Check_Visible();
    }

    #endregion

    #region Trigger

    private void OnTriggerStay2D(Collider2D col)
    {
        isColliding = true;
    }

    #endregion

    #region Set State

    private void Check_Visible()
    {
        if (isColliding && targetOpactiyValue != opacityValue)
            Set_Visible(opacityValue);
        else if (!isColliding && targetOpactiyValue != 1)
            Set_Visible();
    }

    private void Set_Visible(float alpha = 1f)
    {
        if (setSrList == null || setSrList.Count <= 0) return;

        targetOpactiyValue = alpha;

        DevTool.SetKillTween(thisSeq);

        thisSeq = DOTween.Sequence();
        for (int i = 0; i < setSrList.Count; i++)
        {
            thisSeq.Join(setSrList[i].DOFade(alpha, durTime));
        }
        thisSeq.OnComplete(() => { thisSeq = null; });
    }

    #endregion
}
