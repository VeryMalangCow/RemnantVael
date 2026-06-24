using UnityEngine;

public class PrisonRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault")]

    [Space(10)]
    [Header("=== ParentTF")]
    [SerializeField] public Transform inRoom_PrisonParentTF;
    [SerializeField] public Transform inRoom_PayOperactorParentTF;
    [SerializeField] public Transform inRoom_PuzzleOperactorParentTF;

    [HideInInspector] public PrisonController prison = null;
    [HideInInspector] public PrisonPayOperatorController payOperator = null;
    [HideInInspector] public PrisonPuzzleOperatorController puzzleOperator = null;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        needKeyCardId = 2;
    }

    #endregion

    #region Set

    public override void Set_Completed()
    {
        base.Set_Completed();

        SetOn_Prison();
    }

    private void SetOn_Prison()
    {
        prison.gameObject.SetActive(true);
        payOperator.gameObject.SetActive(true);
        puzzleOperator.gameObject.SetActive(true);
    }

    #endregion

    #region Prison & Oper

    public void SetPrison(PrisonController _prison, PrisonPayOperatorController payOper, PrisonPuzzleOperatorController puzzleOper)
    {
        prison = _prison;
        prison.transform.SetParent(inRoom_PrisonParentTF);
        prison.gameObject.transform.localPosition = Vector2.zero;
        prison.gameObject.SetActive(false);

        payOperator = payOper;
        payOperator.transform.SetParent(inRoom_PayOperactorParentTF);
        payOperator.Set_TargetBuild(prison);
        payOperator.gameObject.SetActive(false);

        puzzleOperator = puzzleOper;
        puzzleOperator.transform.SetParent(inRoom_PuzzleOperactorParentTF);
        puzzleOperator.Set_TargetBuild(prison);
        puzzleOperator.gameObject.SetActive(false);
    }

    #endregion
}
