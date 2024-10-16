using System.Collections.Generic;
using UnityEngine;

public class MakeExplosionImage : MonoBehaviour
{
    #region Value

    [Space(10)]
    [Header("=== Spawned")]
    [SerializeField] private List<SpriteRenderer> SpawnedSRList;

    #endregion

    private void OnEnable()
    {
        //GenExplosionImgs();
    }

    #region Explosion

    public void GenExplosionImgs(int _SpawnImgAmount, float _GrowingUpTime, float _GettingSmallerTime)
    {

    }

    #endregion
}
