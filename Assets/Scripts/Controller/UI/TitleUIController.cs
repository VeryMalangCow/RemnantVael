using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUIController : UIController
{
    #region Value

    [Space(10)]
    [Header("=== Intro")]
    [SerializeField] private ModifyPanelBtn StartPanelBtn;

    [Space(10)]
    [Header("=== End")]
    [SerializeField] private ModifyPanelBtn EndPanleBtn;

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private Button StartBtn;
    [SerializeField] private List<Button> ChooseCharBtns;

    #endregion

    #region Offset

    protected override void Offset_Module()
    {
        StartPanelBtn.Offset();
    }

    protected override void Offset_UI()
    {
        StartBtn.OnClickAsObservable()
            .Subscribe(btn =>
            {
                SceneManager.LoadScene("Lobby");
            });

        for (int i = 0; i < ChooseCharBtns.Count; i++)
        {
            Button btn = ChooseCharBtns[i];
            GameObject prefab = GameManager.Instance.AllPlayerPrefabs[i];

            btn.OnClickAsObservable()
                .Subscribe(btn => 
                {
                    GameManager.Instance.DesignatedPlayerPrefab = prefab;
                });
        }
    }

    #endregion
}
