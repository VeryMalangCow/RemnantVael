using UnityEngine;

public class OnceTimeAnimController : MonoBehaviour, IPoolable
{
    #region Value

    [SerializeField] private Animator thisAnimator;
    [SerializeField] private SpriteRenderer thisSpriteRenderer;
    [HideInInspector] private AnimatorOverrideController aoc;

    #endregion

    public int PoolIndex { get; set; } = -1;
    public int ActiveIndex { get; set; } = -1;

    #region Pool

    public void PoolOffset()
    {

    }

    public void SetActiveOn()
    {

    }

    public void SetActiveOff()
    {

    }

    #endregion

    #region Framework

    private void Update()
    {
        Update_CheckingEndAnim();
    }

    #endregion

    #region Update

    private void Update_CheckingEndAnim()
    {
        if (DevTool.Is_AnimIsDone(thisAnimator))
        {
            End_Anim();
        }
    }

    #endregion

    #region Anim

    public void Start_Anim(
        State_Anim stateAnim,
        State_TF2D structTf, 
        State_Sprite spriteExtra)
    {
        thisAnimator.enabled = true;
        thisAnimator.speed = stateAnim.speed;

        DevTool.Set_TF_FromStruct(gameObject.transform, structTf);
        DevTool.Set_MatAndClr_FromStruct(thisSpriteRenderer, spriteExtra);
        DevTool.Set_Anim(ref aoc, thisAnimator, stateAnim.ac);

        this.gameObject.SetActive(true);
    }

    private void End_Anim()
    {
        aoc = null;
        thisAnimator.speed = 0f;
        thisAnimator.enabled = false;

        this.gameObject.SetActive(false);

        VFXManager.instance.RemoveOnlyOnceAnim(this);
    }

    #endregion
}
