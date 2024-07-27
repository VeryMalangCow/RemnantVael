using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    #region Value


    #endregion

}


[System.Serializable]
public class EnemyLifeState
{
    [SerializeField] public float HealthPoint = 100;
}