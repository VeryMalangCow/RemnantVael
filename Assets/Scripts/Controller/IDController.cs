using System.Collections.Generic;
using UnityEngine;

public class IDController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> ID")]
    [SerializeField] protected int ID = 0;
    public int Get_ID() { return ID; }

    #endregion

    #region Set

    public virtual void Offset(int _ID)
    {
        ID = _ID;
    }

    #endregion

    #region Static

    // 아이디를 가진 객체에 맞는 객체 찾기
    public static T Get_CorrectIDObject<T>(int _ID, List<IDController> _ObjectList) where T : class
    {
        for (int i = 0; i < _ObjectList.Count; i++)
        {
            if (Is_CorrectID(_ID, _ObjectList[i]))
            {
                return DevTool.Get_CastingTType<T>(_ObjectList[i]);
            }
        }
        return default;
    }

    // ID가 맞는가 판별
    public static bool Is_CorrectID(int _ID, IDController _Object)
    {
        if (_Object.ID == _ID)
        {
            return true;
        }
        return false;
    }


    #endregion
}
