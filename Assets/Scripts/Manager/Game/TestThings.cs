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

            player.GainChargedBettery(9999);
            player.GainCredit(9999);
            player.GainOverrider(9999);
            player.GainModuleShard(9999);

            player.strikeTeamPresence.Value = 100;
            player.uplinkTeamPresence.Value = 100;
            player.neoTeamPresence.Value = 100;

            player.GainKeyCard(0, 99);
            player.GainKeyCard(1, 99);
            player.GainKeyCard(2, 99);
            player.GainKeyCard(3, 99);
            player.GainKeyCard(4, 99);

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
