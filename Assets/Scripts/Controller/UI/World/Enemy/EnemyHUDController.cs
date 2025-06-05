using System.Collections;
using UnityEngine;

public class EnemyHUDController : MonoBehaviour
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> HUD")]

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] public Canvas ThisCanvas;
    [SerializeField] public EnemyStateUIController StateUI;
    [SerializeField] public EnemyBuffUIController TemporaryBuffUI;
    [SerializeField] public EnemyBuffUIController PermanentBuffUI;

    [Space(10)]
    [Header("=== Special")]
    [SerializeField] private GameObject EPTxtGO;
    [SerializeField] private GameObject EPChargePanelGO;
    [SerializeField] private CanvasGroup EPSpecialPatterningCG;

    #endregion

    #region - Hide

    [HideInInspector] private Coroutine ThisCor;

    #endregion

    #endregion

    #region Offset

    public void Offset(EnemyController _Enemy)
    {
        StateUI.Offset(this);
        TemporaryBuffUI.Offset(this);
        PermanentBuffUI.Offset(this);
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

    public void Set_Charged(bool _OnOff)
    {
        EPTxtGO.gameObject.SetActive(!_OnOff);
        EPChargePanelGO.gameObject.SetActive(_OnOff);
    }

    public void Set_Patterning(bool _OnOff)
    {
        EPSpecialPatterningCG.gameObject.SetActive(_OnOff);

        if (_OnOff)
        {
            ThisCor = StartCoroutine(Play_Patterning_Cor());
        }
        else
        {
            if (ThisCor != null) 
                StopCoroutine(ThisCor);

            ThisCor = null;
        }
    }

    #endregion

    #region Play

    private IEnumerator Play_Patterning_Cor()
    {
        EPSpecialPatterningCG.alpha = 1f;
        bool bookLightOn = false;

        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            if (bookLightOn)
                EPSpecialPatterningCG.alpha = 1f;
            else
                EPSpecialPatterningCG.alpha = 0.2f;

            bookLightOn = !bookLightOn;
        }
    }

    #endregion
}
