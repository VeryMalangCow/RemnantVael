using System.Collections.Generic;
using UnityEngine;

public class HudPlayerBuffView : MonoBehaviour
{
    [SerializeField] private Transform buffParentTf;
    [SerializeField] public List<BuffIconEUIController> allBuffIconUi;
    [SerializeField] private float buffUiXInterval = 12;

    public void Init()
    {

        gameObject.SetActive(true);
    }


    public void SetBuffPosUI()
    {
        for (int i = 0; i < allBuffIconUi.Count; i++)
            allBuffIconUi[i].rt.anchoredPosition = new Vector2(i * (allBuffIconUi[i].rt.rect.width + buffUiXInterval), 0);
    }

    public void AddBuff(BuffIconEUIController buffIconEui)
    {
        DevTool.Add_InList(allBuffIconUi, buffIconEui);
    }

    public void RemoveBuff(BuffIconEUIController buffIconEui)
    {
        DevTool.Remove_InList(allBuffIconUi, buffIconEui);
    }
}
