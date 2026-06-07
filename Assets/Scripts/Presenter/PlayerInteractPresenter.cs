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

    public void SubscribeOn()
    {
        player.OnInteractableChanged += interactView.SetInteractableUI;
        player.OnInteractableChanged += interactAnno.SetInteractableUI;
    }

    public void SubscribeOff()
    {
        player.OnInteractableChanged -= interactView.SetInteractableUI;
        player.OnInteractableChanged -= interactAnno.SetInteractableUI;
    }
}
