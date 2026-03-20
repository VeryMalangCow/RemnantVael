using UnityEngine;

public class OnceTimeAnimController : MonoBehaviour
{
    #region Value

    [SerializeField] private Animator thisAnimator;
    [SerializeField] private SpriteRenderer thisSpriteRenderer;
    [HideInInspector] private AnimatorOverrideController aoc;

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

        PoolingManager.instance.onlyOnceAnimators.Enqueue(this);
    }

    #endregion
}
