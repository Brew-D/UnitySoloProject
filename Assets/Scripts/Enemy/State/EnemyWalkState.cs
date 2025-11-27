using UnityEngine;

public class EnemyWalkState : IEnemyState
{
    private EnemyStatePattern _enemy;

    public EnemyWalkState(EnemyStatePattern player)
    {
        _enemy = player;
    }

    public void OnEnter()
    {
        _enemy.MoveCheck(true);
        _enemy.AttackCheck(false);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        if (!_enemy._isMove)
        {
            _enemy.SetState(new EnemyIdleState(_enemy));
        }
        else if (_enemy._isAttack == true)
        {
            _enemy.SetState(new EnemyAttackState(_enemy));
        }
        else if (_enemy._isHit)
        {
            _enemy.SetState(new EnemyHitState(_enemy));
        }
        if (_enemy._health < 0.1)
            _enemy._anim.SetFloat("Health", 0);
    }
}
