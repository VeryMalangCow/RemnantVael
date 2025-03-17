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
        Offset_TF();
        Offset_StaticDepthList();
        Offset_GateList();
    }

    private void Offset_TF()
    {
        if (SetSRDepthListParent != null && SetSRDepthListParent.childCount > 0)
        {
            List<StaticDepthController> list = DevTool.Get_ChildList<StaticDepthController>(SetSRDepthListParent);
            for (int i = 0; i < list.Count; i++)
            {
                SpriteRenderer sr = DevTool.Get_ComponentTType<SpriteRenderer>(list[i].TargetObject);
                if (sr != null && sr != default) SetSRList.Add(sr);
            }
        }
        SetSRDepthListParent = null;
    }

    private void Offset_StaticDepthList()
    {
        for (int i = 0; i < SetSRParentDepthList.Count; i++)
        {
            SpriteRenderer sr = DevTool.Get_ComponentTType<SpriteRenderer>(SetSRParentDepthList[i].TargetObject);
            if (sr != null && sr != default) SetSRList.Add(sr);
            List<SpriteRenderer> srList = DevTool.Get_ChildList<SpriteRenderer>(SetSRParentDepthList[i].TargetObject.transform);
            if (srList != null && srList.Count > 0) SetSRList.AddRange(srList);
        }
        SetSRParentDepthList.Clear();
        SetSRParentDepthList = null;
    }

    private void Offset_GateList()
    {
        for (int i = 0; i < SetSRParentGateList.Count; i++)
        {
            SetSRList.AddRange(SetSRParentGateList[i].OpacityLowerSRList);
        }
        SetSRParentGateList.Clear();
        SetSRParentGateList = null;
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
