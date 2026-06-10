using System.Collections.Generic;
using UnityEngine;

public class EnemyBuffUIController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Buff")]

    [Space(10)]
    [Header("=== Buff")]
    [SerializeField] private float xyInterval = 44;
    [SerializeField] private int widthMaxAmount = 5;

    [HideInInspector] private List<BuffIconEUIController> buffIconUiList = new List<BuffIconEUIController>();
    [HideInInspector] private List<BuffIconEUIController> usingBuffIconUiList = new List<BuffIconEUIController>();
    [HideInInspector] public EnemyHUDController enemyHud;

    #endregion

    #region Offset

    public void Offset(EnemyHUDController enemyHud)
    {
        this.enemyHud = enemyHud;

        buffIconUiList = DevTool.Get_ChildList<BuffIconEUIController>(this.transform);
        for (int i = 0; i < buffIconUiList.Count; i++) buffIconUiList[i].Offset();   
        
    }

    #endregion

    #region Get

    // 사용하지 않는 중인 버프 Icon UI
    public BuffIconEUIController Get_BuffIconUI()
    {
        for (int i = 0; i < buffIconUiList.Count; i++)
        {
            if (!buffIconUiList[i].usingNow)
            {
                DevTool.Add_InList(usingBuffIconUiList, buffIconUiList[i]);

                Set_BuffUIPos();

                return buffIconUiList[i];
            }
        }
        return null;
    }

    #endregion

    #region Remove

    // 사용중인 버프 Icon UI 리스트에서 제거
    public void Remove_BuffIconUI(BuffIconEUIController buffIconUi)
    {
        buffIconUi.SetOff();

        DevTool.Remove_InList(usingBuffIconUiList, buffIconUi);

        Set_BuffUIPos();
    }

    #endregion

    #region Set

    // 현재 진행 중인 버프의 종류가 바뀔 때 마다 실행
    private void Set_BuffUIPos()
    {
        for (int i = 0; i < usingBuffIconUiList.Count; i++)
        {
            int height = i / widthMaxAmount;
            int width = i % widthMaxAmount;
            usingBuffIconUiList[i].rt.anchoredPosition
                = new Vector2(xyInterval * width, xyInterval * height);
        }
    }

    #endregion
}
