using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ModifyMinimapElement : UIModule
{
    #region Value

    [HideInInspector] private Image ThisMMImg = null;
    [HideInInspector] private Image ThisMMOImg = null;
    [HideInInspector] private CanvasGroup ThisCG = null;
    [HideInInspector] private RoomController ConnectedRC = null;

    [HideInInspector] private Color ThisMainColor;

    [SerializeField] private Color UnknowColor;
    [SerializeField] private const float IntervalEachMM = 36;
    [SerializeField] private const float IntervalEachIMM = 60;

    #endregion

    #region Offset

    public void Offset(RoomController _RC, Color _MainColor, bool _IsNormal)
    {
        Offset();

        ConnectedRC = _RC;
        ThisMainColor = _MainColor;


        if (_IsNormal)
        {
            ConnectedRC.ThisMME = this;
            ThisMMImg.sprite = ConnectedRC.ThisSpriteMM;
            ThisMMOImg.sprite = ConnectedRC.ThisSpriteMMO;
        }
        else
        {
            ConnectedRC.ThisIMME = this;
            ThisMMImg.sprite = ConnectedRC.ThisSpriteMMI;
            ThisMMOImg.sprite = ConnectedRC.ThisSpriteMMIO;
        }

        ThisMMImg.SetNativeSize();
        ThisMMOImg.SetNativeSize();

        if (this.TryGetComponent(out RectTransform rt))
        {
            rt.pivot = _RC.SpritePivot;
            if (_IsNormal)
            {
                rt.anchoredPosition = new Vector2(
                    (float)_RC.RoomVec[0].x * IntervalEachMM,
                    (float)_RC.RoomVec[0].y * IntervalEachMM);
            }
            else
            {
                rt.anchoredPosition = new Vector2(
                    (float)_RC.RoomVec[0].x * IntervalEachIMM,
                    (float)_RC.RoomVec[0].y * IntervalEachIMM);
            }
        }
    }

    public override void Offset()
    {
        if (this.TryGetComponent(out Image MM_Img))
        {
            ThisMMImg = MM_Img;
        }
        if (this.TryGetComponent(out CanvasGroup CG))
        {
            ThisCG = CG;
            ThisCG.alpha = 0f;
        }
        if (this.transform.GetChild(0).TryGetComponent(out Image MMO_Img))
        {
            ThisMMOImg = MMO_Img;
        }
    }

    #endregion

    #region SetState

    public void SetState_Complete()
    {
        ThisMMOImg.DOColor(ThisMainColor, 0.5f);
    }

    public void SetState_Uncomplete()
    {
        ThisMMOImg.DOColor(UnknowColor, 0.5f);
    }

    public void SetState_Visible()
    {
        ThisMMOImg.color = new Color(0, 0, 0, 0.5f);
    }

    public void SetActiveOn()
    {
        this.gameObject.SetActive(true);
        ThisCG.DOFade(1f, 0.5f);
    }

    #endregion
}
