using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBuffUIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff")]

    [Space(10)]
    [Header("=== Buff")]
    [SerializeField] private float XYInterval = 44;
    [SerializeField] private int WidthMaxAmount = 5;

    [HideInInspector] private List<BuffIconEUIController> BuffIconUIs = new List<BuffIconEUIController>();
    [HideInInspector] private List<BuffIconEUIController> UsingBuffIconUIs = new List<BuffIconEUIController>();
    [HideInInspector] public AllyHUDController AllyHUD;

    #endregion

    #region Offset

    public void Offset(AllyHUDController _AllyHUD)
    {
        AllyHUD = _AllyHUD;

        BuffIconUIs = DevTool.Get_ChildList<BuffIconEUIController>(this.transform);
        for (int i = 0; i < BuffIconUIs.Count; i++) BuffIconUIs[i].Offset();
    }

    #endregion

    #region Get

    // 사용하지 않는 중인 버프 Icon UI
    public BuffIconEUIController Get_BuffIconUI()
    {
        for (int i = 0; i < BuffIconUIs.Count; i++)
        {
            if (!BuffIconUIs[i].UsingNow)
            {
                DevTool.Add_InList(UsingBuffIconUIs, BuffIconUIs[i]);

                Set_BuffUIPos();

                return BuffIconUIs[i];
            }
        }
        return null;
    }

    #endregion

    #region Remove

    // 사용중인 버프 Icon UI 리스트에서 제거
    public void Remove_BuffIconUI(BuffIconEUIController _BuffIconUI)
    {
        _BuffIconUI.SetOff();

        DevTool.Remove_InList(UsingBuffIconUIs, _BuffIconUI);

        Set_BuffUIPos();
    }

    #endregion

    #region Set

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
