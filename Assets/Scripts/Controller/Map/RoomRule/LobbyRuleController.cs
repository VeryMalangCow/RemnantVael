using System.Collections;
using UnityEngine;

public class LobbyRuleController : RoomRuleController
{
    #region Set

    public void Set_LobbyStart(float _DelayTime)
    {
        StartCoroutine(Play_LobbyStart_Cor(_DelayTime));
    }

    public void Set_LobbyOffset()
    {
        PlayerManager.Instance.PlayerController.Set_StartStage();
        EventManager.Instance.Set_Input(true);
    }

    #endregion

    #region Play

    private IEnumerator Play_LobbyStart_Cor(float _DelayTime)
    {
        Debug.Log("½ÃÀÛ");
        yield return new WaitForSeconds(_DelayTime);

        Debug.Log("¼Â");
        Set_LobbyOffset();
    }

    #endregion
}
