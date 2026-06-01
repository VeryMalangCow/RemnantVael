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
        gameObject.SetActive(false);
    }

    public void SetActiveOn()
    {
        gameObject.SetActive(true);
    }

    public void SetActiveOff()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Update

    public void HandleCheckingEndAnim()
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
    }

    private void End_Anim()
    {
        aoc = null;
        thisAnimator.speed = 0f;
        thisAnimator.enabled = false;

        VFXManager.instance.RemoveOnlyOnceAnim(this);
    }

    #endregion
}
