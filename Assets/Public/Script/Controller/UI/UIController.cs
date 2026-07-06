using UnityEngine;

public class UIController : MonoBehaviour
{
    protected Canvas canvas;

    public virtual void Offset() 
    {
        if (TryGetComponent(out Canvas _canvas))
        {
            canvas = _canvas;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    public virtual void SetLanguageTxt() { }

}