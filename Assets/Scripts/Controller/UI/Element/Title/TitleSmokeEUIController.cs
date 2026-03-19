using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSmokeEUIController : ElementUIController
{
    #region Value

    #region - Inspector

    [Space(20)]
    [Header("<><><><><> Title Smoke")]

    [Space(10)]
    [Header("=== Sprite")]
    [SerializeField] private List<Sprite> SmokeSpriteList;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private RectTransform YRT;
    [SerializeField] private RectTransform LeftRT;
    [SerializeField] private RectTransform RightRT;

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private float StartSize;
    [SerializeField] private CoupleData<Color> DurColor;
    [SerializeField] private CoupleData<float> MovingDis;
    [SerializeField] private CoupleData<float> MovingTime;
    [SerializeField] private CoupleData<float> DelayTime;

    #endregion

    #region - Hide

    [HideInInspector] public TitleLobbyUIController OwnerUIController;

    [HideInInspector] private GameObject CellEUIPrefab;
    [HideInInspector] private float YPos = 0;
    [HideInInspector] private float LeftXPos = 0;
    [HideInInspector] private float RightXPos = 0;

    [HideInInspector] private Queue<TitleSmokeCellEUIController> CellQueue = new Queue<TitleSmokeCellEUIController>();
    [HideInInspector] private Coroutine ThisCor = null;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        CellEUIPrefab = OwnerUIController.SmokeCellEUIPrefab;

        YPos = YRT.anchoredPosition.y;
        LeftXPos = LeftRT.anchoredPosition.x;
        RightXPos = RightRT.anchoredPosition.x;

        ThisCor = StartCoroutine(Play_VFX_Cor());
    }

    #endregion

    #region Play

    private IEnumerator Play_VFX_Cor()
    {
        while (true)
        {
            TitleSmokeCellEUIController cell = Get_OP(CellEUIPrefab);
            cell.transform.SetParent(this.transform);
            cell.OwnerEUIController = this;
            cell.Offset();

            cell.Play_Smoke(
                Get_RandomSprite(),
                DurColor,
                StartSize,
                Get_RandomSpotX(),
                Get_RandomDis(),
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

    public void Set_OP_Enqueue(TitleSmokeCellEUIController _CellEUI)
    {
        if (!CellQueue.Contains(_CellEUI))
            CellQueue.Enqueue(_CellEUI);
    }

    #endregion

    #region Get

    private TitleSmokeCellEUIController Get_OP(GameObject _SpawnGO)
    {
        // No Object
        if (CellQueue.Count <= 0)
        {
            GameObject GenGO = Instantiate(_SpawnGO);
            GenGO.TryGetComponent(out TitleSmokeCellEUIController typeClass);
            GenGO.SetActive(false);

            return typeClass;
        }
        else
        {
            TitleSmokeCellEUIController getTypeClass = CellQueue.Dequeue();

            return getTypeClass;
        }
    }

    // ½ºÆù ·£´ý µô·¹ÀÌ
    private float Get_RandomDelay()
    {
        return Random.Range(DelayTime.typeBase, DelayTime.typeSpecial);
    }

    // ·£´ý °Å¸®
    private float Get_RandomDis()
    {
        return Random.Range(MovingDis.typeBase, MovingDis.typeSpecial);
    }

    // ·£´ý ½Ã°£
    private float Get_RandomDurTime()
    {
        return Random.Range(MovingTime.typeBase, MovingTime.typeSpecial);
    }

    // ·£´ý ½ºÆù À§Ä¡
    private float Get_RandomSpotX()
    {
        return Random.Range(LeftXPos, RightXPos);
    }

    // ·£´ý ½ºÇÁ¶óÀÌÆ®
    private Sprite Get_RandomSprite()
    {
        return SmokeSpriteList[Random.Range(0, SmokeSpriteList.Count - 1)];
    }

    #endregion
}
