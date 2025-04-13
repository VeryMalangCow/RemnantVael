using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PrisonRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault")]

    [Space(10)]
    [Header("=== ParentTF")]
    [SerializeField] public Transform InRoom_PrisonParentTF;
    [SerializeField] public Transform InRoom_PayOperactorParentTF;
    [SerializeField] public Transform InRoom_PuzzleOperactorParentTF;

    [HideInInspector] public PrisonController Prison = null;
    [HideInInspector] public PrisonPayOperatorController PayOperator = null;
    [HideInInspector] public PrisonPuzzleOperatorController PuzzleOperator = null;

    #endregion

    #region Set

    public override void Set_Completed()
    {
        base.Set_Completed();

        SetOn_Prison();
    }

    private void SetOn_Prison()
    {
        Prison.gameObject.SetActive(true);
        PayOperator.gameObject.SetActive(true);
        PuzzleOperator.gameObject.SetActive(true);
    }

    #endregion
}
