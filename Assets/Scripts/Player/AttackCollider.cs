using Unity.Behavior;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField]PlayerStatePattern _player;
    [SerializeField]BoxCollider2D _boxCollider;
    [SerializeField]AudioSource _audio;

    float _timeChecker = 0;
    float _period = 0.25f;
    bool _hitTimePassed = false;

    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.layer == 14 && _player._isAttack == true && !_hitTimePassed)
        {
            EnemyStatePattern enemy = collision.GetComponent<EnemyStatePattern>();

            enemy.GetHit();
            _hitTimePassed = true;
        }
    }

    private void Update()
    {
        if (!_boxCollider.enabled)
            _timeChecker = 0;

        _timeChecker += Time.deltaTime;
        if (_timeChecker > _period)
        {
            _timeChecker = 0;
            _audio.Play();
            _boxCollider.enabled = false;
            _hitTimePassed = false;
        }
    }

    public void TimeFreeze()
    {
        GameManager.Instance.Credit();
        Time.timeScale = 0;
    }
}
