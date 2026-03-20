using System.Collections.Generic;
using UnityEngine;

public class IDController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> ID")]
    [SerializeField] protected int id = 0;

    public int Get_ID() { return id; }

    #endregion

    #region Set

    public virtual void Offset(int id)
    {
        this.id = id;
    }

    #endregion

    #region Static

    // 아이디를 가진 객체에 맞는 객체 찾기
    public static T Get_CorrectIDObject<T>(int id, List<IDController> objList) where T : class
    {
        for (int i = 0; i < objList.Count; i++)
            if (Is_CorrectID(id, objList[i]))
                return DevTool.Get_CastingTType<T>(objList[i]);
            
        return default;
    }

    public static T Get_CorrectIDObject<T>(int id, Dictionary<int, T> objList) where T : IDController
    {
        if (objList.ContainsKey(id)) 
            return objList[id];

        return null;
    }

    // ID가 맞는가 판별
    private static bool Is_CorrectID(int id, IDController obj)
    {
        if (obj.id == id) return true;
        return false;
    }


    #endregion
}
