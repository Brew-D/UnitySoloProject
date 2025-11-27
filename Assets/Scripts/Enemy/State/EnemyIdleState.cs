using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private EnemyStatePattern _enemy;
    private bool _isGround;

    public EnemyIdleState(EnemyStatePattern enemy)
    {
        _enemy = enemy;
        _isGround = _enemy._isGround;
    }

    private void Update()
    {

    }
    public void OnEnter()
    {
        _enemy.MoveCheck(false);
        _enemy.AttackCheck(false);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        _isGround = _enemy._isGround;
        if (_enemy._isMove)
        {
            _enemy.SetState(new EnemyWalkState(_enemy));
        }
        else if (_isGround && _enemy._isAttack)
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
