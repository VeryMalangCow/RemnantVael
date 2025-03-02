using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffUIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff")]

    [Space(10)]
    [Header("=== Buff")]
    [SerializeField] private List<BuffIconEUIController> BuffIconUIs;
    [SerializeField] private List<BuffIconEUIController> UsingBuffIconUIs = new List<BuffIconEUIController>();
    [SerializeField] private float XYInterval = 44;
    [SerializeField] private int WidthMaxAmount = 5;

    [HideInInspector] public EnemyHUDController EnemyHUD;

    #endregion

    #region Offset

    public void Offset(EnemyHUDController _EnemyHUD)
    {
        EnemyHUD = _EnemyHUD;

        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).TryGetComponent(out BuffIconEUIController MBI))
            { 
                BuffIconUIs.Add(MBI);
                MBI.Offset();
            }
        }
    }

    #endregion

    #region Get & Set

    // 사용하지 않는 중인 버프 Icon UI
    public BuffIconEUIController Get_BuffIconUI()
    {
        for (int i = 0; i < BuffIconUIs.Count; i++)
        {
            if (!BuffIconUIs[i].UsingNow)
            {
                if (!UsingBuffIconUIs.Contains(BuffIconUIs[i]))
                { UsingBuffIconUIs.Add(BuffIconUIs[i]); }

                Set_BuffUIPos();

                return BuffIconUIs[i];
            }
        }
        return null;
    }

    // 사용중인 버프 Icon UI 리스트에서 제거
    public void Remove_BuffIconUI(BuffIconEUIController _BuffIconUI)
    {
        _BuffIconUI.SetOff();

        if (UsingBuffIconUIs.Contains(_BuffIconUI))
        { UsingBuffIconUIs.Remove(_BuffIconUI); }

        Set_BuffUIPos();
    }

    // 현재 진행 중인 버프의 종류가 바뀔 때 마다 실행
    private void Set_BuffUIPos()
    {
        for (int i = 0; i < UsingBuffIconUIs.Count; i++)
        {
            int height = i / WidthMaxAmount;
            int width = i % WidthMaxAmount;
            UsingBuffIconUIs[i].ThisRT.anchoredPosition
                = new Vector2(XYInterval * width, XYInterval * height);
        }
    }

    #endregion

    
}
