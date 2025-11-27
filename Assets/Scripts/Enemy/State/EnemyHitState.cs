using UnityEngine;

public class EnemyHitState : IEnemyState
{
    private EnemyStatePattern _enemy;
    private bool _isGround;

    /// <summary>
    /// 플레이어의 상태패턴 관리용 코드를 받아 해당 코드에서 필요한 내용을 가져옵니다.
    /// </summary>
    /// <param name="enemy">EnemyStatePattern 코드</param>
    public EnemyHitState(EnemyStatePattern enemy)
    {
        _enemy = enemy;
    }

    public void OnEnter()
    {
        _enemy.HitCheck(true);
        _enemy.AttackCheck(false);
        _enemy.MoveCheck(false);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        if (_enemy._health >= 0.1)
        {
            _enemy.GetHit();
            _enemy._anim.SetFloat("Health", _enemy._health);
            if (_enemy._isMove)
            {
                _enemy.SetState(new EnemyWalkState(_enemy));
            }
            else if (!_enemy._isMove)
            {
                _enemy.SetState(new EnemyIdleState(_enemy));
            }
        }
        else if (_enemy._health < 0.1)
            _enemy._anim.SetFloat("Health", 0);
    }
}
