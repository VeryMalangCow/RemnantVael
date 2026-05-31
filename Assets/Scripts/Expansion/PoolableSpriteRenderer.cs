using UnityEngine;

public class PoolableSpriteRenderer : MonoBehaviour, IPoolable
{
    [SerializeField] public SpriteRenderer spriteRenderer;
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
}
