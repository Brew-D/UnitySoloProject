using UnityEngine;

public class EnemyStatePattern : MonoBehaviour
{
    public float _health = 5;

    public void GetHit()
    {
        _health--;
    }
}
