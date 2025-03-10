using UnityEngine;

public class StateAnimController : MonoBehaviour
{
    #region Value

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private Animator ThisAnimator;
    [SerializeField] public SpriteRenderer ThisSR;
    [SerializeField] private SpriteRenderer ThisInnerSR;

    [HideInInspector] private AnimatorOverrideController AOC;

    #endregion

    #region Anim

    public void Set_Anim(AnimationClip _AC, Sprite _InnerSprite, float _AnimSpeed = 1f, float _AnimSize = 1f)
    {
        DevTool.Set_Anim(ref AOC, ThisAnimator, _AC);
        DevTool.Set_AnimSpeedAndSize(ThisAnimator, _AnimSpeed, _AnimSize);
        Set_Inner(true, _InnerSprite);
    }

    public void Set_Anim(AnimationClip _AC, float _AnimSpeed = 1f, float _AnimSize = 1f)
    {
        DevTool.Set_Anim(ref AOC, ThisAnimator, _AC);
        DevTool.Set_AnimSpeedAndSize(ThisAnimator, _AnimSpeed, _AnimSize);
        Set_Inner(false);
    }

    public void Set_Inner(bool _OnOff, Sprite _Sprite = null)
    {
        if (ThisInnerSR != null)
        {
            ThisInnerSR.gameObject.SetActive(_OnOff);
            if (_OnOff && _Sprite != null)
            {
                ThisInnerSR.sprite = _Sprite;
            }
        }
    }

    #endregion

}