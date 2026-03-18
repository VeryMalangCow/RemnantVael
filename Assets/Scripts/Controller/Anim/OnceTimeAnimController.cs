using UnityEngine;

public class OnceTimeAnimController : MonoBehaviour
{
    #region Value

    [SerializeField] private Animator ThisAnimator;
    [SerializeField] private SpriteRenderer ThisSpriteRenderer;
    [HideInInspector] private AnimatorOverrideController AOC;

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
        if (DevTool.Is_AnimIsDone(ThisAnimator))
        {
            End_Anim();
        }
    }

    #endregion

    #region Anim

    public void Start_Anim(
        State_Anim _State_Anim,
        State_TF2D _StructTF, 
        State_Sprite _SpriteExtra)
    {
        ThisAnimator.enabled = true;
        ThisAnimator.speed = _State_Anim.Speed;

        DevTool.Set_TF_FromStruct(gameObject.transform, _StructTF);
        DevTool.Set_MatAndClr_FromStruct(ThisSpriteRenderer, _SpriteExtra);
        DevTool.Set_Anim(ref AOC, ThisAnimator, _State_Anim.AC);

        this.gameObject.SetActive(true);
    }

    private void End_Anim()
    {
        AOC = null;
        ThisAnimator.speed = 0f;
        ThisAnimator.enabled = false;

        this.gameObject.SetActive(false);

        PoolingManager.Instance.onlyOnceAnimators.Enqueue(this);
    }

    #endregion
}
