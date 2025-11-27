using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject _enemyPrefab; // 생성할 적 프리팹입니다.

    /// <summary>
    /// 입력받은 좌표에 적을 생성합니다.
    /// </summary>
    /// <param name="position">적을 생성할 좌표.</param>
    public void SpawnEnemy(Vector3 position)
    {
        Instantiate(_enemyPrefab, position, Quaternion.identity);
    }
}
