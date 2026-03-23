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
    [SerializeField] private List<Sprite> smokeSpriteList;

    [Space(10)]
    [Header("=== Comp")]
    [SerializeField] private RectTransform yRt;
    [SerializeField] private RectTransform leftRt;
    [SerializeField] private RectTransform rightRt;

    [Space(10)]
    [Header("=== Data")]
    [SerializeField] private float startSize;
    [SerializeField] private CoupleData<Color> durColor;
    [SerializeField] private CoupleData<float> movingDis;
    [SerializeField] private CoupleData<float> movingTime;
    [SerializeField] private CoupleData<float> delayTime;

    #endregion

    #region - Hide

    [HideInInspector] public TitleLobbyUIController ownerUIController;

    [HideInInspector] private GameObject cellEuiPrefab;
    [HideInInspector] private float yPos = 0;
    [HideInInspector] private float leftXPos = 0;
    [HideInInspector] private float rightXPos = 0;

    [HideInInspector] private Queue<TitleSmokeCellEUIController> cellQueue = new Queue<TitleSmokeCellEUIController>();
    [HideInInspector] private Coroutine cor = null;

    #endregion

    #endregion

    #region Offset

    public override void Offset()
    {
        cellEuiPrefab = ownerUIController.smokeCellEuiPrefab;

        yPos = yRt.anchoredPosition.y;
        leftXPos = leftRt.anchoredPosition.x;
        rightXPos = rightRt.anchoredPosition.x;

        cor = StartCoroutine(Play_VFX_Cor());
    }

    #endregion

    #region Play

    private IEnumerator Play_VFX_Cor()
    {
        while (true)
        {
            TitleSmokeCellEUIController cell = Get_OP(cellEuiPrefab);
            cell.transform.SetParent(this.transform);
            cell.ownerEuiController = this;
            cell.Offset();

            cell.Play_Smoke(
                Get_RandomSprite(),
                durColor,
                startSize,
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
        if (cor != null)
            StopCoroutine(cor);
    }

    #endregion

    #region Set

    public void Set_OP_Enqueue(TitleSmokeCellEUIController cellEui)
    {
        if (!cellQueue.Contains(cellEui))
            cellQueue.Enqueue(cellEui);
    }

    #endregion

    #region Get

    private TitleSmokeCellEUIController Get_OP(GameObject spawnGO)
    {
        // No Object
        if (cellQueue.Count <= 0)
        {
            GameObject GenGO = Instantiate(spawnGO);
            GenGO.TryGetComponent(out TitleSmokeCellEUIController typeClass);
            GenGO.SetActive(false);

            return typeClass;
        }
        else
        {
            TitleSmokeCellEUIController getTypeClass = cellQueue.Dequeue();

            return getTypeClass;
        }
    }

    // ½ºÆù ·£´ý µô·¹ÀÌ
    private float Get_RandomDelay()
    {
        return Random.Range(delayTime.typeBase, delayTime.typeSpecial);
    }

    // ·£´ý °Å¸®
    private float Get_RandomDis()
    {
        return Random.Range(movingDis.typeBase, movingDis.typeSpecial);
    }

    // ·£´ý ½Ã°£
    private float Get_RandomDurTime()
    {
        return Random.Range(movingTime.typeBase, movingTime.typeSpecial);
    }

    // ·£´ý ½ºÆù À§Ä¡
    private float Get_RandomSpotX()
    {
        return Random.Range(leftXPos, rightXPos);
    }

    // ·£´ý ½ºÇÁ¶óÀÌÆ®
    private Sprite Get_RandomSprite()
    {
        return smokeSpriteList[Random.Range(0, smokeSpriteList.Count - 1)];
    }

    #endregion
}
