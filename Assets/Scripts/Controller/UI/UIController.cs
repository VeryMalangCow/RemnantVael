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
}
