using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCloudEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Title Cloud")]

    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] private List<Sprite> CloudSpriteList;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private RectTransform XRT;
    [SerializeField] private RectTransform UpRT;
    [SerializeField] private RectTransform DownRT;

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private float StartSize;
    [SerializeField] private CoupleData<float> MovingTime;
    [SerializeField] private CoupleData<float> DelayTime;

    #endregion

    #region - Hide

    [HideInInspector] public TitleLobbyUIController OwnerUIController;

    [HideInInspector] private GameObject CellEUIPrefab;
    [HideInInspector] private float XPos = 0;
    [HideInInspector] private float EnxXPos = 0;
    [HideInInspector] private float UpYPos = 0;
    [HideInInspector] private float DownYPos = 0;

    [HideInInspector] private Queue<TitleCloudCellEUIController> CellQueue = new Queue<TitleCloudCellEUIController>();
    [HideInInspector] private Coroutine ThisCor = null;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        CellEUIPrefab = OwnerUIController.CloudCellEUIPrefab;

        XPos = XRT.anchoredPosition.x;
        EnxXPos = XPos * -2;
        UpYPos = UpRT.anchoredPosition.y;
        DownYPos = DownRT.anchoredPosition.y;

        ThisCor = StartCoroutine(Play_VFX_Cor());
    }

    #endregion

    #region Play

    private IEnumerator Play_VFX_Cor()
    {
        while (true)
        {
            TitleCloudCellEUIController cell = Get_OP(CellEUIPrefab);
            cell.transform.SetParent(this.transform);
            cell.OwnerEUIController = this;
            cell.Offset();

            cell.Play_Cloud(
                Get_RandomSprite(),
                StartSize,
                Get_RandomSpotY(),
                EnxXPos,
                Get_RandomDurTime());

            yield return new WaitForSeconds(Get_RandomDelay());
        }
    }

    #endregion

    #region End

    public void Stop_VFX()
    {
        if (ThisCor != null)
            StopCoroutine(ThisCor);
    }

    #endregion

    #region Set

    public void Set_OP_Enqueue(TitleCloudCellEUIController _CellEUI)
    {
        CellQueue.Enqueue(_CellEUI);
    }

    #endregion

    #region Get

    private TitleCloudCellEUIController Get_OP(GameObject _SpawnGO)
    {
        // No Object
        if (CellQueue.Count <= 0)
        {
            GameObject GenGO = Instantiate(_SpawnGO);
            GenGO.TryGetComponent(out TitleCloudCellEUIController typeClass);
            GenGO.SetActive(false);

            return typeClass;
        }
        else
        {
            TitleCloudCellEUIController getTypeClass = CellQueue.Dequeue();

            return getTypeClass;
        }
    }

    // ½ºÆù ·£´ý µô·¹ÀÌ
    private float Get_RandomDelay()
    {
        return Random.Range(DelayTime.TypeBase, DelayTime.TypeSpecial);
    }

    // ·£´ý ½Ã°£
    private float Get_RandomDurTime()
    {
        return Random.Range(MovingTime.TypeBase, MovingTime.TypeSpecial);
    }

    // ·£´ý ½ºÆù À§Ä¡
    private float Get_RandomSpotY()
    {
        return Random.Range(DownYPos, UpYPos);
    }

    // ·£´ý ½ºÇÁ¶óÀÌÆ®
    private Sprite Get_RandomSprite()
    {
        return CloudSpriteList[Random.Range(0, CloudSpriteList.Count - 1)];
    }

    #endregion
}
