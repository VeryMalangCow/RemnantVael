using UnityEngine;

public class StateAnimController : MonoBehaviour
{
    #region Value

    [Space(10)]
    [Header("=== Component")]
    [SerializeField] private Animator at;
    [SerializeField] public SpriteRenderer sr;
    [SerializeField] public SpriteRenderer innerSr;

    [HideInInspector] private AnimatorOverrideController AOC;

    #endregion

    #region Anim

    public void Set_Anim(State_Anim state_Anim, Sprite innerSprite, float animSize = 1f)
    {
        DevTool.Set_Anim(ref AOC, at, state_Anim.ac);
        DevTool.Set_AnimSpeedAndSize(at, state_Anim.speed, animSize);
        Set_Inner(true, innerSprite);
    }

    public void Set_Anim(State_Anim state_Anim, float animSize = 1f)
    {
        DevTool.Set_Anim(ref AOC, at, state_Anim.ac);
        DevTool.Set_AnimSpeedAndSize(at, state_Anim.speed, animSize);
        Set_Inner(false);
    }

    public void Set_Inner(bool onOff, Sprite sprite = null)
    {
        if (innerSr != null)
        {
            innerSr.gameObject.SetActive(onOff);
            if (onOff && sprite != null)
            {
                innerSr.sprite = sprite;
            }
        }
    }

    #endregion

}