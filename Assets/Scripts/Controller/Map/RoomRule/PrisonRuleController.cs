using UnityEngine;
using UnityEngine.Serialization;

public class PrisonRuleController : RoomRuleController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Vault")]

    [Space(10)]
    [Header("=== ParentTF")]
    [FormerlySerializedAs("InRoom_PrisonParentTF")][SerializeField] public Transform inRoom_PrisonParentTF;
    [FormerlySerializedAs("InRoom_PayOperactorParentTF")][SerializeField] public Transform inRoom_PayOperactorParentTF;
    [FormerlySerializedAs("InRoom_PuzzleOperactorParentTF")][SerializeField] public Transform inRoom_PuzzleOperactorParentTF;

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
}
