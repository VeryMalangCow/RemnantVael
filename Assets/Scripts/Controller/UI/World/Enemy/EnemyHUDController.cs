using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyHUDController : MonoBehaviour
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> HUD")]

    [Space(10)]
    [Header("=== Comp")]
    [FormerlySerializedAs("ThisCanvas")][SerializeField] public Canvas canvas;
    [FormerlySerializedAs("StateUI")][SerializeField] public EnemyStateUIController stateUi;
    [FormerlySerializedAs("TemporaryBuffUI")][SerializeField] public EnemyBuffUIController temporaryBuffUi;
    [FormerlySerializedAs("PermanentBuffUI")][SerializeField] public EnemyBuffUIController permanentBuffUi;

    [Space(10)]
    [Header("=== Special")]
    [FormerlySerializedAs("EPTxtGO")][SerializeField] private GameObject epTxtGo;
    [FormerlySerializedAs("EPChargePanelGO")][SerializeField] private GameObject epChargePanelGo;
    [FormerlySerializedAs("EPSpecialPatterningCG")][SerializeField] private CanvasGroup epSpecialPatterningCg;

    #endregion

    #region - Hide

    [HideInInspector] private Coroutine cor;

    #endregion

    #endregion

    #region Offset

    public void Offset()
    {
        stateUi.Offset(this);
        temporaryBuffUi.Offset(this);
        permanentBuffUi.Offset(this);
    }

    #endregion

    #region Reset

    public void Reset_HUD()
    {
        Set_Charged(false);
        Set_Patterning(false);
    }

    #endregion

    #region Set (Charge)

    public void Set_Charged(bool onOff)
    {
        epTxtGo.gameObject.SetActive(!onOff);
        epChargePanelGo.gameObject.SetActive(onOff);
    }

    public void Set_Patterning(bool onOff)
    {
        epSpecialPatterningCg.gameObject.SetActive(onOff);

        if (onOff)
        {
            cor = StartCoroutine(Play_Patterning_Cor());
        }
        else
        {
            if (cor != null) 
                StopCoroutine(cor);

            cor = null;
        }
    }

    #endregion

    #region Play

    private IEnumerator Play_Patterning_Cor()
    {
        epSpecialPatterningCg.alpha = 1f;
        bool bookLightOn = false;

        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            if (bookLightOn)
                epSpecialPatterningCg.alpha = 1f;
            else
                epSpecialPatterningCg.alpha = 0.2f;

            bookLightOn = !bookLightOn;
        }
    }

    #endregion
}
