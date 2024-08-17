using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIController : MonoBehaviour
{
    protected abstract void Offset_Module();
    protected abstract void Offset_UI();

    protected virtual void Start()
    {
        Offset_Module();
        Offset_UI();
    }

    public virtual void OpenThisPanel()
    {
        InputManager.Instance.InputMoveDir = Vector2.zero;

        UIManager.Instance.CurrentOpeningUIController = this;
        this.gameObject.SetActive(true);
        InputManager.Instance.enabled = false;
    }

    public virtual void CloseThisPanel()
    {
        UIManager.Instance.CurrentOpeningUIController = null;
        this.gameObject.SetActive(false);
        InputManager.Instance.enabled = true;
    }
}
