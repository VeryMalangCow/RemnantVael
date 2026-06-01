using UnityEngine;

public class TestThings : MonoBehaviour
{
    #region Value

    #endregion

    [Space(20)]
    [Header("<><><><><> Unit Manager")]

    [SerializeField] private GameObject[] testGO;
    [SerializeField] private GameObject ultraModeGO;
    public void Test_Cor()
    {
        for (int i = 0; i < testGO.Length; i++)
            testGO[i].gameObject.SetActive(true);
    }

    public void Set_UltraModeGO(bool onOff)
    {
        ultraModeGO.SetActive(onOff);
    }

    public static bool isUltraMode = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4) && StageManager.instance.targetStageID != 99)
        {
            PlayerController player = PlayerManager.instance.playerController;
            /*
            BaseWeapon.BaseDamage.BuffedState = 300f;
            BaseWeapon.AccuracyRate.ActualState.Value = 100f;
            WalkSpeed.ActualState.Value = 15f;
            */

            player.currentChargedBettery.Value = 9999;
            player.currentCredit.Value = 9999;
            player.currentOverrider.Value = 9999;
            player.currentModuleShard.Value = 9999;

            player.strikeTeamPresence.Value = 100;
            player.uplinkTeamPresence.Value = 100;
            player.neoTeamPresence.Value = 100;

            PlayerManager.instance.Gain_KeyCard(0, 99);
            PlayerManager.instance.Gain_KeyCard(1, 99);
            PlayerManager.instance.Gain_KeyCard(2, 99);
            PlayerManager.instance.Gain_KeyCard(3, 99);
            PlayerManager.instance.Gain_KeyCard(4, 99);

            Debug.Log("Alpha4: Get Many Goods");
        }

        else if (Input.GetKeyDown(KeyCode.Alpha5) && StageManager.instance.targetStageID != 99)
        {
            Test_Cor();

            Debug.Log("Alpha5: Spawn Builds");
        }

        else if (Input.GetKeyDown(KeyCode.Alpha6) && StageManager.instance.targetStageID != 99)
        {
            isUltraMode = !isUltraMode;
            Set_UltraModeGO(isUltraMode);

            Debug.Log("Alpha6: Ultra Mode " + isUltraMode);
        }
    }
}
