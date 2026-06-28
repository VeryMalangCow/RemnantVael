
// +클리어할 시 변경 적용하는 Build Sprite System
using UnityEngine;

public class RoomVisualMaterial : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer thisSr;
    public SpriteRenderer ThisSr { get { return ThisSr; } }

#if UNITY_EDITOR
    public void SetData()
    {
        thisSr = GetComponent<SpriteRenderer>();
    }

#endif

}
