using UnityEngine;

public class UIController : MonoBehaviour
{
    protected Canvas canvas;

    public virtual void Offset(Camera uiCamera) 
    {
        if (TryGetComponent(out Canvas _canvas))
        {
            canvas = _canvas;
            canvas.worldCamera = uiCamera;
        }
    }

    public virtual void Set_LanguageTxt() { }

}