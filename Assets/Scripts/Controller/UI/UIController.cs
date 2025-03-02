using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIController : MonoBehaviour
{
    #region Value

    #endregion

    #region Abstract Function

    protected abstract void Offset_Module();
    protected abstract void Offset_UI();

    #endregion

    #region Framework

    public virtual void Offset_Main()
    {
        Offset_Module();
        Offset_UI();
    }

    #endregion

    #region Set Color

    protected void Set_Color(Color _Clr, List<Component> _ApplyCompList)
    {
        for (int i = 0; i < _ApplyCompList.Count; i++)
        {
            if (_ApplyCompList[i].TryGetComponent(out TMP_Text tmp))
            {
                Set_Color(_Clr, tmp);
            }
            else if (_ApplyCompList[i].TryGetComponent(out Image img))
            {
                Set_Color(_Clr, img);
            }
        }
    }

    private void Set_Color(Color _Clr, TMP_Text _Txt)
    {
        Color clr = _Clr;
        if (_Txt != null)
        {
            clr.a = _Txt.color.a;
            _Txt.color = clr;
        }
    }

    private void Set_Color(Color _Clr, Image _Img)
    {
        Color clr = _Clr;
        if (_Img != null)
        {
            clr.a = _Img.color.a;
            _Img.color = clr;
        }
    }

    #endregion

    #region For UI Case

    protected string Get_KindOfCaseString(IInteract _II)
    {
        if (_II == null)
        { return ""; }
        if (_II is EnemyController EC && EC.IsDischarge)
        { return "KILL"; }
        else if (_II is DestructibleBuildController DBC && !DBC.IsBroken && (_II is BaseUpgradeController || _II is ModuleUpgradeController))
        { return "SHOP"; }
        else if (_II is GateController GC && GC.IsOpen)
        { return "GATE"; }
        else if (_II is InteractItemController)
        { return "MODULE"; }
        else if (_II is StartElevatorController DEC && DEC.IsOn)
        { return "NEXT STAGE"; }

        return "";
    }

    #endregion

    #region Dur

    protected void Set_Dur(int _DurState, List<Image> _ImgList, TMP_Text _Txt)
    {
        _Txt.text = _DurState.ToString();
        for (int i = 0; i < _ImgList.Count; i++)
        {
            if (_DurState > i) // On
            {
                _ImgList[i].color = Color.white;
            }
            else // Off
            {
                _ImgList[i].color = new Color(1, 1, 1, 0);
            }
        }
    }


    #endregion
}