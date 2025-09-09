using UnityEngine;

public class BuildSetAnimController : MonoBehaviour
{
    #region Set

    private void Add_CurrentSetAnim()
    {
        StageManager.Instance.Add_SetAnim(this);
    }

    #endregion

    #region Framework

    private void OnEnable()
    {
        Add_CurrentSetAnim();
    }

    #endregion
}
