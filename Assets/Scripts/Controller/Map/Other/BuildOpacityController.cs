using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BuildOpacityController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Visible State")]
    [SerializeField] private List<GateController> SetSRParentGateList;
    [SerializeField] private List<StaticDepthController> SetSRParentDepthList;
    [SerializeField] private Transform SetSRDepthListParent;

    // Sr
    [SerializeField] private List<SpriteRenderer> SetSRList = new List<SpriteRenderer>();

    // Value
    [HideInInspector] private bool IsCompletlyVisible = true;
    [HideInInspector] private float OpacityValue = 0.7f;
    [HideInInspector] private float DurTime = 0.3f;

    // Data
    [HideInInspector] private Sequence ThisSeq;
    [HideInInspector] private HashSet<Collider2D> currentCollisions = new HashSet<Collider2D>();

    #endregion

    #region Offset

    private void Offset()
    {
        Offset_TF(SetSRDepthListParent);
        Offset_StaticDepthList(SetSRParentDepthList);
        Offset_GateList(SetSRParentGateList);
    }

    private void Offset_TF(Transform _TF)
    {
        if (_TF != null && _TF.childCount > 0)
        {
            List<StaticDepthController> depthList = DevTool.Get_ChildList<StaticDepthController>(_TF);
            for (int i = 0; i < depthList.Count; i++)
            {
                SpriteRenderer sr = DevTool.Get_ComponentTType<SpriteRenderer>(depthList[i].TargetObject);
                if (sr != null && sr != default) SetSRList.Add(sr);
            }
        }
        _TF = null;
    }

    private void Offset_StaticDepthList(List<StaticDepthController> _DepthList)
    {
        for (int i = 0; i < _DepthList.Count; i++)
        {
            SpriteRenderer sr = DevTool.Get_ComponentTType<SpriteRenderer>(_DepthList[i].TargetObject);
            if (sr != null && sr != default) SetSRList.Add(sr);

            List<SpriteRenderer> srList = DevTool.Get_ChildList<SpriteRenderer>(_DepthList[i].TargetObject.transform);
            if (srList != null && srList.Count > 0) SetSRList.AddRange(srList);
        }
        _DepthList.Clear();
        _DepthList = null;
    }

    private void Offset_GateList(List<GateController> _GateList)
    {
        for (int i = 0; i < _GateList.Count; i++)
        {
            SetSRList.AddRange(_GateList[i].OpacityLowerSRList);
        }
        _GateList.Clear();
        _GateList = null;
    }

    #endregion

    #region Framework

    private void Start()
    {
        Offset();
    }

    private void LateUpdate()
    {
        if (Is_Colliding() && IsCompletlyVisible == true)
        {
            IsCompletlyVisible = false;
            Set_Visible(OpacityValue);
        }
        else if (!Is_Colliding() && IsCompletlyVisible == false)
        {
            IsCompletlyVisible = true;
            Set_Visible();
        }
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        currentCollisions.Add(_Col);
    }

    private void OnTriggerExit2D(Collider2D _Col)
    {
        currentCollisions.Remove(_Col);
    }

    #endregion

    #region Set State

    public bool Is_Colliding()
    {
        return currentCollisions.Count > 0;
    }

    private void Set_Visible(float _Alpha = 1f)
    {
        if (SetSRList == null || SetSRList.Count <= 0) return;

        DevTool.Set_KillTween(ThisSeq);

        ThisSeq = DOTween.Sequence();
        for (int i = 0; i < SetSRList.Count; i++)
        {
            ThisSeq.Join(SetSRList[i].DOFade(_Alpha, DurTime));
        }
    }

    #endregion
}
