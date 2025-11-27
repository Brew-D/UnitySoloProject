using UnityEngine;

public class WorldBoarder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 14)
        {
            EnemyStatePattern _enemy = collision.gameObject.GetComponent<EnemyStatePattern>();
            _enemy._rigid.transform.position = _enemy._spawnLocation;
        }
        if (collision.gameObject.layer == 15)
        {
            PlayerStatePattern _player = collision.gameObject.GetComponent<PlayerStatePattern>();
            if (GameManager.Instance._isStarted)
            {
                _player._health = 1;
                _player.HitChecker(true);
            }
            else
            {
                _player.transform.position = _player._spawnLocation;
            }
        }
    }
}
