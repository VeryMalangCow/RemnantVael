using UnityEngine;

public class PlayerInteractPresenter : MonoBehaviour, IPresentable
{
    private PlayerController player;

    private HudInteractView interactView;
    private InteractAnnoUIController interactAnno;

    public void Init(PresenterOwnerData ownerData, PresenterUIData uiData)
    {
        player = ownerData.player;

        interactView = uiData.hud.InteractView;
        interactAnno = uiData.interactAnno;
    }

    private void SetInteract(IInteract ii)
    {
        interactView.SetInteractableUI(ii);
        interactAnno.SetInteractableUI(ii);
    }

    public void SubscribeOn()
    {
        player.OnInteractableChanged += SetInteract;
    }

    public void SubscribeOff()
    {
        player.OnInteractableChanged -= SetInteract;
    }
}
