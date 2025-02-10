using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ModifySynergySlot : ModifyOwnEachBtn
{
    #region Value

    [HideInInspector] public int ID;
    [HideInInspector] public Image ThisImg;
    [HideInInspector] public Image ThisTierImg;
    [HideInInspector] public TMP_Text ThisTxt;

    #endregion

    #region Offset

    public override void Offset()
    {
        base.Offset();

        if (ThisImg == null && this.gameObject.TryGetComponent(out Image Img))
        { ThisImg = Img; }
        
        if (ThisTierImg == null && this.gameObject.transform.GetChild(0).TryGetComponent(out Image TierImg))
        { ThisTierImg = TierImg; }

        if (ThisTxt == null && this.gameObject.transform.GetChild(1).TryGetComponent(out TMP_Text TierTxt))
        { ThisTxt = TierTxt; }

        SetOffSynergySlot();
    }

    #endregion

    #region Set

    public void SetOffSynergySlot()
    {
        this.gameObject.SetActive(false);
    }

    public void SetOnSynergySlot(int _ID, Sprite _Icon, int _Amalgamation)
    {
        this.gameObject.SetActive(true);

        ID = _ID;
        ThisImg.sprite = _Icon;

        if (_Amalgamation < 5) 
        { ThisTierImg.sprite = MainGameUIManager.Instance.ModuleUpgrade_UIController.SynergyTier0; }
        else if (_Amalgamation < 10)
        { ThisTierImg.sprite = MainGameUIManager.Instance.ModuleUpgrade_UIController.SynergyTier1; }
        else if (_Amalgamation < 15)
        { ThisTierImg.sprite = MainGameUIManager.Instance.ModuleUpgrade_UIController.SynergyTier2; }

        ThisTxt.text = _Amalgamation.ToString();
    }

    #endregion
}
