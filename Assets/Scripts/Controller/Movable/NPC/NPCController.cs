using UnityEngine;

public class NPCController : MovableObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> NPC")]
    [SerializeField] public string Name;

    #endregion

    #region Framework

    protected override void OnEnable()
    {
        base.OnEnable();

        DevTool.Add_InList(NPCManager.Instance.AllNPCs, this);
    }

    #endregion
}
