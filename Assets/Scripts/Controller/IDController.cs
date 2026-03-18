using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class IDController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> ID")]
    [SerializeField] protected int id = 0;

    public int Get_ID() { return id; }

    #endregion

    #region Set

    public virtual void Offset(int _ID)
    {
        id = _ID;
    }

    #endregion

    #region Static

    // 아이디를 가진 객체에 맞는 객체 찾기
    public static T Get_CorrectIDObject<T>(int _ID, List<IDController> _ObjectList) where T : class
    {
        for (int i = 0; i < _ObjectList.Count; i++)
            if (Is_CorrectID(_ID, _ObjectList[i]))
                return DevTool.Get_CastingTType<T>(_ObjectList[i]);
            
        return default;
    }

    public static T Get_CorrectIDObject<T>(int _ID, Dictionary<int, T> _ObjectDict) where T : IDController
    {
        if (_ObjectDict.ContainsKey(_ID)) 
            return _ObjectDict[_ID];

        return null;
    }

    // ID가 맞는가 판별
    private static bool Is_CorrectID(int _ID, IDController _Object)
    {
        if (_Object.id == _ID) return true;
        return false;
    }


    #endregion
}
