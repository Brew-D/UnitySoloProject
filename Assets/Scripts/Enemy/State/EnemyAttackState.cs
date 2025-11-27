using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private EnemyStatePattern _enemy;
    private BoxCollider2D _attackRange;


    private float _currentAttackDuration;

    /// <summary>
    /// 플레이어의 상태패턴 관리용 코드를 받아 해당 코드에서 필요한 내용을 가져옵니다.
    /// </summary>
    /// <param name="player">PlayerStatePattern 코드</param>
    public EnemyAttackState(EnemyStatePattern enemy)
    {
        _enemy = enemy;
        _attackRange = _enemy._attackRange;
        _attackRange.enabled = true;
    }

    public void OnEnter()
    {
        _enemy.AttackCheck(true);
        _enemy.HitCheck(false);
        _enemy.MoveCheck(false);
    }

    public void OnExit()
    {

    }


    public void OnUpdate()
    {
        if (!_enemy._isAttack)
        {
            if (_enemy._isMove)
            {
                _enemy.SetState(new EnemyWalkState(_enemy));
            }
            else if (!_enemy._isMove)
            {
                _enemy.SetState(new EnemyIdleState(_enemy));
            }
            else if (_enemy._isHit)
            {
                _enemy.SetState(new EnemyHitState(_enemy));
            }

        }
        if (_enemy._health < 0.1)
            _enemy._anim.SetFloat("Health", 0);
    }
}
