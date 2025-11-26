using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] int _poolSize = 5;
    [SerializeField] float _health = 4;
    [SerializeField] float _spawnDelay = 3;

    public bool _isGameStart = false;
    private float timeChecker;
    private void Update()
    {
        if(_isGameStart)
        timeChecker += Time.deltaTime;
    }
    public void SpawnEnemy(Transform position)
    {
        if (timeChecker > _spawnDelay)
        {
            timeChecker = 0;
            Instantiate(_enemyPrefab, position);
        }
    }
}
